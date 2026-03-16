using App.Core.AI;
using App.Core.Dungeon;
using App.Core.Events;
using App.Core.Models;

namespace App.Core.Turn
{
    public enum TurnResult
    {
        Continue,
        FloorChange,
        GameOver
    }

    public class TurnSystem
    {
        private readonly AIDecisionMaker _aiDecisionMaker = new();

        public TurnResult ProcessPlayerAction(IAction action, GameState state)
        {
            var result = action.Execute(state);

            if (!state.Player.Stats.IsAlive)
                return TurnResult.GameOver;

            if (result.Success && action is MoveAction)
            {
                if (state.Map.GetTile(state.Player.Position).TileType == TileType.Stairs)
                    return TurnResult.FloorChange;
            }

            ProcessEnemyActions(state);

            if (!state.Player.Stats.IsAlive)
                return TurnResult.GameOver;

            state.TurnNumber++;
            state.EventBus.Publish(new TurnAdvanced(state.TurnNumber));

            return TurnResult.Continue;
        }

        private void ProcessEnemyActions(GameState state)
        {
            for (int i = state.Enemies.Count - 1; i >= 0; i--)
            {
                if (i >= state.Enemies.Count) continue;

                var enemy = state.Enemies[i];
                var enemyAction = _aiDecisionMaker.DecideAction(enemy, state);
                enemyAction.Execute(state);
            }
        }
    }
}
