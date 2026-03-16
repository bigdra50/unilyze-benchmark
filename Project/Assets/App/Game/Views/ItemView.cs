using System.Threading;
using App.Core.Models;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace App.Game.Views
{
    public class ItemView : MonoBehaviour
    {
        private float _baseY;
        private const float FloatAmplitude = 0.1f;
        private const float FloatSpeed = 2f;

        public Item Item { get; private set; }

        public void Init(Item item)
        {
            Item = item;
            var world = ViewFactory.PositionToWorld(item.Position);
            world.y = 0.3f;
            transform.position = world;
            _baseY = world.y;
        }

        public void PickUp()
        {
            PickUpAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        private async UniTaskVoid PickUpAsync(CancellationToken ct)
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

            Destroy(gameObject);
        }

        private void Update()
        {
            var pos = transform.position;
            pos.y = _baseY + Mathf.Sin(Time.time * FloatSpeed) * FloatAmplitude;
            transform.position = pos;
        }
    }
}
