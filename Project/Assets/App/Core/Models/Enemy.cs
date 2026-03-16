namespace App.Core.Models
{
    public class Enemy : Entity
    {
        public EnemyType EnemyType { get; }

        public Enemy(EnemyType enemyType, Position position)
            : base(enemyType.GetName(), position, enemyType.GetBaseStats())
        {
            EnemyType = enemyType;
        }
    }
}
