using System;
using App.Core.Models;

namespace App.Core.Inventory
{
    public static class ItemEffect
    {
        public static string Apply(ItemType type, Stats stats)
        {
            switch (type)
            {
                case ItemType.HealthPotion:
                    var healed = stats.Heal(30);
                    return $"Restored {healed} HP.";

                case ItemType.AttackBoost:
                    stats.Attack += 3;
                    return "Attack increased by 3.";

                case ItemType.DefenseBoost:
                    stats.Defense += 2;
                    return "Defense increased by 2.";

                default:
                    return "Nothing happened.";
            }
        }
    }
}
