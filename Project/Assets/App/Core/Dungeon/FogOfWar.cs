using System;
using App.Core.Models;

namespace App.Core.Dungeon
{
    public static class FogOfWar
    {
        public static void Update(DungeonMap map, Position center, int radius)
        {
            for (int x = 0; x < map.Width; x++)
            for (int y = 0; y < map.Height; y++)
            {
                var tile = map[x, y];
                tile.IsVisible = false;
                map[x, y] = tile;
            }

            for (int angle = 0; angle < 360; angle++)
            {
                double rad = angle * Math.PI / 180.0;
                double dx = Math.Cos(rad);
                double dy = Math.Sin(rad);

                double fx = center.X + 0.5;
                double fy = center.Y + 0.5;

                for (int step = 0; step <= radius; step++)
                {
                    int tx = (int)Math.Floor(fx);
                    int ty = (int)Math.Floor(fy);
                    var pos = new Position(tx, ty);

                    if (!map.IsInBounds(pos)) break;

                    var tile = map[pos];
                    tile.IsVisible = true;
                    tile.IsExplored = true;
                    map[pos] = tile;

                    if (tile.TileType == TileType.Wall) break;

                    fx += dx;
                    fy += dy;
                }
            }
        }
    }
}
