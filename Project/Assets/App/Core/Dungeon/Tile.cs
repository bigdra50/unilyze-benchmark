namespace App.Core.Dungeon
{
    public enum TileType
    {
        Wall,
        Floor,
        Stairs
    }

    public struct Tile
    {
        public TileType TileType;
        public bool IsVisible;
        public bool IsExplored;

        public Tile(TileType tileType, bool isVisible = false, bool isExplored = false)
        {
            TileType = tileType;
            IsVisible = isVisible;
            IsExplored = isExplored;
        }
    }
}
