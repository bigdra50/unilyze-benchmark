using System;
using System.Collections.Generic;
using App.Core.Dungeon;
using App.Core.Models;

namespace App.Game.Managers
{
    public sealed class DungeonManager
    {
        private const int MapWidth = 32;
        private const int MapHeight = 32;
        private const int BspDepth = 4;

        public FloorData GenerateFloor(int floorLevel, System.Random random)
        {
            var map = BspGenerator.Generate(MapWidth, MapHeight, BspDepth, random);
            var (enemies, items) = DungeonPopulator.Populate(map, floorLevel, random);

            Position playerStart = new Position(0, 0);
            if (map.Rooms.Count > 0)
                playerStart = map.Rooms[0].Center;

            FogOfWar.Update(map, playerStart, 6);

            return new FloorData(map, enemies, items, playerStart);
        }
    }

    public class FloorData
    {
        public DungeonMap Map { get; }
        public List<Enemy> Enemies { get; }
        public List<Item> Items { get; }
        public Position PlayerStart { get; }

        public FloorData(DungeonMap map, List<Enemy> enemies, List<Item> items, Position playerStart)
        {
            Map = map;
            Enemies = enemies;
            Items = items;
            PlayerStart = playerStart;
        }
    }
}
