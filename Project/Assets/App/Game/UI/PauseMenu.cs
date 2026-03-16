using System;
using App.Game.Bootstrap;
using UnityEngine.UIElements;

namespace App.Game.UI
{
    public sealed class PauseMenu : IDisposable
    {
        private readonly VisualElement _overlay;
        private readonly Button _btnResume;
        private readonly Button _btnSave;
        private readonly Button _btnSettings;
        private readonly Button _btnQuitTitle;

        public bool IsVisible => _overlay != null &&
                                 _overlay.style.display == DisplayStyle.Flex;

        public PauseMenu(UIDocument document)
        {
            var root = document.rootVisualElement;

            _overlay = root.Q<VisualElement>("pause-overlay");
            _btnResume = root.Q<Button>("btn-resume");
            _btnSave = root.Q<Button>("btn-save");
            _btnSettings = root.Q<Button>("btn-settings");
            _btnQuitTitle = root.Q<Button>("btn-quit-title");

            _btnResume.clicked += Hide;
            _btnQuitTitle.clicked += OnQuitToTitle;

            Hide();
        }

        public void Show()
        {
            _overlay.style.display = DisplayStyle.Flex;
        }

        public void Hide()
        {
            _overlay.style.display = DisplayStyle.None;
        }

        private static void OnQuitToTitle()
        {
            SceneLoader.LoadTitle();
        }

        public void Dispose()
        {
            if (_btnResume != null) _btnResume.clicked -= Hide;
            if (_btnQuitTitle != null) _btnQuitTitle.clicked -= OnQuitToTitle;
        }
    }
}
