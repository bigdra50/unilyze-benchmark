using App.Core.Turn;

namespace App.Game.Input
{
    public interface IInputProvider
    {
        bool HasInput { get; }
        InputAction ConsumeAction();
    }
}
