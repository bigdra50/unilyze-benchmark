using UnityEngine;
using VContainer.Unity;

namespace App.Game.Camera
{
    public sealed class DungeonCamera : IPostLateTickable
    {
        private readonly Transform _cameraTransform;
        private Transform _target;
        private const float Height = 10f;
        private const float SmoothSpeed = 5f;

        public bool Enabled { get; set; } = true;

        public DungeonCamera(UnityEngine.Camera camera)
        {
            _cameraTransform = camera.transform;
        }

        public void SetTarget(Transform target)
        {
            _target = target;
            if (_target != null)
            {
                _cameraTransform.position = _target.position + Vector3.up * Height;
                _cameraTransform.eulerAngles = new Vector3(90f, 0f, 0f);
            }
        }

        public void PostLateTick()
        {
            if (!Enabled || _target == null) return;

            var targetPosition = _target.position + Vector3.up * Height;
            _cameraTransform.position = Vector3.Lerp(_cameraTransform.position, targetPosition, SmoothSpeed * Time.deltaTime);
            _cameraTransform.eulerAngles = new Vector3(90f, 0f, 0f);
        }
    }
}
