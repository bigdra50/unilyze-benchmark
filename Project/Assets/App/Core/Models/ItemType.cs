namespace App.Core.Models
{
    public enum ItemType
    {
        HealthPotion,
        AttackBoost,
        DefenseBoost
    }

    public static class ItemTypeExtensions
    {
        public static string GetName(this ItemType type)
        {
            switch (type)
            {
                case ItemType.HealthPotion: return "Health Potion";
                case ItemType.AttackBoost: return "Attack Boost";
                case ItemType.DefenseBoost: return "Defense Boost";
                default: return "Unknown";
            }
        }

        public static string GetDescription(this ItemType type)
        {
            switch (type)
            {
                case ItemType.HealthPotion: return "Restores HP.";
                case ItemType.AttackBoost: return "Increases attack power.";
                case ItemType.DefenseBoost: return "Increases defense power.";
                default: return "";
            }
        }
    }
}
