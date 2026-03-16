using System;
using System.IO;
using App.Game.Bootstrap;
using UnityEngine;
using UnityEngine.UIElements;

namespace App.Game.UI
{
    public sealed class TitleScreen : IDisposable
    {
        private readonly Button _btnNewGame;
        private readonly Button _btnContinue;
        private readonly Button _btnSettings;
        private readonly Button _btnQuit;

        public TitleScreen(UIDocument document)
        {
            var root = document.rootVisualElement;

            _btnNewGame = root.Q<Button>("btn-new-game");
            _btnContinue = root.Q<Button>("btn-continue");
            _btnSettings = root.Q<Button>("btn-settings");
            _btnQuit = root.Q<Button>("btn-quit");

            _btnNewGame.clicked += OnNewGame;
            _btnContinue.clicked += OnContinue;
            _btnQuit.clicked += OnQuit;

            RefreshContinueButton();
        }

        private void RefreshContinueButton()
        {
            var savePath = Path.Combine(Application.persistentDataPath, "save.dat");
            _btnContinue.SetEnabled(File.Exists(savePath));
        }

        private void OnNewGame()
        {
            SceneLoader.LoadGame();
        }

        private void OnContinue()
        {
            SceneLoader.LoadGame();
        }

        private static void OnQuit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public void Dispose()
        {
            if (_btnNewGame != null) _btnNewGame.clicked -= OnNewGame;
            if (_btnContinue != null) _btnContinue.clicked -= OnContinue;
            if (_btnQuit != null) _btnQuit.clicked -= OnQuit;
        }
    }
}
