using App.Game;
using App.Game.Managers;
using UnityEditor;
using UnityEngine;
using VContainer;

public static class DebugDump
{
    [MenuItem("Tools/Debug Dump")]
    public static void Dump()
    {
        var scope = Object.FindFirstObjectByType<GameLifetimeScope>();
        if (scope == null || scope.Container == null) { Debug.Log("[DebugDump] GameLifetimeScope not found"); return; }
        var gm = scope.Container.Resolve<GameManager>();
        if (gm == null) { Debug.Log("[DebugDump] GameManager not found"); return; }
        var state = gm.GameState;
        if (state == null) { Debug.Log("[DebugDump] GameState is null"); return; }

        Debug.Log($"[DebugDump] Player: {state.Player.Name} pos={state.Player.Position} HP={state.Player.Stats.CurrentHP}/{state.Player.Stats.MaxHP}");
        Debug.Log($"[DebugDump] Floor={state.CurrentFloor} Turn={state.TurnNumber}");
        Debug.Log($"[DebugDump] Enemies={state.Enemies?.Count} Items={state.Items?.Count}");

        var tileCount = Object.FindObjectsByType<App.Game.Views.TileView>(FindObjectsSortMode.None).Length;
        Debug.Log($"[DebugDump] TileView count={tileCount}");
    }
}
