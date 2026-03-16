using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace App.Game.Effects
{
    public class DamageFlash : MonoBehaviour
    {
        private Renderer _renderer;
        private Color _originalColor;
        private CancellationTokenSource _cts;

        private void Awake()
        {
            _renderer = GetComponent<Renderer>();
            if (_renderer != null)
            {
                _originalColor = _renderer.material.color;
            }
        }

        public void Flash()
        {
            if (_renderer == null) return;
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());
            FlashAsync(_cts.Token).Forget();
        }

        private async UniTaskVoid FlashAsync(CancellationToken ct)
        {
            _originalColor = _renderer.material.color;
            _renderer.material.color = Color.red;
            await UniTask.Delay(TimeSpan.FromSeconds(0.2f), cancellationToken: ct);
            _renderer.material.color = _originalColor;
        }

        private void OnDestroy()
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }
    }
}
