using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using App.Core.Models;

namespace App.Core.Save
{
    public static class SaveSerializer
    {
        public static string Serialize(SaveData data)
        {
            var lines = new List<string>
            {
                $"name={data.Player.Name}",
                $"level={data.Player.Level}",
                $"experience={data.Player.Experience}",
                $"hp={data.Player.HP}",
                $"maxHp={data.Player.MaxHP}",
                $"attack={data.Player.Attack}",
                $"defense={data.Player.Defense}",
                $"posX={data.Player.PositionX}",
                $"posY={data.Player.PositionY}",
                $"inventory={string.Join(",", data.Player.InventoryItems.Select(i => (int)i))}",
                $"floor={data.Floor.FloorNumber}",
                $"turn={data.Floor.TurnNumber}",
                $"savedAt={data.SavedAt.ToString("O", CultureInfo.InvariantCulture)}"
            };
            return string.Join("\n", lines);
        }

        public static SaveData Deserialize(string text)
        {
            if (string.IsNullOrEmpty(text)) return null;

            try
            {
                var dict = new Dictionary<string, string>();
                foreach (var line in text.Split('\n'))
                {
                    var idx = line.IndexOf('=');
                    if (idx < 0) continue;
                    dict[line.Substring(0, idx)] = line.Substring(idx + 1);
                }

                var inventory = new List<ItemType>();
                if (dict.TryGetValue("inventory", out var inv) && !string.IsNullOrEmpty(inv))
                {
                    foreach (var s in inv.Split(','))
                    {
                        if (int.TryParse(s, out var val))
                            inventory.Add((ItemType)val);
                    }
                }

                var player = new PlayerData(
                    dict.GetValueOrDefault("name", "Hero"),
                    int.Parse(dict.GetValueOrDefault("level", "1")),
                    int.Parse(dict.GetValueOrDefault("experience", "0")),
                    int.Parse(dict.GetValueOrDefault("hp", "100")),
                    int.Parse(dict.GetValueOrDefault("maxHp", "100")),
                    int.Parse(dict.GetValueOrDefault("attack", "10")),
                    int.Parse(dict.GetValueOrDefault("defense", "5")),
                    int.Parse(dict.GetValueOrDefault("posX", "0")),
                    int.Parse(dict.GetValueOrDefault("posY", "0")),
                    inventory);

                var floor = new FloorData(
                    int.Parse(dict.GetValueOrDefault("floor", "1")),
                    int.Parse(dict.GetValueOrDefault("turn", "0")));

                var savedAt = dict.TryGetValue("savedAt", out var ts)
                    ? DateTime.Parse(ts, CultureInfo.InvariantCulture)
                    : DateTime.Now;

                return new SaveData(player, floor, savedAt);
            }
            catch
            {
                return null;
            }
        }
    }
}
