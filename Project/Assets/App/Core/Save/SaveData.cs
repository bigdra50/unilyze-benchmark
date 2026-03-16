using System;
using System.Collections.Generic;
using App.Core.Models;

namespace App.Core.Save
{
    public record PlayerData(
        string Name,
        int Level,
        int Experience,
        int HP,
        int MaxHP,
        int Attack,
        int Defense,
        int PositionX,
        int PositionY,
        List<ItemType> InventoryItems
    );

    public record FloorData(int FloorNumber, int TurnNumber);

    public record SaveData(PlayerData Player, FloorData Floor, DateTime SavedAt);
}
