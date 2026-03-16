using System.Reflection;
using App.Game;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

public static class SetupScene
{
    [MenuItem("Tools/Setup Scene")]
    public static void Setup()
    {
        MigrateToPlainCSharp();
    }

    [MenuItem("Tools/Migrate MonoBehaviour to Plain CSharp")]
    public static void MigrateToPlainCSharp()
    {
        var mainCamera = FindByName<Camera>("Main Camera");
        var hudGo = GameObject.Find("HUD");
        var messageLogGo = GameObject.Find("MessageLog");
        var lifetimeScopeGo = GameObject.Find("[LifetimeScope]");

        if (mainCamera == null || hudGo == null || messageLogGo == null || lifetimeScopeGo == null)
        {
            Debug.LogError("[Migration] Required GameObjects not found. Ensure SampleScene is open.");
            return;
        }

        var scope = lifetimeScopeGo.GetComponent<SampleSceneLifetimeScope>();
        if (scope == null)
        {
            Debug.LogError("[Migration] SampleSceneLifetimeScope not found on [LifetimeScope] GO.");
            return;
        }

        // 1. Remove old MonoBehaviour components from Main Camera
        RemoveComponentByName(mainCamera.gameObject, "DungeonCamera");
        RemoveComponentByName(mainCamera.gameObject, "CameraShake");

        // 2. Remove old MonoBehaviour components from UI GameObjects
        RemoveComponentByName(hudGo, "HudController");
        RemoveComponentByName(messageLogGo, "MessageLog");

        // 3. Wire SerializeFields on SampleSceneLifetimeScope
        var so = new SerializedObject(scope);

        // _camera
        var cameraProp = so.FindProperty("_camera");
        cameraProp.objectReferenceValue = mainCamera;

        // _hudDocument
        var hudDoc = hudGo.GetComponent<UIDocument>();
        var hudDocProp = so.FindProperty("_hudDocument");
        hudDocProp.objectReferenceValue = hudDoc;

        // _messageLogDocument
        var msgDoc = messageLogGo.GetComponent<UIDocument>();
        var msgDocProp = so.FindProperty("_messageLogDocument");
        msgDocProp.objectReferenceValue = msgDoc;

        so.ApplyModifiedProperties();

        EditorUtility.SetDirty(scope);
        EditorSceneManager.MarkSceneDirty(scope.gameObject.scene);

        Debug.Log("[Migration] Done. Removed old MonoBehaviours, wired new SerializeFields. Save the scene.");
    }

    private static T FindByName<T>(string name) where T : Component
    {
        var go = GameObject.Find(name);
        return go != null ? go.GetComponent<T>() : null;
    }

    private static void RemoveComponentByName(GameObject go, string typeName)
    {
        // plain C# 化されたスクリプトはコンポーネントとして null になる
        var components = go.GetComponents<Component>();
        var removed = 0;
        for (int i = components.Length - 1; i >= 0; i--)
        {
            if (components[i] == null)
            {
                var so = new SerializedObject(go);
                // SerializedObject からコンポーネントを除去
                removed++;
            }
        }

        // Missing Script を除去
        GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);

        // シーンYAML上の壊れた参照を除去するため SerializedObject 経由で再保存
        var gso = new SerializedObject(go);
        var prop = gso.FindProperty("m_Component");
        if (prop != null && prop.isArray)
        {
            for (int i = prop.arraySize - 1; i >= 0; i--)
            {
                var element = prop.GetArrayElementAtIndex(i);
                var compRef = element.FindPropertyRelative("component");
                if (compRef != null && compRef.objectReferenceValue == null)
                {
                    prop.DeleteArrayElementAtIndex(i);
                    removed++;
                }
            }
            gso.ApplyModifiedProperties();
        }

        Debug.Log($"[Migration] Cleaned {removed} broken component(s) from {go.name} (was: {typeName})");
    }
}
