using App.Core.Dungeon;
using App.Core.Events;
using App.Core.Models;

namespace App.Core.Turn
{
    public class MoveAction : IAction
    {
        private readonly Entity _entity;
        private readonly Direction _direction;

        public MoveAction(Entity entity, Direction direction)
        {
            _entity = entity;
            _direction = direction;
        }

        public ActionResult Execute(GameState state)
        {
            var from = _entity.Position;
            var targetPos = from + _direction.ToOffset();

            if (!state.Map.IsWalkable(targetPos))
                return new ActionResult(false, "Blocked by wall.");

            if (state.GetEntityAt(targetPos) != null)
                return new ActionResult(false, "Blocked by entity.");

            _entity.Position = targetPos;
            state.EventBus.Publish(new EntityMoved(_entity, from, targetPos));

            if (_entity is Player player)
            {
                var item = state.GetItemAt(targetPos);
                if (item != null && !player.Inventory.IsFull)
                {
                    player.Inventory.Add(item.ItemType);
                    state.RemoveItem(item);
                    state.EventBus.Publish(new ItemPickedUp(player, item));
                }

                if (state.Map.GetTile(targetPos).TileType == TileType.Stairs)
                {
                    state.EventBus.Publish(new FloorChanged(state.CurrentFloor + 1));
                }
            }

            return new ActionResult(true, $"{_entity.Name} moved {_direction}.");
        }
    }
}
