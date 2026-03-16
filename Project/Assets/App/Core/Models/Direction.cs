namespace App.Core.Models
{
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    public static class DirectionExtensions
    {
        public static Position ToOffset(this Direction direction)
        {
            switch (direction)
            {
                case Direction.Up: return Position.Up;
                case Direction.Down: return Position.Down;
                case Direction.Left: return Position.Left;
                case Direction.Right: return Position.Right;
                default: return new Position(0, 0);
            }
        }
    }
}
