namespace App.Core.Models
{
    public enum EnemyType
    {
        Slime,
        Goblin,
        Dragon
    }

    public static class EnemyTypeExtensions
    {
        public static Stats GetBaseStats(this EnemyType type)
        {
            switch (type)
            {
                case EnemyType.Slime: return new Stats(15, 3, 1);
                case EnemyType.Goblin: return new Stats(25, 5, 2);
                case EnemyType.Dragon: return new Stats(50, 10, 5);
                default: return new Stats(10, 1, 0);
            }
        }

        public static string GetName(this EnemyType type)
        {
            switch (type)
            {
                case EnemyType.Slime: return "Slime";
                case EnemyType.Goblin: return "Goblin";
                case EnemyType.Dragon: return "Dragon";
                default: return "Unknown";
            }
        }

        public static int GetExperienceReward(this EnemyType type)
        {
            switch (type)
            {
                case EnemyType.Slime: return 10;
                case EnemyType.Goblin: return 25;
                case EnemyType.Dragon: return 100;
                default: return 0;
            }
        }
    }
}
