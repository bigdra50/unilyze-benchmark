using App.Core.Models;

namespace App.Core.Inventory
{
    public enum SlotType
    {
        Weapon,
        Armor,
        Accessory
    }

    public record EquipmentSlot(SlotType SlotType, ItemType? Equipped);
}
