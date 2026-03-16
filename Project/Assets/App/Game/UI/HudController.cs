using UnityEngine;
using UnityEngine.UIElements;

namespace App.Game.UI
{
    public sealed class HudController
    {
        private readonly UIDocument _document;
        private Label _floorLabel;
        private Label _levelLabel;
        private VisualElement _hpBarFill;
        private Label _hpLabel;
        private bool _bound;

        public HudController(UIDocument document)
        {
            _document = document;
        }

        private void EnsureBound()
        {
            if (_bound) return;

            var root = _document != null ? _document.rootVisualElement : null;
            if (root == null) return;

            _floorLabel = root.Q<Label>("floor-label");
            _levelLabel = root.Q<Label>("level-label");
            _hpBarFill = root.Q<VisualElement>("hp-bar-fill");
            _hpLabel = root.Q<Label>("hp-label");
            _bound = true;
        }

        public void UpdateHP(int current, int max)
        {
            EnsureBound();
            if (!_bound) return;

            if (max <= 0) max = 1;
            var ratio = Mathf.Clamp01((float)current / max) * 100f;
            _hpBarFill.style.width = new Length(ratio, LengthUnit.Percent);
            _hpLabel.text = $"HP: {current}/{max}";
        }

        public void UpdateFloor(int floor)
        {
            EnsureBound();
            if (!_bound) return;

            _floorLabel.text = $"Floor {floor}";
        }

        public void UpdateLevel(int level)
        {
            EnsureBound();
            if (!_bound) return;

            _levelLabel.text = $"Lv.{level}";
        }
    }
}
