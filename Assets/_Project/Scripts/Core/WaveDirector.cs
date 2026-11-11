using System;

namespace Arena.Core
{
    // Fale „do wybicia”: fala = określona liczba wrogów; kończy się, gdy zginie ostatni, potem przerwa i trudniejsza fala.
    // Maszyna stanów: Break → Spawning → Fighting → Break. Nie tworzy wrogów — prosi o nich zdarzeniem SpawnRequested.
    public sealed class WaveDirector
    {
        public enum State { Break, Spawning, Fighting }

        private readonly WaveSettings settings;
        private float now;
        private float stateEndsAt;
        private float nextSpawnTime;
        private int toSpawn;
        private int alive;

        public WaveDirector(WaveSettings settings)
        {
            this.settings = settings;
            Wave = Math.Max(0, settings.StartWave - 1);
            stateEndsAt = settings.BreakDuration;
        }

        public event Action<int> WaveStarted;
        public event Action<int> WaveCleared;
        public event Action<int> SpawnRequested; // ilu wrogów naraz, każdy w innym miejscu

        public State CurrentState { get; private set; } = State.Break;
        public int Wave { get; private set; } // 0 = jeszcze przed pierwszą falą
        public int WaveSize { get; private set; } // ilu wrogów liczy bieżąca fala (znane od WaveStarted)
        public int EnemiesRemaining => toSpawn + alive; // do pojawienia się + żywi
        public float BreakTimeLeft => CurrentState == State.Break ? Math.Max(0f, stateEndsAt - now) : 0f;
        public bool IsSpeedUpWave => Wave > 1 && (Wave - 1) % settings.WavesPerSpeedUp == 0;
        public float SpeedBonus => (Wave - 1) / settings.WavesPerSpeedUp * settings.SpeedBonusPerSpeedUp;

        private int BatchSize => Math.Min(1 + (Wave - 1) / settings.WavesPerBatchGrowth, settings.MaxBatchSize);

        private float Interval =>
            Math.Max(settings.StartInterval * (float)Math.Pow(settings.IntervalMultiplierPerWave, Wave - 1), settings.MinInterval);

        public void Tick(float deltaTime)
        {
            now += deltaTime;

            switch (CurrentState)
            {
                case State.Break:
                    if (now >= stateEndsAt) StartNextWave();
                    break;

                case State.Spawning:
                    if (now < nextSpawnTime) break;
                    nextSpawnTime = now + Interval;
                    SpawnBatch();
                    break;

                case State.Fighting:
                    break; // koniec fali sprawdza EnemyDied
            }
        }

        // Spawner melduje śmierć każdego wroga, którego stworzył na prośbę tej fali.
        public void EnemyDied()
        {
            if (alive == 0) return;
            alive--;
            if (CurrentState != State.Fighting || alive > 0) return;

            CurrentState = State.Break;
            stateEndsAt = now + settings.BreakDuration;
            WaveCleared?.Invoke(Wave);
        }

        private void StartNextWave()
        {
            Wave++;
            toSpawn = settings.FirstWaveCount + settings.ExtraEnemiesPerWave * (Wave - 1);
            WaveSize = toSpawn;
            nextSpawnTime = now;
            CurrentState = State.Spawning;
            WaveStarted?.Invoke(Wave);
            Tick(0f); // pierwsza paczka od razu, w tej samej klatce co start fali
        }

        private void SpawnBatch()
        {
            int count = Math.Min(BatchSize, toSpawn);
            toSpawn -= count;
            alive += count;
            if (toSpawn == 0) CurrentState = State.Fighting;
            SpawnRequested?.Invoke(count);
        }
    }
}
