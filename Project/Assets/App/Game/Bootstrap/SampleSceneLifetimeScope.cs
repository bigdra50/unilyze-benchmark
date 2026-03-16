using App.Game.Bootstrap;
using App.Game.Camera;
using App.Game.Managers;
using App.Game.UI;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;
using VContainer.Unity;

namespace App.Game
{
    public class SampleSceneLifetimeScope : LifetimeScope
    {
        [SerializeField] private UnityEngine.Camera _camera;
        [SerializeField] private UIDocument _hudDocument;
        [SerializeField] private UIDocument _messageLogDocument;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(_camera);

            builder.Register<GameSettings>(Lifetime.Singleton);
            builder.RegisterEntryPoint<GameBootstrap>().AsSelf();

            builder.RegisterEntryPoint<DungeonCamera>(Lifetime.Singleton).AsSelf();
            builder.Register<CameraShake>(Lifetime.Singleton);
            builder.Register<AudioManager>(Lifetime.Singleton);

            builder.Register(_ => new HudController(_hudDocument), Lifetime.Singleton);
            builder.Register(_ => new MessageLog(_messageLogDocument), Lifetime.Singleton);
        }
    }
}
