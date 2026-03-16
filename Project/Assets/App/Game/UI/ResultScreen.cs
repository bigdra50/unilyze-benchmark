using System;
using App.Core.Score;
using App.Game.Bootstrap;
using UnityEngine.UIElements;

namespace App.Game.UI
{
    public sealed class ResultScreen : IDisposable
    {
        private readonly VisualElement _overlay;
        private readonly Label _titleLabel;
        private readonly Label _statFloor;
        private readonly Label _statEnemies;
        private readonly Label _statLevel;
        private readonly Label _statScore;
        private readonly Button _btnRetry;
        private readonly Button _btnTitle;

        public ResultScreen(UIDocument document)
        {
            var root = document.rootVisualElement;

            _overlay = root.Q<VisualElement>("result-overlay");
            _titleLabel = root.Q<Label>("result-title");
            _statFloor = root.Q<Label>("stat-floor");
            _statEnemies = root.Q<Label>("stat-enemies");
            _statLevel = root.Q<Label>("stat-level");
            _statScore = root.Q<Label>("stat-score");
            _btnRetry = root.Q<Button>("btn-retry");
            _btnTitle = root.Q<Button>("btn-title");

            _btnRetry.clicked += OnRetry;
            _btnTitle.clicked += OnTitle;

            Hide();
        }

        public void Show(RunResult result, bool escaped)
        {
            _titleLabel.text = escaped ? "ESCAPED!" : "GAME OVER";
            _statFloor.text = result.FloorsCleared.ToString();
            _statEnemies.text = result.EnemiesDefeated.ToString();
            _statLevel.text = result.PlayerLevel.ToString();
            _statScore.text = result.Score.ToString();

            _overlay.style.display = DisplayStyle.Flex;
        }

        public void Hide()
        {
            _overlay.style.display = DisplayStyle.None;
        }

        private static void OnRetry()
        {
            SceneLoader.ReloadGame();
        }

        private static void OnTitle()
        {
            SceneLoader.LoadTitle();
        }

        public void Dispose()
        {
            if (_btnRetry != null) _btnRetry.clicked -= OnRetry;
            if (_btnTitle != null) _btnTitle.clicked -= OnTitle;
        }
    }
}
