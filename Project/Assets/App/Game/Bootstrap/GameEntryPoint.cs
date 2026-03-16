using App.Game.Managers;
using VContainer.Unity;

namespace App.Game.Bootstrap
{
    public sealed class GameEntryPoint : IStartable
    {
        private readonly GameManager _gameManager;

        public GameEntryPoint(GameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public void Start()
        {
            _gameManager.StartGame();
        }
    }
}
