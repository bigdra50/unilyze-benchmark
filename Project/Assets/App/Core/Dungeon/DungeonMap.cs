using System.Collections.Generic;
using App.Core.Models;

namespace App.Core.Dungeon
{
    public class DungeonMap
    {
        private readonly Tile[,] _tiles;
        private readonly List<Room> _rooms;

        public int Width { get; }
        public int Height { get; }
        public IReadOnlyList<Room> Rooms => _rooms;

        public DungeonMap(int width, int height)
        {
            Width = width;
            Height = height;
            _tiles = new Tile[width, height];
            _rooms = new List<Room>();

            for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                _tiles[x, y] = new Tile(TileType.Wall, false, false);
        }

        public Tile this[int x, int y]
        {
            get => _tiles[x, y];
            set => _tiles[x, y] = value;
        }

        public Tile this[Position pos]
        {
            get => _tiles[pos.X, pos.Y];
            set => _tiles[pos.X, pos.Y] = value;
        }

        public bool IsInBounds(Position pos)
        {
            return pos.X >= 0 && pos.X < Width
                && pos.Y >= 0 && pos.Y < Height;
        }

        public bool IsWalkable(Position pos)
        {
            if (!IsInBounds(pos)) return false;
            var type = _tiles[pos.X, pos.Y].TileType;
            return type == TileType.Floor || type == TileType.Stairs;
        }

        public Tile GetTile(Position pos) => _tiles[pos.X, pos.Y];

        public void SetTile(Position pos, Tile tile) => _tiles[pos.X, pos.Y] = tile;

        public void AddRoom(Room room) => _rooms.Add(room);

        public bool IsStairs(Position pos)
        {
            return IsInBounds(pos) && _tiles[pos.X, pos.Y].TileType == TileType.Stairs;
        }

        public Position? FindStairs()
        {
            for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
            {
                if (_tiles[x, y].TileType == TileType.Stairs)
                    return new Position(x, y);
            }
            return null;
        }

        public List<Position> GetNeighbors(Position pos)
        {
            var neighbors = new List<Position>(4);
            Position[] offsets =
            {
                new(0, -1),
                new(0, 1),
                new(-1, 0),
                new(1, 0)
            };

            foreach (var offset in offsets)
            {
                var next = pos + offset;
                if (IsWalkable(next))
                    neighbors.Add(next);
            }

            return neighbors;
        }
    }
}
