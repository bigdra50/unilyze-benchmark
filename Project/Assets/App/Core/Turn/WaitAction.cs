namespace App.Core.Turn
{
    public class WaitAction : IAction
    {
        public ActionResult Execute(GameState state)
        {
            return new ActionResult(true, "Waiting...");
        }
    }
}
