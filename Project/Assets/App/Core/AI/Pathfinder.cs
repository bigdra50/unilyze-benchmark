using System;
using System.Collections.Generic;
using App.Core.Dungeon;
using App.Core.Models;

namespace App.Core.AI
{
    public static class Pathfinder
    {
        public static Direction? FindNextDirection(
            Position current, Position target, DungeonMap map, Func<Position, bool> isBlocked)
        {
            if (current == target)
                return null;

            var queue = new Queue<Position>();
            var cameFrom = new Dictionary<Position, Position>();

            queue.Enqueue(current);
            cameFrom[current] = current;

            while (queue.Count > 0)
            {
                var pos = queue.Dequeue();

                if (pos == target)
                    return ReconstructFirstStep(cameFrom, current, target);

                foreach (var neighbor in map.GetNeighbors(pos))
                {
                    if (cameFrom.ContainsKey(neighbor))
                        continue;

                    if (neighbor != target && isBlocked(neighbor))
                        continue;

                    cameFrom[neighbor] = pos;
                    queue.Enqueue(neighbor);
                }
            }

            return null;
        }

        private static Direction? ReconstructFirstStep(
            Dictionary<Position, Position> cameFrom, Position start, Position target)
        {
            var step = target;
            while (cameFrom[step] != start)
                step = cameFrom[step];

            var diff = new Position(step.X - start.X, step.Y - start.Y);

            if (diff.Equals(Direction.Up.ToOffset())) return Direction.Up;
            if (diff.Equals(Direction.Down.ToOffset())) return Direction.Down;
            if (diff.Equals(Direction.Left.ToOffset())) return Direction.Left;
            if (diff.Equals(Direction.Right.ToOffset())) return Direction.Right;

            return null;
        }
    }
}
