using System.Threading;
using App.Core.Models;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace App.Game.Views
{
    public class EnemyView : MonoBehaviour
    {
        private Vector3 _targetPosition;
        private const float MoveSpeed = 8f;

        public Enemy Enemy { get; private set; }

        public void Init(Enemy enemy)
        {
            Enemy = enemy;
            SyncPosition(enemy.Position);
        }

        public void SyncPosition(Position position)
        {
            var world = ViewFactory.PositionToWorld(position);
            world.y = 0.5f;
            _targetPosition = world;
        }

        public void PlayDeathEffect()
        {
            DeathAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        private async UniTaskVoid DeathAsync(CancellationToken ct)
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
            transform.position = Vector3.Lerp(transform.position, _targetPosition, Time.deltaTime * MoveSpeed);
        }
    }
}
