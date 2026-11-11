using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Arena.Core.Tests
{
    public class TargetSwitcherTests
    {
        private const float Interval = 2f;

        private TargetSwitcher<string> switcher;
        private List<string> candidates;
        private HashSet<string> dead;

        [SetUp]
        public void SetUp()
        {
            switcher = new TargetSwitcher<string>(Interval, new Random(3));
            candidates = new List<string> { "a", "b", "c" };
            dead = new HashSet<string>();
        }

        private string Update(float now) => switcher.Update(now, candidates, c => !dead.Contains(c));

        [Test]
        public void PicksSomeCandidate()
        {
            Assert.Contains(Update(now: 0f), candidates);
        }

        [Test]
        public void KeepsTargetUntilIntervalPasses()
        {
            string first = Update(now: 0f);

            Assert.AreEqual(first, Update(now: 1f));
            Assert.AreEqual(first, Update(now: 1.9f));
        }

        [Test]
        public void AfterInterval_SwitchesToADifferentTarget()
        {
            string first = Update(now: 0f);

            string second = Update(now: Interval);

            Assert.AreNotEqual(first, second);
            Assert.Contains(second, candidates);
        }

        [Test]
        public void WhenTargetDies_SwitchesImmediately()
        {
            string first = Update(now: 0f);
            dead.Add(first);

            string second = Update(now: 0.1f);

            Assert.AreNotEqual(first, second);
            Assert.IsFalse(dead.Contains(second));
        }

        [Test]
        public void SingleCandidate_IsKeptAfterInterval()
        {
            candidates = new List<string> { "only" };
            Update(now: 0f);

            Assert.AreEqual("only", Update(now: Interval));
        }

        [Test]
        public void NoValidCandidates_GivesNull()
        {
            dead.UnionWith(candidates);

            Assert.IsNull(Update(now: 0f));
        }
    }
}
