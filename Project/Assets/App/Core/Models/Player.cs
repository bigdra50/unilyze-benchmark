namespace App.Core.Models
{
    public class Player : Entity
    {
        public int Level { get; private set; }
        public int Experience { get; private set; }
        public int TotalEnemiesDefeated { get; set; }
        public Inventory.Inventory Inventory { get; } = new();

        public int ExperienceToNextLevel => Level * 50;

        public Player(string name, Position position, Stats stats)
            : base(name, position, stats)
        {
            Level = 1;
            Experience = 0;
            TotalEnemiesDefeated = 0;
        }

        public bool GainExperience(int amount)
        {
            Experience += amount;
            if (Experience < ExperienceToNextLevel)
                return false;

            Experience -= ExperienceToNextLevel;
            Level++;
            Stats.MaxHP += 5;
            Stats.Attack += 2;
            Stats.Defense += 1;
            Stats.Heal(5);
            return true;
        }
    }
}
