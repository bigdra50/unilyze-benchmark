using System;
using App.Game.Managers;
using UnityEngine;
using UnityEngine.UIElements;

namespace App.Game.UI
{
    public sealed class SettingsPanel : IDisposable
    {
        private readonly AudioManager _audioManager;
        private readonly VisualElement _overlay;
        private readonly Slider _seVolumeSlider;
        private readonly Button _btnClose;

        public bool IsVisible => _overlay != null &&
                                 _overlay.style.display == DisplayStyle.Flex;

        public SettingsPanel(UIDocument document, AudioManager audioManager)
        {
            _audioManager = audioManager;

            var root = document.rootVisualElement;

            _overlay = root.Q<VisualElement>("settings-overlay");
            _seVolumeSlider = root.Q<Slider>("se-volume-slider");
            _btnClose = root.Q<Button>("btn-settings-close");

            _seVolumeSlider.value = PlayerPrefs.GetFloat("SEVolume", 1f);
            _seVolumeSlider.RegisterValueChangedCallback(OnVolumeChanged);
            _btnClose.clicked += Hide;

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

        private void OnVolumeChanged(ChangeEvent<float> evt)
        {
            PlayerPrefs.SetFloat("SEVolume", evt.newValue);
            _audioManager?.SetVolume(evt.newValue);
        }

        public void Dispose()
        {
            if (_seVolumeSlider != null) _seVolumeSlider.UnregisterValueChangedCallback(OnVolumeChanged);
            if (_btnClose != null) _btnClose.clicked -= Hide;
        }
    }
}
