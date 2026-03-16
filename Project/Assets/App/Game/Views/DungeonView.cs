using System;
using System.Collections.Generic;
using App.Core.Dungeon;
using App.Core.Models;
using UnityEngine;
using Object = UnityEngine.Object;

namespace App.Game.Views
{
    public sealed class DungeonView : IDisposable
    {
        private readonly Transform _root;
        private readonly Dictionary<Position, TileView> _tileViews = new();

        public DungeonView()
        {
            _root = new GameObject("DungeonView").transform;
        }

        public void Initialize(DungeonMap map)
        {
            Clear();

            for (int x = 0; x < map.Width; x++)
            {
                for (int y = 0; y < map.Height; y++)
                {
                    var pos = new Position(x, y);
                    var tile = map.GetTile(pos);

                    if (tile.TileType == TileType.Wall && !HasAdjacentFloor(map, pos))
                        continue;

                    GameObject tileGo;

                    switch (tile.TileType)
                    {
                        case TileType.Floor:
                            tileGo = ViewFactory.CreateFloorTile(pos);
                            break;
                        case TileType.Stairs:
                            tileGo = ViewFactory.CreateStairsTile(pos);
                            break;
                        default:
                            tileGo = ViewFactory.CreateWallTile(pos);
                            break;
                    }

                    tileGo.transform.SetParent(_root);
                    var tileView = tileGo.AddComponent<TileView>();
                    tileView.Init(pos, tile.TileType);
                    tileView.SetVisible(false);
                    _tileViews[pos] = tileView;
                }
            }
        }

        public void UpdateVisibility(DungeonMap map)
        {
            foreach (var kvp in _tileViews)
            {
                var pos = kvp.Key;
                var view = kvp.Value;
                var tile = map.GetTile(pos);

                if (tile.IsVisible)
                {
                    view.SetVisible(true);
                    view.ResetColor();
                }
                else if (tile.IsExplored)
                {
                    view.SetExplored(true);
                }
                else
                {
                    view.SetVisible(false);
                }
            }
        }

        private static bool HasAdjacentFloor(DungeonMap map, Position pos)
        {
            for (int dx = -1; dx <= 1; dx++)
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;
                var adj = new Position(pos.X + dx, pos.Y + dy);
                if (!map.IsInBounds(adj)) continue;
                var t = map.GetTile(adj);
                if (t.TileType == TileType.Floor || t.TileType == TileType.Stairs)
                    return true;
            }
            return false;
        }

        public void Clear()
        {
            foreach (var kvp in _tileViews)
            {
                if (kvp.Value != null)
                    Object.Destroy(kvp.Value.gameObject);
            }

            _tileViews.Clear();
        }

        public void Dispose()
        {
            Clear();
            if (_root != null)
                Object.Destroy(_root.gameObject);
        }
    }
}
