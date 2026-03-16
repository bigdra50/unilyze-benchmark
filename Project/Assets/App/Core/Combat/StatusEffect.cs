namespace App.Core.Combat
{
    public enum StatusEffectType
    {
        None,
        Poisoned,
        Strengthened,
        Shielded
    }

    public record StatusEffect(StatusEffectType Type, int RemainingTurns)
    {
        public bool IsExpired => RemainingTurns <= 0;

        public StatusEffect Tick() => this with { RemainingTurns = RemainingTurns - 1 };
    }
}
