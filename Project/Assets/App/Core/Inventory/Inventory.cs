using System;
using System.Collections.Generic;
using App.Core.Models;

namespace App.Core.Inventory
{
    public class Inventory
    {
        public const int MaxSize = 10;

        private readonly List<ItemType> _items = new();

        public IReadOnlyList<ItemType> Items => _items;
        public int Count => _items.Count;
        public bool IsFull => _items.Count >= MaxSize;

        public bool Add(ItemType item)
        {
            if (IsFull) return false;
            _items.Add(item);
            return true;
        }

        public ItemType RemoveAt(int index)
        {
            if (index < 0 || index >= _items.Count)
                throw new ArgumentOutOfRangeException(nameof(index));

            var item = _items[index];
            _items.RemoveAt(index);
            return item;
        }
    }
}
