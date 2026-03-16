using System;
using App.Game.Camera;
using App.Game.Managers;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Object = UnityEngine.Object;

namespace App.Game.Bootstrap
{
    public sealed class GameBootstrap : IStartable, IDisposable
    {
        private readonly GameSettings _gameSettings;
        private readonly LifetimeScope _rootScope;
        private readonly DungeonCamera _dungeonCamera;
        private readonly UnityEngine.Camera _camera;

        private TitleSetup _titleSetup;
        private GameLifetimeScope _gameScope;

        public GameBootstrap(
            GameSettings gameSettings,
            LifetimeScope rootScope,
            DungeonCamera dungeonCamera,
            UnityEngine.Camera camera)
        {
            _gameSettings = gameSettings;
            _rootScope = rootScope;
            _dungeonCamera = dungeonCamera;
            _camera = camera;
        }

        public void Start()
        {
            Application.runInBackground = true;
            ShowTitle();
        }

        private void ShowTitle()
        {
            if (_gameScope != null)
            {
                Object.Destroy(_gameScope.gameObject);
                _gameScope = null;
            }

            _dungeonCamera.Enabled = false;
            _camera.clearFlags = CameraClearFlags.SolidColor;
            _camera.backgroundColor = new Color(0.1f, 0.1f, 0.18f);
            _camera.orthographic = false;
            _camera.fieldOfView = 60f;
            _camera.transform.position = new Vector3(0, 0, -10);
            _camera.transform.rotation = Quaternion.identity;

            _titleSetup = new TitleSetup(LaunchGame);
        }

        public void LaunchGame(GameMode mode)
        {
            _titleSetup?.Dispose();
            _titleSetup = null;

            _gameSettings.Mode = mode;
            _dungeonCamera.Enabled = true;

            var go = new GameObject("[GameScope]");
            go.SetActive(false);
            go.transform.SetParent(_rootScope.transform, false);

            _gameScope = go.AddComponent<GameLifetimeScope>();
            _gameScope.SetMode(mode);
            _gameScope.parentReference.Object = _rootScope;

            go.SetActive(true);

            var gameManager = _gameScope.Container.Resolve<GameManager>();
            gameManager.OnGameOver = ShowTitle;
        }

        public void Dispose()
        {
            if (_gameScope != null) Object.Destroy(_gameScope.gameObject);
            _titleSetup?.Dispose();
        }
    }
}
