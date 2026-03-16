using System;
using App.Core.Dungeon;
using UnityEditor;
using UnityEngine;

namespace App.Editor
{
    public class DungeonPreview : EditorWindow
    {
        private int _width = 50;
        private int _height = 50;
        private int _bspDepth = 4;

        private DungeonMap _map;

        [MenuItem("Tools/Dungeon Preview")]
        private static void Open()
        {
            GetWindow<DungeonPreview>("Dungeon Preview");
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Dungeon Generator Settings", EditorStyles.boldLabel);

            _width = EditorGUILayout.IntSlider("Width", _width, 16, 128);
            _height = EditorGUILayout.IntSlider("Height", _height, 16, 128);
            _bspDepth = EditorGUILayout.IntSlider("BSP Depth", _bspDepth, 1, 8);

            if (GUILayout.Button("Generate"))
            {
                _map = BspGenerator.Generate(_width, _height, _bspDepth, new System.Random());
                Repaint();
            }

            if (_map == null) return;

            float offsetY = GUILayoutUtility.GetLastRect().yMax + 8f;
            float availableWidth = position.width;
            float cellSize = availableWidth / _map.Width;

            for (int y = 0; y < _map.Height; y++)
            {
                for (int x = 0; x < _map.Width; x++)
                {
                    var rect = new Rect(x * cellSize, y * cellSize + offsetY, cellSize, cellSize);
                    var tile = _map[x, y];

                    Color color = tile.TileType switch
                    {
                        TileType.Wall => Color.black,
                        TileType.Floor => Color.gray,
                        TileType.Stairs => Color.yellow,
                        _ => Color.magenta
                    };

                    EditorGUI.DrawRect(rect, color);
                }
            }
        }
    }
}
