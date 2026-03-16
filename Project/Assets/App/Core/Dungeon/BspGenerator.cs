using System;
using System.Collections.Generic;

namespace App.Core.Dungeon
{
    public static class BspGenerator
    {
        private const int MinNodeSize = 8;
        private const int RoomMargin = 2;

        public static DungeonMap Generate(int width, int height, int depth, Random random)
        {
            var map = new DungeonMap(width, height);
            var root = new BspNode(0, 0, width, height);

            SplitNode(root, depth, random);
            CreateRooms(root, map, random);
            ConnectRooms(root, map);
            PlaceStairs(map, random);

            return map;
        }

        private class BspNode
        {
            public int X { get; }
            public int Y { get; }
            public int W { get; }
            public int H { get; }
            public BspNode Left { get; set; }
            public BspNode Right { get; set; }
            public Room Room { get; set; }

            public BspNode(int x, int y, int w, int h)
            {
                X = x;
                Y = y;
                W = w;
                H = h;
            }

            public bool IsLeaf => Left == null && Right == null;

            public Room GetRoom()
            {
                if (Room != null) return Room;
                Room leftRoom = Left?.GetRoom();
                Room rightRoom = Right?.GetRoom();
                return leftRoom ?? rightRoom;
            }
        }

        private static void SplitNode(BspNode node, int depth, Random random)
        {
            if (depth <= 0) return;
            if (node.W < MinNodeSize * 2 && node.H < MinNodeSize * 2) return;

            bool splitHorizontal;
            if (node.W < MinNodeSize * 2)
                splitHorizontal = true;
            else if (node.H < MinNodeSize * 2)
                splitHorizontal = false;
            else
                splitHorizontal = random.Next(2) == 0;

            if (splitHorizontal)
            {
                if (node.H < MinNodeSize * 2) return;
                int split = random.Next(MinNodeSize, node.H - MinNodeSize + 1);
                node.Left = new BspNode(node.X, node.Y, node.W, split);
                node.Right = new BspNode(node.X, node.Y + split, node.W, node.H - split);
            }
            else
            {
                if (node.W < MinNodeSize * 2) return;
                int split = random.Next(MinNodeSize, node.W - MinNodeSize + 1);
                node.Left = new BspNode(node.X, node.Y, split, node.H);
                node.Right = new BspNode(node.X + split, node.Y, node.W - split, node.H);
            }

            SplitNode(node.Left, depth - 1, random);
            SplitNode(node.Right, depth - 1, random);
        }

        private static void CreateRooms(BspNode node, DungeonMap map, Random random)
        {
            if (node.IsLeaf)
            {
                int roomW = random.Next(
                    Math.Max(3, node.W - RoomMargin * 2 - 2),
                    Math.Max(4, node.W - RoomMargin * 2) + 1);
                int roomH = random.Next(
                    Math.Max(3, node.H - RoomMargin * 2 - 2),
                    Math.Max(4, node.H - RoomMargin * 2) + 1);
                int roomX = node.X + RoomMargin + random.Next(Math.Max(1, node.W - RoomMargin * 2 - roomW));
                int roomY = node.Y + RoomMargin + random.Next(Math.Max(1, node.H - RoomMargin * 2 - roomH));

                var room = new Room(roomX, roomY, roomW, roomH);
                node.Room = room;
                map.AddRoom(room);

                for (int x = room.X; x < room.X + room.Width; x++)
                for (int y = room.Y; y < room.Y + room.Height; y++)
                {
                    if (map.IsInBounds(new Models.Position(x, y)))
                        map[x, y] = new Tile(TileType.Floor, false, false);
                }

                return;
            }

            if (node.Left != null) CreateRooms(node.Left, map, random);
            if (node.Right != null) CreateRooms(node.Right, map, random);
        }

        private static void ConnectRooms(BspNode node, DungeonMap map)
        {
            if (node.IsLeaf) return;
            if (node.Left == null || node.Right == null) return;

            ConnectRooms(node.Left, map);
            ConnectRooms(node.Right, map);

            Room roomA = node.Left.GetRoom();
            Room roomB = node.Right.GetRoom();

            if (roomA == null || roomB == null) return;

            var centerA = roomA.Center;
            var centerB = roomB.Center;

            CarveCorridor(map, centerA.X, centerA.Y, centerB.X, centerA.Y);
            CarveCorridor(map, centerB.X, centerA.Y, centerB.X, centerB.Y);
        }

        private static void CarveCorridor(DungeonMap map, int x1, int y1, int x2, int y2)
        {
            int dx = x1 < x2 ? 1 : -1;
            int dy = y1 < y2 ? 1 : -1;

            int x = x1;
            while (x != x2 + dx)
            {
                var pos = new Models.Position(x, y1);
                if (map.IsInBounds(pos) && map[pos].TileType == TileType.Wall)
                    map[pos] = new Tile(TileType.Floor, false, false);
                x += dx;
            }

            int y = y1;
            while (y != y2 + dy)
            {
                var pos = new Models.Position(x2, y);
                if (map.IsInBounds(pos) && map[pos].TileType == TileType.Wall)
                    map[pos] = new Tile(TileType.Floor, false, false);
                y += dy;
            }
        }

        private static void PlaceStairs(DungeonMap map, Random random)
        {
            var rooms = map.Rooms;
            if (rooms.Count == 0) return;

            var room = rooms[rooms.Count - 1];
            var center = room.Center;
            map[center] = new Tile(TileType.Stairs, false, false);
        }
    }
}
