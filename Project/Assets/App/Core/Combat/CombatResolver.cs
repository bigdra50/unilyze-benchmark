using System;
using App.Core.Events;
using App.Core.Models;

namespace App.Core.Combat
{
    public record CombatResult(int Damage, bool TargetDied, string Description);

    public static class CombatResolver
    {
        public static CombatResult Resolve(Entity attacker, Entity defender, EventBus events)
        {
            int damage = DamageCalculator.Calculate(attacker.Stats, defender.Stats);
            int actual = defender.Stats.TakeDamage(damage);

            events.Publish(new EntityDamaged(defender, attacker, actual));

            bool died = !defender.Stats.IsAlive;
            if (died)
            {
                events.Publish(new EntityDied(defender));
            }

            string description = died
                ? $"{attacker.Name} dealt {actual} damage to {defender.Name}. {defender.Name} was defeated!"
                : $"{attacker.Name} dealt {actual} damage to {defender.Name}.";

            return new CombatResult(actual, died, description);
        }

        public static CombatResult Resolve(Entity attacker, Entity defender)
        {
            int damage = DamageCalculator.Calculate(attacker.Stats, defender.Stats);
            int actual = defender.Stats.TakeDamage(damage);

            bool died = !defender.Stats.IsAlive;

            string description = died
                ? $"{attacker.Name} dealt {actual} damage to {defender.Name}. {defender.Name} was defeated!"
                : $"{attacker.Name} dealt {actual} damage to {defender.Name}.";

            return new CombatResult(actual, died, description);
        }
    }
}
