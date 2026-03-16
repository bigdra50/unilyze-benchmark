using App.Game.Effects;
using App.Game.Fog;
using App.Game.Input;
using App.Game.Managers;
using App.Game.Views;
using VContainer;
using VContainer.Unity;

namespace App.Game
{
    public class GameLifetimeScope : LifetimeScope
    {
        private Bootstrap.GameMode _mode;

        public void SetMode(Bootstrap.GameMode mode)
        {
            _mode = mode;
        }

        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<Bootstrap.GameStateHolder>(Lifetime.Scoped);

            builder.Register<DungeonManager>(Lifetime.Scoped);

            if (_mode == Bootstrap.GameMode.AutoPilot)
            {
                builder.RegisterEntryPoint<AutoPilotProvider>(Lifetime.Scoped)
                    .As<IInputProvider>();
            }
            else
            {
                builder.RegisterEntryPoint<ManualInputProvider>(Lifetime.Scoped)
                    .As<IInputProvider>();
            }

            builder.Register<DungeonView>(Lifetime.Scoped);

            builder.Register(resolver =>
            {
                var go = ViewFactory.CreatePlayer(new App.Core.Models.Position(0, 0));
                go.transform.SetParent(transform);
                var playerView = go.AddComponent<PlayerView>();
                go.AddComponent<DamageFlash>();
                return playerView;
            }, Lifetime.Scoped);

            builder.Register<FogRenderer>(Lifetime.Scoped);

            builder.RegisterEntryPoint<GameManager>(Lifetime.Scoped).AsSelf();

            builder.RegisterEntryPoint<Bootstrap.GameEntryPoint>();
        }
    }
}
