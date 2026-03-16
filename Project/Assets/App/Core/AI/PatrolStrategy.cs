using System;
using App.Core.Models;
using App.Core.Turn;

namespace App.Core.AI
{
    public class PatrolStrategy : IAIStrategy
    {
        private static readonly Direction[] Directions =
            { Direction.Up, Direction.Down, Direction.Left, Direction.Right };

        private readonly Random _random = new();

        public IAction DecideAction(Enemy enemy, GameState state)
        {
            for (int attempt = 0; attempt < 3; attempt++)
            {
                var dir = Directions[_random.Next(Directions.Length)];
                var offset = dir.ToOffset();
                var newPos = enemy.Position + offset;

                if (state.Map.IsWalkable(newPos) && state.GetEntityAt(newPos) == null)
                    return new MoveAction(enemy, dir);
            }

            return new WaitAction();
        }
    }
}
