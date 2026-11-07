using System;
using NUnit.Framework;

namespace Arena.Core.Tests
{
    public class HealthTests
    {
        [Test]
        public void StartsFull()
        {
            var health = new Health(max: 3);

            Assert.AreEqual(3, health.Current);
            Assert.IsFalse(health.IsDead);
        }

        [Test]
        public void Damage_ReducesCurrentAndRaisesDamaged()
        {
            var health = new Health(max: 3);
            int damagedCount = 0;
            health.Damaged += () => damagedCount++;

            health.TakeDamage(1, now: 0f);

            Assert.AreEqual(2, health.Current);
            Assert.AreEqual(1, damagedCount);
        }

        [Test]
        public void LethalDamage_RaisesDiedOnce()
        {
            var health = new Health(max: 2);
            int diedCount = 0;
            health.Died += () => diedCount++;

            health.TakeDamage(1, now: 0f);
            health.TakeDamage(5, now: 0f);
            health.TakeDamage(1, now: 0f); // już martwy — np. drugi pocisk w tej samej klatce

            Assert.IsTrue(health.IsDead);
            Assert.AreEqual(0, health.Current);
            Assert.AreEqual(1, diedCount);
        }

        [Test]
        public void Invulnerability_BlocksDamageInsideWindowOnly()
        {
            var health = new Health(max: 5, invulnerabilityDuration: 1f);

            Assert.IsTrue(health.TakeDamage(1, now: 0f));
            Assert.IsFalse(health.TakeDamage(1, now: 0.5f));
            Assert.IsTrue(health.TakeDamage(1, now: 1f));
            Assert.AreEqual(3, health.Current);
        }

        [Test]
        public void NonPositiveDamage_IsIgnored()
        {
            var health = new Health(max: 3);

            Assert.IsFalse(health.TakeDamage(0, now: 0f));
            Assert.AreEqual(3, health.Current);
        }

        [Test]
        public void NonPositiveMax_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Health(max: 0));
        }
    }
}
