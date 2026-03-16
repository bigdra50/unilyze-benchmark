using System;
using App.Core.Models;
using App.Core.Turn;

namespace App.Core.AI
{
    public class ChaseStrategy : IAIStrategy
    {
        private static readonly Direction[] Directions =
            { Direction.Up, Direction.Down, Direction.Left, Direction.Right };

        public IAction DecideAction(Enemy enemy, GameState state)
        {
            var playerPos = state.Player.Position;
            var enemyPos = enemy.Position;

            var dx = playerPos.X - enemyPos.X;
            var dy = playerPos.Y - enemyPos.Y;

            if (Math.Abs(dx) + Math.Abs(dy) == 1)
                return new AttackAction(enemy, state.Player);

            Direction bestDir = default;
            int bestDist = int.MaxValue;

            foreach (var dir in Directions)
            {
                var offset = dir.ToOffset();
                var newPos = enemyPos + offset;

                if (!state.Map.IsWalkable(newPos)) continue;
                if (state.GetEntityAt(newPos) != null) continue;

                var dist = Math.Abs(playerPos.X - newPos.X) + Math.Abs(playerPos.Y - newPos.Y);
                if (dist < bestDist)
                {
                    bestDist = dist;
                    bestDir = dir;
                }
            }

            if (bestDist < int.MaxValue)
                return new MoveAction(enemy, bestDir);

            return new WaitAction();
        }
    }
}
