using App.Core.Dungeon;
using App.Core.Models;
using UnityEngine;

namespace App.Game.Views
{
    public class TileView : MonoBehaviour
    {
        private Renderer _renderer;
        private Color _baseColor;
        private TileType _tileType;

        public Position Position { get; private set; }

        public void Init(Position position, TileType tileType)
        {
            Position = position;
            _tileType = tileType;
            _renderer = GetComponent<Renderer>();
            if (_renderer != null)
            {
                _baseColor = _renderer.material.color;
            }
        }

        public void SetVisible(bool visible)
        {
            if (_renderer != null)
            {
                _renderer.enabled = visible;
            }
        }

        public void SetExplored(bool explored)
        {
            if (_renderer == null) return;

            _renderer.enabled = explored;
            if (explored)
            {
                _renderer.material.color = _baseColor * 0.5f;
            }
        }

        public void ResetColor()
        {
            if (_renderer != null)
            {
                _renderer.material.color = _baseColor;
            }
        }
    }
}
