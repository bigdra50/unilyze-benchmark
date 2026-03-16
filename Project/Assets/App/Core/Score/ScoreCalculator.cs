namespace App.Core.Score
{
    public static class ScoreCalculator
    {
        public static int Calculate(RunResult result)
        {
            return result.FloorsCleared * 1000
                 + result.EnemiesDefeated * 100
                 + result.PlayerLevel * 500
                 - result.TurnsElapsed;
        }
    }
}
