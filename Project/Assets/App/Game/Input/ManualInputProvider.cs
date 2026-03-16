using UnityEngine.InputSystem;
using VContainer.Unity;
using GameInputAction = App.Core.Turn.InputAction;

namespace App.Game.Input
{
    public sealed class ManualInputProvider : IInputProvider, ITickable
    {
        private readonly InputState _state = new();

        public bool HasInput => _state.CurrentAction != GameInputAction.None;

        public void Tick()
        {
            if (_state.CurrentAction != GameInputAction.None) return;

            var kb = Keyboard.current;
            if (kb == null) return;

            if (kb.wKey.wasPressedThisFrame || kb.upArrowKey.wasPressedThisFrame)
                _state.CurrentAction = GameInputAction.MoveUp;
            else if (kb.sKey.wasPressedThisFrame || kb.downArrowKey.wasPressedThisFrame)
                _state.CurrentAction = GameInputAction.MoveDown;
            else if (kb.aKey.wasPressedThisFrame || kb.leftArrowKey.wasPressedThisFrame)
                _state.CurrentAction = GameInputAction.MoveLeft;
            else if (kb.dKey.wasPressedThisFrame || kb.rightArrowKey.wasPressedThisFrame)
                _state.CurrentAction = GameInputAction.MoveRight;
            else if (kb.spaceKey.wasPressedThisFrame)
                _state.CurrentAction = GameInputAction.Wait;
            else if (kb.eKey.wasPressedThisFrame)
                _state.CurrentAction = GameInputAction.Inventory;
            else if (kb.escapeKey.wasPressedThisFrame)
                _state.CurrentAction = GameInputAction.Pause;
        }

        public GameInputAction ConsumeAction()
        {
            var action = _state.CurrentAction;
            _state.CurrentAction = GameInputAction.None;
            return action;
        }
    }
}
