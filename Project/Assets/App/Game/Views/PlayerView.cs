using App.Core.Models;
using UnityEngine;

namespace App.Game.Views
{
    public class PlayerView : MonoBehaviour
    {
        private Vector3 _targetPosition;
        private const float MoveSpeed = 8f;

        private void Awake()
        {
            _targetPosition = transform.position;
        }

        public void SyncPosition(Position position, bool immediate = false)
        {
            var world = ViewFactory.PositionToWorld(position);
            world.y = 0.75f;
            _targetPosition = world;
            if (immediate)
                transform.position = world;
        }

        private void Update()
        {
            transform.position = Vector3.Lerp(transform.position, _targetPosition, Time.deltaTime * MoveSpeed);
        }
    }
}
