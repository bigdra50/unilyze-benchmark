using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace App.Game.Effects
{
    public class DeathEffect : MonoBehaviour
    {
        public async UniTask PlayAsync(CancellationToken ct)
        {
            var startScale = transform.localScale;
            float elapsed = 0f;
            const float duration = 0.3f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
                await UniTask.Yield(ct);
            }

            transform.localScale = Vector3.zero;
        }
    }
}
