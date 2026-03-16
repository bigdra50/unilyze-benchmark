namespace App.Game.Bootstrap
{
    public enum GameMode
    {
        Manual,
        AutoPilot
    }

    public sealed class GameSettings
    {
        public bool IsGameStarted { get; set; }
        public GameMode Mode { get; set; } = GameMode.Manual;
    }
}
