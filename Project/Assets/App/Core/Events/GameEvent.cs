using App.Core.Models;
using App.Core.Score;

namespace App.Core.Events
{
    public record EntityMoved(Entity Entity, Position From, Position To);

    public record EntityDamaged(Entity Target, Entity Attacker, int Damage);

    public record EntityDied(Entity Entity);

    public record ItemPickedUp(Player Player, Item Item);

    public record ItemUsed(Player Player, ItemType ItemType);

    public record PlayerLeveledUp(Player Player, int NewLevel);

    public record FloorChanged(int NewFloor);

    public record TurnAdvanced(int TurnNumber);

    public record GameOver(RunResult Result);
}
