using System;
using System.Linq;
using NUnit.Framework;

namespace Arena.Core.Tests
{
    public class EnemyMixerTests
    {
        private const string Slime = "slime";
        private const string Runner = "runner";
        private const string Tank = "tank";

        private static EnemyMixer<string> Mixer(params MixInRule<string>[] rules) =>
            new EnemyMixer<string>(Slime, rules, new Random(7));

        private static int Count(EnemyMixer<string> mixer, string kind) => mixer.Composition.Count(k => k == kind);

        [Test]
        public void WithoutRules_AllBase()
        {
            var mixer = Mixer();

            mixer.StartWave(wave: 5, size: 10);

            Assert.AreEqual(10, mixer.Composition.Count);
            Assert.AreEqual(10, Count(mixer, Slime));
        }

        [Test]
        public void BeforeFromWave_RuleIsInactive()
        {
            var mixer = Mixer(new MixInRule<string>(Tank, fromWave: 5, chance: 1f));

            mixer.StartWave(wave: 4, size: 10);

            Assert.AreEqual(0, Count(mixer, Tank));
        }

        [Test]
        public void SureChanceWithMaxOne_GivesExactlyOne()
        {
            var mixer = Mixer(new MixInRule<string>(Tank, fromWave: 5, chance: 1f, maxPerWave: 1));

            mixer.StartWave(wave: 5, size: 22);

            Assert.AreEqual(1, Count(mixer, Tank));
            Assert.AreEqual(21, Count(mixer, Slime));
        }

        [Test]
        public void LimitResetsEveryWave()
        {
            var mixer = Mixer(new MixInRule<string>(Tank, fromWave: 1, chance: 1f, maxPerWave: 2));

            mixer.StartWave(wave: 1, size: 10);
            mixer.StartWave(wave: 2, size: 10);

            Assert.AreEqual(2, Count(mixer, Tank));
        }

        [Test]
        public void ZeroChance_NeverMixes()
        {
            var mixer = Mixer(new MixInRule<string>(Runner, fromWave: 1, chance: 0f));

            mixer.StartWave(wave: 10, size: 100);

            Assert.AreEqual(0, Count(mixer, Runner));
        }

        [Test]
        public void Chance_GivesRoughlyThatShare()
        {
            var mixer = Mixer(new MixInRule<string>(Runner, fromWave: 1, chance: 0.15f));

            mixer.StartWave(wave: 3, size: 10000);

            Assert.AreEqual(0.15, Count(mixer, Runner) / 10000.0, 0.02);
        }

        [Test]
        public void EarlierRuleHasPriority_LaterFillsAfterItsLimit()
        {
            var mixer = Mixer(
                new MixInRule<string>(Tank, fromWave: 1, chance: 1f, maxPerWave: 1),
                new MixInRule<string>(Runner, fromWave: 1, chance: 1f));

            mixer.StartWave(wave: 1, size: 6);

            Assert.AreEqual(1, Count(mixer, Tank));
            Assert.AreEqual(5, Count(mixer, Runner));
        }

        [Test]
        public void Next_WalksCompositionThenFallsBackToBase()
        {
            var mixer = Mixer(new MixInRule<string>(Tank, fromWave: 1, chance: 1f));
            mixer.StartWave(wave: 1, size: 2);

            Assert.AreEqual(Tank, mixer.Next());
            Assert.AreEqual(Tank, mixer.Next());
            Assert.AreEqual(Slime, mixer.Next());
        }

        [Test]
        public void Shuffle_SingleTankIsNotAlwaysFirst()
        {
            var mixer = Mixer(new MixInRule<string>(Tank, fromWave: 1, chance: 1f, maxPerWave: 1));
            int tankFirst = 0;

            for (int wave = 1; wave <= 50; wave++)
            {
                mixer.StartWave(wave, size: 10);
                if (mixer.Composition[0] == Tank) tankFirst++;
            }

            Assert.Less(tankFirst, 50);
        }

        [Test]
        public void ChanceOutsideRange_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new MixInRule<string>(Tank, fromWave: 1, chance: 1.5f));
        }
    }
}
