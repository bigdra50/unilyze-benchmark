using App.Game;
using App.Game.Managers;
using UnityEditor;
using UnityEngine;
using VContainer;

public static class TestGameOver
{
    [MenuItem("Tools/Force Game Over")]
    public static void ForceGameOver()
    {
        var scope = Object.FindFirstObjectByType<GameLifetimeScope>();
        if (scope == null || scope.Container == null)
        {
            Debug.Log("[TestGameOver] GameLifetimeScope not found");
            return;
        }

        var gm = scope.Container.Resolve<GameManager>();
        if (gm == null || gm.GameState == null)
        {
            Debug.Log("[TestGameOver] GameManager or GameState not found");
            return;
        }

        gm.GameState.Player.Stats.TakeDamage(9999);
        Debug.Log("[TestGameOver] Forced player HP to 0");
    }
}
