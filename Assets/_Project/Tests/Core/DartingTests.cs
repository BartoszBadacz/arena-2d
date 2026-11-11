using System;
using System.Numerics;
using NUnit.Framework;

namespace Arena.Core.Tests
{
    public class DartingTests
    {
        private const float Tolerance = 0.0001f;
        private static readonly Vector2 Forward = new Vector2(0f, 5f); // leci w górę z prędkością 5

        // Stała przerwa 0.5 s (min = max) — przewidywalny rozkład skoków w teście.
        private static Darting NewDarting() => new Darting(
            new DartSettings { IntervalMin = 0.5f, IntervalMax = 0.5f, Duration = 0.1f, SpeedMultiplier = 2f },
            new Random(1));

        [Test]
        public void BeforeFirstDart_VelocityUnchanged()
        {
            var darting = NewDarting();

            Assert.AreEqual(Forward, darting.Apply(Forward, now: 0f));
            Assert.AreEqual(Forward, darting.Apply(Forward, now: 0.4f));
        }

        [Test]
        public void DuringDart_MovesSidewaysAndFaster()
        {
            var darting = NewDarting();
            darting.Apply(Forward, now: 0f);

            Vector2 v = darting.Apply(Forward, now: 0.5f);

            Assert.IsTrue(darting.IsDarting(0.5f));
            Assert.Greater(Math.Abs(v.X), Math.Abs(v.Y)); // bardziej w bok niż do przodu
            Assert.Greater(v.Y, 0f);                       // ale nie zawraca
            Assert.AreEqual(10f, v.Length(), Tolerance);   // 5 × 2
        }

        [Test]
        public void AfterDart_BackOnCourse()
        {
            var darting = NewDarting();
            darting.Apply(Forward, now: 0f);
            darting.Apply(Forward, now: 0.5f);

            Assert.AreEqual(Forward, darting.Apply(Forward, now: 0.65f));
        }

        [Test]
        public void DartsRepeat()
        {
            var darting = NewDarting();
            darting.Apply(Forward, now: 0f);
            darting.Apply(Forward, now: 0.5f);
            darting.Apply(Forward, now: 0.65f);

            Assert.AreNotEqual(Forward, darting.Apply(Forward, now: 1.0f));
        }

        [Test]
        public void StandingStill_StaysStill()
        {
            var darting = NewDarting();
            darting.Apply(Vector2.Zero, now: 0f);

            Assert.AreEqual(Vector2.Zero, darting.Apply(Vector2.Zero, now: 0.5f));
        }
    }
}
