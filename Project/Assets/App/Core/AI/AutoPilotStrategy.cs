using System;
using System.Collections.Generic;
using System.Linq;
using App.Core.Dungeon;
using App.Core.Models;
using App.Core.Turn;

namespace App.Core.AI
{
    public class AutoPilotStrategy
    {
        private static readonly Direction[] AllDirections =
            { Direction.Up, Direction.Down, Direction.Left, Direction.Right };

        private readonly Random _random = new();

        public IAction DecideAction(GameState state)
        {
            var player = state.Player;
            var pos = player.Position;

            // 1. HP < 40% かつ HealthPotion を持っている → UseItemAction
            if (player.Stats.CurrentHP < player.Stats.MaxHP * 0.4)
            {
                int potionIndex = FindHealthPotionIndex(player);
                if (potionIndex >= 0)
                    return new UseItemAction(player, potionIndex);
            }

            // 2. 隣接4方向に敵がいる → 最もHP低い敵を攻撃
            Enemy adjacentTarget = null;
            foreach (var dir in AllDirections)
            {
                var adjPos = pos + dir.ToOffset();
                var entity = state.GetEntityAt(adjPos);
                if (entity is Enemy enemy)
                {
                    if (adjacentTarget == null ||
                        enemy.Stats.CurrentHP < adjacentTarget.Stats.CurrentHP)
                    {
                        adjacentTarget = enemy;
                    }
                }
            }

            if (adjacentTarget != null)
                return new AttackAction(player, adjacentTarget);

            // 3. 視界内（IsVisible かつ距離8以内）に敵がいる → 最も近い敵の方向へ移動
            var visibleEnemies = state.Enemies
                .Where(e => e.Stats.IsAlive &&
                            IsVisibleAndInRange(state.Map, e.Position, pos, 8))
                .OrderBy(e => pos.ManhattanDistance(e.Position))
                .ToList();

            if (visibleEnemies.Count > 0)
            {
                var moveDir = PickBestDirection(pos, visibleEnemies[0].Position, state);
                if (moveDir.HasValue)
                    return new MoveAction(player, moveDir.Value);
            }

            // 4. 視界内にアイテムがある → 最も近いアイテムの方向へ移動
            var visibleItems = state.Items
                .Where(i => IsVisibleAndInRange(state.Map, i.Position, pos, 8))
                .OrderBy(i => pos.ManhattanDistance(i.Position))
                .ToList();

            if (visibleItems.Count > 0)
            {
                var moveDir = PickBestDirection(pos, visibleItems[0].Position, state);
                if (moveDir.HasValue)
                    return new MoveAction(player, moveDir.Value);
            }

            // 5. マップにStairsがあり、IsExplored → 階段方向へ移動
            var stairsPos = state.Map.FindStairs();
            if (stairsPos.HasValue && state.Map.GetTile(stairsPos.Value).IsExplored)
            {
                var moveDir = PickBestDirection(pos, stairsPos.Value, state);
                if (moveDir.HasValue)
                    return new MoveAction(player, moveDir.Value);
            }

            // 6. 未探索タイルがある方向を優先して移動
            var exploreDirs = new List<Direction>();
            var walkableDirs = new List<Direction>();

            foreach (var dir in AllDirections)
            {
                var nextPos = pos + dir.ToOffset();
                if (!state.Map.IsWalkable(nextPos) || state.GetEntityAt(nextPos) != null)
                    continue;

                walkableDirs.Add(dir);

                // 移動先の先に未探索タイルがあるか確認
                var beyondPos = nextPos + dir.ToOffset();
                if (state.Map.IsInBounds(beyondPos) &&
                    !state.Map.GetTile(beyondPos).IsExplored)
                {
                    exploreDirs.Add(dir);
                }
            }

            if (exploreDirs.Count > 0)
            {
                var dir = exploreDirs[_random.Next(exploreDirs.Count)];
                return new MoveAction(player, dir);
            }

            // 7. Walkableな方向にランダム移動
            if (walkableDirs.Count > 0)
            {
                var dir = walkableDirs[_random.Next(walkableDirs.Count)];
                return new MoveAction(player, dir);
            }

            // 8. どこにも動けない → Wait
            return new WaitAction();
        }

        /// <summary>
        /// InputAction を直接返す。AutoPilotProvider から呼ばれる。
        /// GameManager.ConvertToGameAction で IAction に変換される。
        /// </summary>
        public InputAction DecideInputAction(GameState state)
        {
            var player = state.Player;
            var pos = player.Position;

            // HP低下時にポーション所持 → UseItem
            if (player.Stats.CurrentHP < player.Stats.MaxHP * 0.4)
            {
                int potionIndex = FindHealthPotionIndex(player);
                if (potionIndex >= 0)
                    return InputAction.UseItem;
            }

            // 隣接敵 → その方向へ移動入力（ConvertToGameAction が AttackAction に変換する）
            foreach (var dir in AllDirections)
            {
                var adjPos = pos + dir.ToOffset();
                var entity = state.GetEntityAt(adjPos);
                if (entity is Enemy)
                    return DirectionToInput(dir);
            }

            // 視界内の敵方向へ
            var nearestEnemy = FindNearestVisible(
                state.Enemies, e => e.Stats.IsAlive, e => e.Position, pos, state.Map, 8);
            if (nearestEnemy != null)
            {
                var moveDir = PickBestDirection(pos, nearestEnemy.Position, state);
                if (moveDir.HasValue)
                    return DirectionToInput(moveDir.Value);
            }

            // 視界内アイテム方向へ
            var nearestItem = FindNearestVisible(
                state.Items, _ => true, i => i.Position, pos, state.Map, 8);
            if (nearestItem != null)
            {
                var moveDir = PickBestDirection(pos, nearestItem.Position, state);
                if (moveDir.HasValue)
                    return DirectionToInput(moveDir.Value);
            }

            // 階段方向へ（見つけている場合。敵がいなくなったら積極的に階段へ）
            var stairsPos = state.Map.FindStairs();
            if (stairsPos.HasValue)
            {
                var stairsTile = state.Map.GetTile(stairsPos.Value);
                if (stairsTile.IsExplored || stairsTile.IsVisible)
                {
                    var moveDir = PickBestDirection(pos, stairsPos.Value, state);
                    if (moveDir.HasValue)
                        return DirectionToInput(moveDir.Value);
                }
            }

            // 未探索方向
            var exploreDirs = new List<Direction>();
            var walkableDirs = new List<Direction>();

            foreach (var dir in AllDirections)
            {
                var nextPos = pos + dir.ToOffset();
                if (!state.Map.IsWalkable(nextPos) || state.GetEntityAt(nextPos) != null)
                    continue;

                walkableDirs.Add(dir);

                var beyondPos = nextPos + dir.ToOffset();
                if (state.Map.IsInBounds(beyondPos) &&
                    !state.Map.GetTile(beyondPos).IsExplored)
                {
                    exploreDirs.Add(dir);
                }
            }

            if (exploreDirs.Count > 0)
                return DirectionToInput(exploreDirs[_random.Next(exploreDirs.Count)]);

            if (walkableDirs.Count > 0)
                return DirectionToInput(walkableDirs[_random.Next(walkableDirs.Count)]);

            return InputAction.Wait;
        }

        private static InputAction DirectionToInput(Direction dir)
        {
            switch (dir)
            {
                case Direction.Up: return InputAction.MoveUp;
                case Direction.Down: return InputAction.MoveDown;
                case Direction.Left: return InputAction.MoveLeft;
                case Direction.Right: return InputAction.MoveRight;
                default: return InputAction.Wait;
            }
        }

        private static T FindNearestVisible<T>(
            IEnumerable<T> collection,
            Func<T, bool> filter,
            Func<T, Position> posSelector,
            Position fromPos,
            DungeonMap map,
            int range) where T : class
        {
            T nearest = null;
            int nearestDist = int.MaxValue;

            foreach (var item in collection)
            {
                if (!filter(item)) continue;
                var itemPos = posSelector(item);
                if (!IsVisibleAndInRange(map, itemPos, fromPos, range)) continue;

                int dist = fromPos.ManhattanDistance(itemPos);
                if (dist < nearestDist)
                {
                    nearestDist = dist;
                    nearest = item;
                }
            }

            return nearest;
        }

        private static int FindHealthPotionIndex(Player player)
        {
            var items = player.Inventory.Items;
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] == ItemType.HealthPotion)
                    return i;
            }

            return -1;
        }

        private static bool IsVisibleAndInRange(
            DungeonMap map, Position targetPos, Position fromPos, int range)
        {
            if (!map.IsInBounds(targetPos))
                return false;

            var tile = map.GetTile(targetPos);
            return tile.IsVisible && fromPos.ManhattanDistance(targetPos) <= range;
        }

        private static Direction? PickBestDirection(
            Position current, Position target, GameState state)
        {
            return Pathfinder.FindNextDirection(
                current, target, state.Map,
                pos => state.GetEntityAt(pos) != null);
        }
    }
}
