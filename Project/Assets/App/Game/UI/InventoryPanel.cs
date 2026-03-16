using System;
using App.Core.Models;
using UnityEngine.UIElements;

namespace App.Game.UI
{
    public sealed class InventoryPanel : IDisposable
    {
        private readonly VisualElement _overlay;
        private readonly ScrollView _itemScroll;
        private readonly Button _btnClose;

        public Action<int> OnItemSelected;
        public bool IsVisible => _overlay != null &&
                                 _overlay.style.display == DisplayStyle.Flex;

        public InventoryPanel(UIDocument document)
        {
            var root = document.rootVisualElement;

            _overlay = root.Q<VisualElement>("inventory-overlay");
            _itemScroll = root.Q<ScrollView>("item-scroll");
            _btnClose = root.Q<Button>("btn-close");

            _btnClose.clicked += Hide;

            Hide();
        }

        public void Show(Core.Inventory.Inventory inventory)
        {
            _itemScroll.Clear();

            for (var i = 0; i < inventory.Items.Count; i++)
            {
                var index = i;
                var item = inventory.Items[i];
                var btn = new Button(() => OnItemSelected?.Invoke(index));
                btn.AddToClassList("item-button");
                btn.text = $"[{index + 1}] {item.GetName()} - {item.GetDescription()}";
                _itemScroll.Add(btn);
            }

            _overlay.style.display = DisplayStyle.Flex;
        }

        public void Hide()
        {
            _overlay.style.display = DisplayStyle.None;
        }

        public void Dispose()
        {
            if (_btnClose != null) _btnClose.clicked -= Hide;
        }
    }
}
