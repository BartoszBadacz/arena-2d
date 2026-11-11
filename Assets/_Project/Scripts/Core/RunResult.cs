namespace Arena.Core
{
    // Podsumowanie zakończonej rozgrywki — zdjęcie stanu w chwili Game Over.
    public sealed class RunResult
    {
        public RunResult(int wave, int kills, float survivedTime, bool isNewRecord, int bestWave)
        {
            Wave = wave;
            Kills = kills;
            SurvivedTime = survivedTime;
            IsNewRecord = isNewRecord;
            BestWave = bestWave;
        }

        public int Wave { get; }
        public int Kills { get; }
        public float SurvivedTime { get; }
        public bool IsNewRecord { get; }
        public int BestWave { get; }
    }
}
