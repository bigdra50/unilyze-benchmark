using System.Collections.Generic;
using System.Linq;
using App.Core.Dungeon;
using App.Core.Events;
using App.Core.Models;

namespace App.Core.Turn
{
    public class GameState
    {
        public Player Player { get; set; }
        public List<Enemy> Enemies { get; set; } = new();
        public List<Item> Items { get; set; } = new();
        public DungeonMap Map { get; set; }
        public int CurrentFloor { get; set; }
        public int TurnNumber { get; set; }
        public EventBus EventBus { get; set; } = new();

        public Entity GetEntityAt(Position position)
        {
            if (Player.Position == position)
                return Player;

            return Enemies.FirstOrDefault(e => e.Position == position);
        }

        public Item GetItemAt(Position position)
        {
            return Items.FirstOrDefault(i => i.Position == position);
        }

        public void RemoveEnemy(Enemy enemy)
        {
            Enemies.Remove(enemy);
        }

        public void RemoveItem(Item item)
        {
            Items.Remove(item);
        }
    }
}
