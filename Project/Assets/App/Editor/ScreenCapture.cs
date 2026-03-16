using UnityEditor;
using UnityEngine;

public static class GameScreenCapture
{
    [MenuItem("Tools/Capture GameView")]
    public static void Capture()
    {
        UnityEngine.ScreenCapture.CaptureScreenshot("/tmp/pd-gameview.png", 1);
        Debug.Log("[Capture] Saved to /tmp/pd-gameview.png (will appear next frame)");
    }
}
