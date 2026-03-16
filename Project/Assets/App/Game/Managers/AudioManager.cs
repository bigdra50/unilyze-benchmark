using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace App.Game.Managers
{
    public sealed class AudioManager : IDisposable
    {
        private readonly GameObject _audioPlayer;
        private readonly AudioSource _audioSource;

        public AudioManager()
        {
            _audioPlayer = new GameObject("[AudioManager]");
            _audioSource = _audioPlayer.AddComponent<AudioSource>();
        }

        public void PlaySE(AudioClip clip)
        {
            if (clip == null) return;
            _audioSource.PlayOneShot(clip);
        }

        public void SetVolume(float volume)
        {
            _audioSource.volume = volume;
        }

        public void Dispose()
        {
            if (_audioPlayer != null)
                Object.Destroy(_audioPlayer);
        }
    }
}
