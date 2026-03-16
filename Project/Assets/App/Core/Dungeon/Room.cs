using App.Core.Models;

namespace App.Core.Dungeon
{
    public class Room
    {
        public int X { get; }
        public int Y { get; }
        public int Width { get; }
        public int Height { get; }

        public Position Center => new(X + Width / 2, Y + Height / 2);

        public Room(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public bool Contains(Position pos)
        {
            return pos.X >= X && pos.X < X + Width
                && pos.Y >= Y && pos.Y < Y + Height;
        }

        public bool Intersects(Room other)
        {
            return X - 1 < other.X + other.Width
                && X + Width + 1 > other.X
                && Y - 1 < other.Y + other.Height
                && Y + Height + 1 > other.Y;
        }
    }
}
