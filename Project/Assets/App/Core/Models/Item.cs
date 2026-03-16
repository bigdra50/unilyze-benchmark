namespace App.Core.Models
{
    public class Item
    {
        public ItemType ItemType { get; }
        public Position Position { get; set; }

        public Item(ItemType itemType, Position position)
        {
            ItemType = itemType;
            Position = position;
        }
    }
}
