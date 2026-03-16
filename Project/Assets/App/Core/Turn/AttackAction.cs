using App.Core.Combat;
using App.Core.Events;
using App.Core.Models;

namespace App.Core.Turn
{
    public class AttackAction : IAction
    {
        private readonly Entity _attacker;
        private readonly Entity _target;

        public AttackAction(Entity attacker, Entity target)
        {
            _attacker = attacker;
            _target = target;
        }

        public ActionResult Execute(GameState state)
        {
            var result = CombatResolver.Resolve(_attacker, _target, state.EventBus);

            if (result.TargetDied)
            {
                if (_target is Enemy enemy)
                {
                    state.RemoveEnemy(enemy);
                    if (_attacker is Player player)
                    {
                        player.GainExperience(enemy.EnemyType.GetExperienceReward());
                        player.TotalEnemiesDefeated++;
                    }
                }
            }

            return new ActionResult(true,
                $"{_attacker.Name} dealt {result.Damage} damage to {_target.Name}.");
        }
    }
}
