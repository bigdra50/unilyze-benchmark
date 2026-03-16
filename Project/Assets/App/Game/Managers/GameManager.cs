using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using App.Core.Dungeon;
using App.Core.Events;
using App.Core.Models;
using App.Core.Turn;
using App.Game.Bootstrap;
using App.Game.Camera;
using App.Game.Effects;
using App.Game.Fog;
using App.Game.Input;
using App.Game.UI;
using App.Game.Views;
using UnityEngine;
using VContainer.Unity;
using Object = UnityEngine.Object;

namespace App.Game.Managers
{
    public sealed class GameManager : ITickable, IDisposable
    {
        private readonly DungeonManager _dungeonManager;
        private readonly IInputProvider _playerInput;
        private readonly CameraShake _cameraShake;
        private readonly DungeonView _dungeonView;
        private readonly PlayerView _playerView;
        private readonly DungeonCamera _dungeonCamera;
        private readonly FogRenderer _fogRenderer;
        private readonly HudController _hud;
        private readonly MessageLog _messageLog;
        private readonly GameStateHolder _gameStateHolder;
        private readonly CancellationTokenSource _cts = new();

        public GameState GameState { get; private set; }
        public Action OnGameOver;

        private TurnSystem _turnSystem;

        private Dictionary<Guid, EnemyView> _enemyViews = new();
        private Dictionary<Item, ItemView> _itemViews = new();

        private bool _isPaused;

        public GameManager(
            DungeonManager dungeonManager,
            IInputProvider playerInput,
            CameraShake cameraShake,
            DungeonView dungeonView,
            PlayerView playerView,
            DungeonCamera dungeonCamera,
            FogRenderer fogRenderer,
            HudController hud,
            MessageLog messageLog,
            GameStateHolder gameStateHolder)
        {
            _dungeonManager = dungeonManager;
            _playerInput = playerInput;
            _cameraShake = cameraShake;
            _dungeonView = dungeonView;
            _playerView = playerView;
            _dungeonCamera = dungeonCamera;
            _fogRenderer = fogRenderer;
            _hud = hud;
            _messageLog = messageLog;
            _gameStateHolder = gameStateHolder;
        }

        public void StartGame()
        {
            _turnSystem = new TurnSystem();

            var player = new Player("Hero", new Position(0, 0), new Stats(30, 8, 3));
            GameState = new GameState
            {
                Player = player,
                CurrentFloor = 1,
                TurnNumber = 0
            };
            _gameStateHolder.Current = GameState;

            GenerateFloor();
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            var bus = GameState.EventBus;

            bus.Subscribe<EntityMoved>(OnEntityMoved);
            bus.Subscribe<EntityDamaged>(OnEntityDamaged);
            bus.Subscribe<EntityDied>(OnEntityDied);
            bus.Subscribe<ItemPickedUp>(OnItemPickedUp);
            bus.Subscribe<PlayerLeveledUp>(OnPlayerLeveledUp);
        }

        private void UnsubscribeEvents()
        {
            if (GameState?.EventBus == null) return;

            var bus = GameState.EventBus;
            bus.Unsubscribe<EntityMoved>(OnEntityMoved);
            bus.Unsubscribe<EntityDamaged>(OnEntityDamaged);
            bus.Unsubscribe<EntityDied>(OnEntityDied);
            bus.Unsubscribe<ItemPickedUp>(OnItemPickedUp);
            bus.Unsubscribe<PlayerLeveledUp>(OnPlayerLeveledUp);
        }

        private void GenerateFloor()
        {
            ClearViews();

            var result = _dungeonManager.GenerateFloor(GameState.CurrentFloor, new System.Random());

            GameState.Map = result.Map;
            GameState.Enemies = new List<Enemy>(result.Enemies);
            GameState.Items = new List<Item>(result.Items);
            GameState.Player.Position = result.PlayerStart;

            _dungeonView.Initialize(result.Map);
            _playerView.SyncPosition(result.PlayerStart, immediate: true);
            _dungeonCamera.SetTarget(_playerView.transform);

            foreach (var enemy in result.Enemies)
            {
                var go = ViewFactory.CreateEnemy(enemy.EnemyType, enemy.Position);
                var view = go.AddComponent<EnemyView>();
                go.AddComponent<DamageFlash>();
                view.Init(enemy);
                _enemyViews[enemy.Id] = view;
            }

            foreach (var item in result.Items)
            {
                var go = ViewFactory.CreateItem(item.ItemType, item.Position);
                var view = go.AddComponent<ItemView>();
                view.Init(item);
                _itemViews[item] = view;
            }

            FogOfWar.Update(GameState.Map, GameState.Player.Position, 6);

            _fogRenderer.ApplyFog(GameState.Map, _dungeonView);

            _hud.UpdateFloor(GameState.CurrentFloor);
            _hud.UpdateLevel(GameState.Player.Level);
            _hud.UpdateHP(GameState.Player.Stats.CurrentHP, GameState.Player.Stats.MaxHP);
        }

        public void Tick()
        {
            if (GameState == null) return;
            if (_playerInput == null) return;

            if (_isPaused)
            {
                if (_playerInput.HasInput)
                {
                    var action = _playerInput.ConsumeAction();
                    if (action == InputAction.Pause)
                        _isPaused = false;
                }
                return;
            }

            if (!_playerInput.HasInput) return;

            var peeked = _playerInput.ConsumeAction();

            if (peeked == InputAction.Pause)
            {
                _isPaused = true;
                return;
            }

            if (peeked == InputAction.Inventory)
                return;

            var gameAction = ConvertToGameAction(peeked);
            if (gameAction == null) return;

            var turnResult = _turnSystem.ProcessPlayerAction(gameAction, GameState);

            FogOfWar.Update(GameState.Map, GameState.Player.Position, 6);
            _fogRenderer.ApplyFog(GameState.Map, _dungeonView);

            switch (turnResult)
            {
                case TurnResult.Continue:
                    break;

                case TurnResult.FloorChange:
                    GameState.CurrentFloor++;
                    GenerateFloor();
                    break;

                case TurnResult.GameOver:
                    Debug.Log($"Game Over! Floor={GameState.CurrentFloor} Turn={GameState.TurnNumber}");
                    _messageLog.AddMessage("You have been defeated...");
                    GameState = null;
                    _gameStateHolder.Current = null;
                    DelayedGameOverAsync(_cts.Token).Forget();
                    return;
            }

            _hud.UpdateHP(GameState.Player.Stats.CurrentHP, GameState.Player.Stats.MaxHP);
            _hud.UpdateLevel(GameState.Player.Level);
        }

        private IAction ConvertToGameAction(InputAction inputAction)
        {
            var player = GameState.Player;

            Direction? direction = inputAction switch
            {
                InputAction.MoveUp => Direction.Up,
                InputAction.MoveDown => Direction.Down,
                InputAction.MoveLeft => Direction.Left,
                InputAction.MoveRight => Direction.Right,
                _ => null
            };

            if (direction.HasValue)
            {
                var targetPos = player.Position + direction.Value.ToOffset();
                var entityAtTarget = GameState.GetEntityAt(targetPos);

                if (entityAtTarget != null && entityAtTarget != player)
                    return new AttackAction(player, entityAtTarget);

                return new MoveAction(player, direction.Value);
            }

            if (inputAction == InputAction.Wait)
                return new WaitAction();

            if (inputAction == InputAction.UseItem && player.Inventory.Count > 0)
                return new UseItemAction(player, 0);

            return null;
        }

        private void ClearViews()
        {
            foreach (var kv in _enemyViews)
            {
                if (kv.Value != null)
                    Object.Destroy(kv.Value.gameObject);
            }
            _enemyViews.Clear();

            foreach (var kv in _itemViews)
            {
                if (kv.Value != null)
                    Object.Destroy(kv.Value.gameObject);
            }
            _itemViews.Clear();
        }

        private async UniTaskVoid DelayedGameOverAsync(CancellationToken ct)
        {
            await UniTask.DelayFrame(2, cancellationToken: ct);
            OnGameOver?.Invoke();
        }

        private void OnEntityMoved(EntityMoved e)
        {
            if (e.Entity is Player && _playerView != null)
            {
                _playerView.SyncPosition(e.To);
            }
            else if (_enemyViews.TryGetValue(e.Entity.Id, out var view))
            {
                view.SyncPosition(e.To);
            }
        }

        private void OnEntityDamaged(EntityDamaged e)
        {
            if (e.Target is Player && _playerView != null)
            {
                var flash = _playerView.GetComponent<DamageFlash>();
                if (flash != null) flash.Flash();
            }
            else if (_enemyViews.TryGetValue(e.Target.Id, out var view))
            {
                var flash = view.GetComponent<DamageFlash>();
                if (flash != null) flash.Flash();
            }

            if (_cameraShake != null)
                _cameraShake.Shake();

            _messageLog.AddMessage($"{e.Attacker.Name} dealt {e.Damage} to {e.Target.Name}");
        }

        private void OnEntityDied(EntityDied e)
        {
            if (_enemyViews.TryGetValue(e.Entity.Id, out var view))
            {
                view.PlayDeathEffect();
                _enemyViews.Remove(e.Entity.Id);
            }

            _messageLog.AddMessage($"{e.Entity.Name} was defeated!");
        }

        private void OnItemPickedUp(ItemPickedUp e)
        {
            if (_itemViews.TryGetValue(e.Item, out var view))
            {
                view.PickUp();
                _itemViews.Remove(e.Item);
            }

            _messageLog.AddMessage($"Picked up {e.Item.ItemType.GetName()}");
        }

        private void OnPlayerLeveledUp(PlayerLeveledUp e)
        {
            Debug.Log($"Level Up! Now level {e.NewLevel}");
            _messageLog.AddMessage($"Level up! Now level {e.NewLevel}");
        }

        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
            UnsubscribeEvents();
            ClearViews();
        }
    }
}
