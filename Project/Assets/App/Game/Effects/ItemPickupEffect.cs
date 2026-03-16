using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace App.Game.Effects
{
    public class ItemPickupEffect : MonoBehaviour
    {
        public async UniTask PlayAsync(CancellationToken ct)
        {
            var startPos = transform.position;
            var endPos = startPos + new Vector3(0f, 1f, 0f);
            var startScale = transform.localScale;
            float elapsed = 0f;
            const float duration = 0.3f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                transform.position = Vector3.Lerp(startPos, endPos, t);
                transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
                await UniTask.Yield(ct);
            }

            transform.position = endPos;
            transform.localScale = Vector3.zero;
        }
    }
}
