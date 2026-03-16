using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace App.Game.Camera
{
    public sealed class CameraShake : IDisposable
    {
        private readonly Transform _cameraTransform;
        private readonly CancellationTokenSource _cts = new();

        public CameraShake(UnityEngine.Camera camera)
        {
            _cameraTransform = camera.transform;
        }

        public void Shake(float duration = 0.15f, float magnitude = 0.2f)
        {
            ShakeAsync(duration, magnitude, _cts.Token).Forget();
        }

        private async UniTaskVoid ShakeAsync(float duration, float magnitude, CancellationToken ct)
        {
            var originalPos = _cameraTransform.localPosition;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                float offsetX = UnityEngine.Random.Range(-1f, 1f) * magnitude;
                float offsetY = UnityEngine.Random.Range(-1f, 1f) * magnitude;

                _cameraTransform.localPosition = originalPos + new Vector3(offsetX, offsetY, 0f);

                elapsed += Time.deltaTime;
                await UniTask.Yield(ct);
            }

            _cameraTransform.localPosition = originalPos;
        }

        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
        }
    }
}
