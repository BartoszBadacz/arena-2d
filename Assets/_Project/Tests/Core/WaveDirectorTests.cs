using NUnit.Framework;

namespace Arena.Core.Tests
{
    public class WaveDirectorTests
    {
        private WaveSettings settings;
        private WaveDirector director;
        private int spawned;

        [SetUp]
        public void SetUp()
        {
            settings = new WaveSettings(); // domyślne = liczby z prototypu
            CreateDirector();
        }

        private void CreateDirector()
        {
            director = new WaveDirector(settings);
            spawned = 0;
            director.SpawnRequested += count => spawned += count;
        }

        // Tyka, aż wszyscy wrogowie bieżącej fali się pojawią (krok 0.1 s, z limitem bezpieczeństwa).
        private void TickUntilAllSpawned()
        {
            for (int i = 0; i < 10000 && director.CurrentState != WaveDirector.State.Fighting; i++) director.Tick(0.1f);
        }

        private void KillAll()
        {
            int remaining = director.EnemiesRemaining;
            for (int i = 0; i < remaining; i++) director.EnemyDied();
        }

        private void ClearWavesUpTo(int wave)
        {
            while (director.Wave < wave)
            {
                director.Tick(settings.BreakDuration);
                TickUntilAllSpawned();
                KillAll();
            }
        }

        [Test]
        public void StartsWithBreakBeforeFirstWave()
        {
            Assert.AreEqual(WaveDirector.State.Break, director.CurrentState);
            Assert.AreEqual(0, director.Wave);
            Assert.AreEqual(settings.BreakDuration, director.BreakTimeLeft, 0.0001f);
        }

        [Test]
        public void AfterBreak_FirstWaveStartsAndSpawnsFirstBatchAtOnce()
        {
            int startedWave = 0;
            director.WaveStarted += wave => startedWave = wave;

            director.Tick(settings.BreakDuration);

            Assert.AreEqual(1, startedWave);
            Assert.AreEqual(WaveDirector.State.Spawning, director.CurrentState);
            Assert.AreEqual(1, spawned);
        }

        [Test]
        public void FirstWave_SpawnsFirstWaveCountThenFights()
        {
            director.Tick(settings.BreakDuration);

            TickUntilAllSpawned();

            Assert.AreEqual(settings.FirstWaveCount, spawned);
            Assert.AreEqual(WaveDirector.State.Fighting, director.CurrentState);
            Assert.AreEqual(settings.FirstWaveCount, director.EnemiesRemaining);
        }

        [Test]
        public void KillingLastEnemy_ClearsWaveAndStartsBreak()
        {
            director.Tick(settings.BreakDuration);
            TickUntilAllSpawned();
            int clearedWave = 0;
            director.WaveCleared += wave => clearedWave = wave;

            KillAll();

            Assert.AreEqual(1, clearedWave);
            Assert.AreEqual(WaveDirector.State.Break, director.CurrentState);
            Assert.AreEqual(0, director.EnemiesRemaining);
        }

        [Test]
        public void KillsDuringSpawning_DoNotEndWave()
        {
            director.Tick(settings.BreakDuration); // 1 wróg na planszy, reszta jeszcze nie wyszła

            director.EnemyDied();

            Assert.AreEqual(WaveDirector.State.Spawning, director.CurrentState);
            Assert.AreEqual(settings.FirstWaveCount - 1, director.EnemiesRemaining);
        }

        [Test]
        public void NextWave_HasMoreEnemies()
        {
            ClearWavesUpTo(1);
            spawned = 0;

            director.Tick(settings.BreakDuration);
            TickUntilAllSpawned();

            Assert.AreEqual(2, director.Wave);
            Assert.AreEqual(director.WaveSizeFor(2), spawned);
            Assert.Greater(spawned, settings.FirstWaveCount);
            Assert.AreEqual(spawned, director.WaveSize);
        }

        [Test]
        public void FractionalGrowth_IsRounded()
        {
            settings.FirstWaveCount = 6;
            settings.ExtraEnemiesPerWave = 3.6f;

            Assert.AreEqual(6, director.WaveSizeFor(1));
            Assert.AreEqual(10, director.WaveSizeFor(2));  // 9.6
            Assert.AreEqual(20, director.WaveSizeFor(5));  // 20.4 (wcześniej 22 przy +4)
            Assert.AreEqual(38, director.WaveSizeFor(10)); // 38.4 (wcześniej 42)
        }

        [Test]
        public void BatchSize_GrowsEveryFewWavesUpToMax()
        {
            int lastBatch = 0;
            director.SpawnRequested += count => lastBatch = count;

            ClearWavesUpTo(2);
            director.Tick(settings.BreakDuration); // fala 3
            Assert.AreEqual(2, lastBatch);

            settings.StartWave = 20;
            CreateDirector();
            director.SpawnRequested += count => lastBatch = count;
            director.Tick(settings.BreakDuration); // fala 20
            Assert.AreEqual(settings.MaxBatchSize, lastBatch);
        }

        [Test]
        public void SpeedUp_EveryFewWaves()
        {
            ClearWavesUpTo(3);
            Assert.IsFalse(director.IsSpeedUpWave);
            Assert.AreEqual(0f, director.SpeedBonus, 0.0001f);

            ClearWavesUpTo(4);
            Assert.IsTrue(director.IsSpeedUpWave);
            Assert.AreEqual(settings.SpeedBonusPerSpeedUp, director.SpeedBonus, 0.0001f);
        }

        [Test]
        public void StartWave_SkipsEarlierWaves()
        {
            settings.StartWave = 5;
            CreateDirector();

            director.Tick(settings.BreakDuration);

            Assert.AreEqual(5, director.Wave);
        }

        [Test]
        public void StrayDeath_WithoutEnemiesAlive_IsIgnored()
        {
            director.EnemyDied();

            Assert.AreEqual(WaveDirector.State.Break, director.CurrentState);
            Assert.AreEqual(0, director.EnemiesRemaining);
        }
    }
}
