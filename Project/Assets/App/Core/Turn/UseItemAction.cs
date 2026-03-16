using App.Core.Events;
using App.Core.Inventory;
using App.Core.Models;

namespace App.Core.Turn
{
    public class UseItemAction : IAction
    {
        private readonly Player _player;
        private readonly int _inventoryIndex;

        public UseItemAction(Player player, int inventoryIndex)
        {
            _player = player;
            _inventoryIndex = inventoryIndex;
        }

        public ActionResult Execute(GameState state)
        {
            if (_inventoryIndex < 0 || _inventoryIndex >= _player.Inventory.Count)
                return new ActionResult(false, "Invalid item index.");

            var itemType = _player.Inventory.RemoveAt(_inventoryIndex);
            var description = ItemEffect.Apply(itemType, _player.Stats);
            state.EventBus.Publish(new ItemUsed(_player, itemType));

            return new ActionResult(true, description);
        }
    }
}
