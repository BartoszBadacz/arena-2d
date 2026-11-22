using System;

namespace Arena.Core
{
    // Przebieg jednej rozgrywki: czeka na start (Begin), potem trwa, dopóki gracz żyje. Śmierć gracza = Game Over:
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

        public event Action Started;
        public event Action<RunResult> GameOver;

        public bool HasStarted { get; private set; }
        public bool IsOver => Result != null;
        public RunResult Result { get; private set; }

        // Start biegu: z ekranu startowego (klik) albo od razu przy restarcie. Drugi raz nic nie robi.
        public void Begin()
        {
            if (HasStarted) return;
            HasStarted = true;
            Started?.Invoke();
        }

        // Czas płynie tylko w trakcie gry: po starcie, przed Game Over.
        public void Tick(float deltaTime)
        {
            if (HasStarted && !IsOver) stats.AddTime(deltaTime);
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
