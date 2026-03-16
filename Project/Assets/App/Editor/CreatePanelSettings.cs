using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public static class CreatePanelSettingsAsset
{
    [MenuItem("Tools/Create PanelSettings")]
    public static void Create()
    {
        var ps = ScriptableObject.CreateInstance<PanelSettings>();
        ps.scaleMode = PanelScaleMode.ScaleWithScreenSize;
        ps.referenceResolution = new Vector2Int(1920, 1080);

        AssetDatabase.CreateAsset(ps, "Assets/Resources/DefaultPanelSettings.asset");
        AssetDatabase.SaveAssets();
        Debug.Log($"[CreatePanelSettings] Created. Theme: {ps.themeStyleSheet}");
    }
}
