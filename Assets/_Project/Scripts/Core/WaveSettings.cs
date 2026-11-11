namespace Arena.Core
{
    // Liczby krzywej fal w postaci, którą rozumie Core. Tworzy je WaveConfig.ToSettings() (Data).
    public sealed class WaveSettings
    {
        public float BreakDuration { get; set; } = 3f;
        public int FirstWaveCount { get; set; } = 6;
        public float ExtraEnemiesPerWave { get; set; } = 3.6f; // ułamkowy przyrost; liczność fali jest zaokrąglana
        public float StartInterval { get; set; } = 1.5f;
        public float IntervalMultiplierPerWave { get; set; } = 0.9f;
        public float MinInterval { get; set; } = 0.4f;
        public int WavesPerBatchGrowth { get; set; } = 2;
        public int MaxBatchSize { get; set; } = 4;
        public int WavesPerSpeedUp { get; set; } = 3;
        public float SpeedBonusPerSpeedUp { get; set; } = 0.6f;
        public int StartWave { get; set; } = 1;
    }
}
