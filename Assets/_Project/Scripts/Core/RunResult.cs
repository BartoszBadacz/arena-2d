namespace Arena.Core
{
    // Podsumowanie zakończonej rozgrywki — zdjęcie stanu w chwili Game Over.
    public sealed class RunResult
    {
        public RunResult(int score, int wave, int kills, float survivedTime, bool isNewRecord, int bestScore)
        {
            Score = score;
            Wave = wave;
            Kills = kills;
            SurvivedTime = survivedTime;
            IsNewRecord = isNewRecord;
            BestScore = bestScore;
        }

        public int Score { get; }
        public int Wave { get; }
        public int Kills { get; }
        public float SurvivedTime { get; }
        public bool IsNewRecord { get; }
        public int BestScore { get; }
    }
}
