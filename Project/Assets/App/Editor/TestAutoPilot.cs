using App.Game;
using App.Game.Bootstrap;
using UnityEditor;
using UnityEngine;
using VContainer;

public static class TestAutoPilot
{
    [MenuItem("Tools/Start AutoPilot")]
    public static void StartAutoPilot()
    {
        var scope = Object.FindFirstObjectByType<SampleSceneLifetimeScope>();
        if (scope == null)
        {
            Debug.LogError("[TestAutoPilot] SampleSceneLifetimeScope not found in scene");
            return;
        }

        var bootstrap = scope.Container.Resolve<GameBootstrap>();
        bootstrap.LaunchGame(GameMode.AutoPilot);
    }
}
