using System;
using System.Collections.Generic;
using App.Core.Models;

namespace App.Core.Dungeon
{
    public static class DungeonPopulator
    {
        public static (List<Enemy> Enemies, List<Item> Items) Populate(
            DungeonMap map, int floorLevel, Random random)
        {
            var enemies = new List<Enemy>();
            var items = new List<Item>();
            var rooms = map.Rooms;

            if (rooms.Count == 0)
                return (enemies, items);

            for (int i = 1; i < rooms.Count; i++)
            {
                var room = rooms[i];
                var floorTiles = GetFloorTiles(map, room);
                if (floorTiles.Count == 0) continue;

                int enemyCount = random.Next(0, 4);
                for (int e = 0; e < enemyCount && floorTiles.Count > 0; e++)
                {
                    int tileIndex = random.Next(floorTiles.Count);
                    var pos = floorTiles[tileIndex];
                    floorTiles.RemoveAt(tileIndex);

                    var enemyType = PickEnemyType(floorLevel, random);
                    enemies.Add(new Enemy(enemyType, pos));
                }

                if (floorTiles.Count > 0 && random.NextDouble() < 0.3)
                {
                    int tileIndex = random.Next(floorTiles.Count);
                    var pos = floorTiles[tileIndex];
                    var itemType = PickItemType(random);
                    items.Add(new Item(itemType, pos));
                }
            }

            return (enemies, items);
        }

        private static List<Position> GetFloorTiles(DungeonMap map, Room room)
        {
            var tiles = new List<Position>();
            for (int x = room.X; x < room.X + room.Width; x++)
            for (int y = room.Y; y < room.Y + room.Height; y++)
            {
                var pos = new Position(x, y);
                if (map.IsInBounds(pos) && map[pos].TileType == TileType.Floor)
                    tiles.Add(pos);
            }
            return tiles;
        }

        private static EnemyType PickEnemyType(int floorLevel, Random random)
        {
            if (floorLevel >= 7)
            {
                int roll = random.Next(100);
                if (roll < 30) return EnemyType.Dragon;
                if (roll < 70) return EnemyType.Goblin;
                return EnemyType.Slime;
            }

            if (floorLevel >= 4)
            {
                int roll = random.Next(100);
                if (roll < 50) return EnemyType.Goblin;
                return EnemyType.Slime;
            }

            return EnemyType.Slime;
        }

        private static ItemType PickItemType(Random random)
        {
            var types = Enum.GetValues(typeof(ItemType));
            return (ItemType)types.GetValue(random.Next(types.Length));
        }
    }
}
