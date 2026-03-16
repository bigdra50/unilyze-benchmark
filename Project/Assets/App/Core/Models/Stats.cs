using System;

namespace App.Core.Models
{
    public class Stats
    {
        public int CurrentHP { get; private set; }
        public int MaxHP { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }

        public bool IsAlive => CurrentHP > 0;

        public Stats(int maxHP, int attack, int defense)
        {
            MaxHP = maxHP;
            CurrentHP = maxHP;
            Attack = attack;
            Defense = defense;
        }

        public int TakeDamage(int damage)
        {
            int actual = Math.Min(Math.Max(0, damage), CurrentHP);
            CurrentHP -= actual;
            return actual;
        }

        public int Heal(int amount)
        {
            int actual = Math.Min(amount, MaxHP - CurrentHP);
            CurrentHP += actual;
            return actual;
        }

        public Stats Clone()
        {
            return new Stats(MaxHP, Attack, Defense)
            {
                CurrentHP = CurrentHP
            };
        }
    }
}
