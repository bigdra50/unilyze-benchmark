namespace App.Core.Turn
{
    public interface IAction
    {
        ActionResult Execute(GameState state);
    }

    public record ActionResult(bool Success, string Message);
}
