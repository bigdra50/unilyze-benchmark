using App.Core.AI;
using App.Core.Turn;
using App.Game.Bootstrap;
using UnityEngine;
using VContainer.Unity;

namespace App.Game.Input
{
    public sealed class AutoPilotProvider : IInputProvider, ITickable
    {
        private readonly GameStateHolder _gameStateHolder;
        private readonly AutoPilotStrategy _strategy = new();
        private InputAction _pendingAction = InputAction.None;
        private const float ActionInterval = 0.15f;
        private float _timer;

        public AutoPilotProvider(GameStateHolder gameStateHolder)
        {
            _gameStateHolder = gameStateHolder;
        }

        public bool HasInput => _pendingAction != InputAction.None;

        public void Tick()
        {
            if (_pendingAction != InputAction.None) return;
            if (_gameStateHolder?.Current == null) return;

            _timer += Time.deltaTime;
            if (_timer < ActionInterval) return;
            _timer = 0f;

            _pendingAction = _strategy.DecideInputAction(_gameStateHolder.Current);
        }

        public InputAction ConsumeAction()
        {
            var a = _pendingAction;
            _pendingAction = InputAction.None;
            return a;
        }
    }
}
