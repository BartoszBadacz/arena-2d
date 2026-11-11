using System;

namespace Arena.Core
{
    // Przebieg jednej rozgrywki: trwa, dopóki gracz żyje. Śmierć gracza = Game Over:
    // zdjęcie wyniku, zgłoszenie rekordu i jedno zdarzenie GameOver dla ekranu, pauzy i restartu.
    public sealed class RunSession
    {
        private readonly RunStats stats;
        private readonly SessionRecord record;

        public RunSession(Health playerHealth, RunStats stats, SessionRecord record)
        {
            this.stats = stats;
            this.record = record;
            playerHealth.Died += End;
        }

        public event Action<RunResult> GameOver;

        public bool IsOver => Result != null;
        public RunResult Result { get; private set; }

        // Czas płynie tylko w trakcie gry.
        public void Tick(float deltaTime)
        {
            if (!IsOver) stats.AddTime(deltaTime);
        }

        private void End()
        {
            if (IsOver) return;
            bool isNewRecord = record.Submit(stats.Score);
            Result = new RunResult(stats.Score, stats.Wave, stats.Kills, stats.SurvivedTime, isNewRecord, record.BestScore);
            GameOver?.Invoke(Result);
        }
    }
}
