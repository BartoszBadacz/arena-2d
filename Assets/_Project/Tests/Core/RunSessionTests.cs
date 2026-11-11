using NUnit.Framework;

namespace Arena.Core.Tests
{
    public class RunSessionTests
    {
        private Health playerHealth;
        private RunStats stats;
        private SessionRecord record;
        private RunSession session;

        [SetUp]
        public void SetUp()
        {
            playerHealth = new Health(max: 1);
            stats = new RunStats();
            record = new SessionRecord();
            session = new RunSession(playerHealth, stats, record);
        }

        [Test]
        public void WhilePlayerAlive_TimeFlows()
        {
            session.Tick(0.5f);
            session.Tick(0.25f);

            Assert.IsFalse(session.IsOver);
            Assert.AreEqual(0.75f, stats.SurvivedTime, 0.0001f);
        }

        [Test]
        public void PlayerDeath_RaisesGameOverOnceWithSnapshot()
        {
            stats.ReachWave(4);
            stats.RegisterKill();
            stats.RegisterKill();
            session.Tick(12f);
            int gameOverCount = 0;
            RunResult received = null;
            session.GameOver += result => { gameOverCount++; received = result; };

            playerHealth.TakeDamage(1, now: 0f);
            playerHealth.TakeDamage(1, now: 5f); // martwy — nie kończy gry drugi raz

            Assert.IsTrue(session.IsOver);
            Assert.AreEqual(1, gameOverCount);
            Assert.AreEqual(4, received.Wave);
            Assert.AreEqual(2, received.Kills);
            Assert.AreEqual(12f, received.SurvivedTime, 0.0001f);
            Assert.IsTrue(received.IsNewRecord);
            Assert.AreEqual(4, received.BestWave);
        }

        [Test]
        public void AfterGameOver_TimeStops()
        {
            session.Tick(3f);
            playerHealth.TakeDamage(1, now: 0f);

            session.Tick(10f);

            Assert.AreEqual(3f, stats.SurvivedTime, 0.0001f);
        }

        [Test]
        public void WaveBelowRecord_IsNotNewRecord()
        {
            record.Submit(10);
            stats.ReachWave(4);

            playerHealth.TakeDamage(1, now: 0f);

            Assert.IsFalse(session.Result.IsNewRecord);
            Assert.AreEqual(10, session.Result.BestWave);
        }
    }
}
