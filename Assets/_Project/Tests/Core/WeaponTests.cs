using System;
using NUnit.Framework;

namespace Arena.Core.Tests
{
    public class WeaponTests
    {
        [Test]
        public void FirstShot_IsAllowedImmediately()
        {
            var weapon = new Weapon(shotsPerSecond: 4f);

            Assert.IsTrue(weapon.TryFire(now: 0f));
        }

        [Test]
        public void ShotWithinCooldown_IsRejected()
        {
            var weapon = new Weapon(shotsPerSecond: 4f); // przerwa 0.25 s
            weapon.TryFire(now: 0f);

            Assert.IsFalse(weapon.TryFire(now: 0.2f));
        }

        [Test]
        public void ShotAfterCooldown_IsAllowed()
        {
            var weapon = new Weapon(shotsPerSecond: 4f);
            weapon.TryFire(now: 0f);

            Assert.IsTrue(weapon.TryFire(now: 0.25f));
        }

        [Test]
        public void RejectedAttempt_DoesNotDelayNextShot()
        {
            var weapon = new Weapon(shotsPerSecond: 4f);
            weapon.TryFire(now: 0f);
            weapon.TryFire(now: 0.2f); // odrzucony — nie może przesunąć przerwy

            Assert.IsTrue(weapon.TryFire(now: 0.25f));
        }

        [Test]
        public void NonPositiveRate_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Weapon(shotsPerSecond: 0f));
        }

        [Test]
        public void Fired_OnlyForShotsActuallyTaken()
        {
            var weapon = new Weapon(shotsPerSecond: 4f);
            int fired = 0;
            weapon.Fired += () => fired++;

            weapon.TryFire(now: 0f);    // strzał
            weapon.TryFire(now: 0.1f);  // za wcześnie — bez zdarzenia
            weapon.TryFire(now: 0.25f); // strzał

            Assert.AreEqual(2, fired);
        }
    }
}
