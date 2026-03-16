using System;
using App.Core.AI;
using App.Core.Dungeon;
using App.Core.Models;
using NUnit.Framework;

namespace App.Core.Tests
{
    [TestFixture]
    public sealed class PathfinderTest
    {
        private static DungeonMap CreateMap(string[] rows)
        {
            int height = rows.Length;
            int width = rows[0].Length;
            var map = new DungeonMap(width, height);

            for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
            {
                char c = rows[y][x];
                var tileType = c == '#' ? TileType.Wall : TileType.Floor;
                map[x, y] = new Tile(tileType);
            }

            return map;
        }

        private static readonly Func<Position, bool> NeverBlocked = _ => false;

        [Test]
        public void FindNextDirection_StraightPath_ReturnsCorrectDirection()
        {
            var map = CreateMap(new[]
            {
                ".....",
                ".....",
                ".....",
            });
            var current = new Position(0, 1);
            var target = new Position(4, 1);

            var actual = Pathfinder.FindNextDirection(current, target, map, NeverBlocked);

            Assert.That(actual, Is.EqualTo(Direction.Right));
        }

        [Test]
        public void FindNextDirection_LShapedCorridor_FindsPath()
        {
            var map = CreateMap(new[]
            {
                "#####",
                "#...#",
                "#.###",
                "#.#.#",
                "#...#",
                "#####",
            });
            var current = new Position(1, 1);
            var target = new Position(3, 4);

            var actual = Pathfinder.FindNextDirection(current, target, map, NeverBlocked);

            Assert.That(actual, Is.Not.Null);
        }

        [Test]
        public void FindNextDirection_UShapedWall_FindsDetour()
        {
            var map = CreateMap(new[]
            {
                ".....",
                ".###.",
                ".....",
            });
            var current = new Position(0, 1);
            var target = new Position(4, 1);

            var actual = Pathfinder.FindNextDirection(current, target, map, NeverBlocked);

            Assert.That(actual, Is.EqualTo(Direction.Down));
        }

        [Test]
        public void FindNextDirection_Unreachable_ReturnsNull()
        {
            var map = CreateMap(new[]
            {
                "..#..",
                "..#..",
                "..#..",
            });
            var current = new Position(0, 1);
            var target = new Position(4, 1);

            var actual = Pathfinder.FindNextDirection(current, target, map, NeverBlocked);

            Assert.That(actual, Is.Null);
        }

        [Test]
        public void FindNextDirection_AlreadyAtTarget_ReturnsNull()
        {
            var map = CreateMap(new[]
            {
                "...",
                "...",
            });
            var pos = new Position(1, 1);

            var actual = Pathfinder.FindNextDirection(pos, pos, map, NeverBlocked);

            Assert.That(actual, Is.Null);
        }

        [Test]
        public void FindNextDirection_BlockedByEntity_AvoidsEntity()
        {
            var map = CreateMap(new[]
            {
                "...",
                "...",
                "...",
            });
            var current = new Position(0, 1);
            var target = new Position(2, 1);
            var blockedPos = new Position(1, 1);

            var actual = Pathfinder.FindNextDirection(
                current, target, map,
                pos => pos == blockedPos);

            Assert.That(actual, Is.Not.Null);
            Assert.That(actual, Is.Not.EqualTo(Direction.Right));
        }

        [Test]
        public void FindNextDirection_TargetOnEntity_StillFindsPath()
        {
            var map = CreateMap(new[]
            {
                "...",
                "...",
            });
            var current = new Position(0, 0);
            var target = new Position(2, 0);

            var actual = Pathfinder.FindNextDirection(
                current, target, map,
                pos => pos == target);

            Assert.That(actual, Is.EqualTo(Direction.Right));
        }

        [Test]
        public void FindNextDirection_AdjacentTarget_ReturnsDirectDirection()
        {
            var map = CreateMap(new[]
            {
                "...",
                "...",
            });
            var current = new Position(1, 0);
            var target = new Position(2, 0);

            var actual = Pathfinder.FindNextDirection(current, target, map, NeverBlocked);

            Assert.That(actual, Is.EqualTo(Direction.Right));
        }
    }
}
