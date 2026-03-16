using UnityEngine.SceneManagement;

namespace App.Game.Bootstrap
{
    public static class SceneLoader
    {
        public static void LoadTitle()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public static void LoadGame()
        {
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                var path = SceneUtility.GetScenePathByBuildIndex(i);
                if (path.Contains("GameScene"))
                {
                    SceneManager.LoadScene("GameScene");
                    return;
                }
            }
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public static void ReloadGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
