using App.Core.Dungeon;
using App.Game.Views;

namespace App.Game.Fog
{
    public sealed class FogRenderer
    {
        public void ApplyFog(DungeonMap map, DungeonView dungeonView)
        {
            if (dungeonView == null) return;
            dungeonView.UpdateVisibility(map);
        }
    }
}
