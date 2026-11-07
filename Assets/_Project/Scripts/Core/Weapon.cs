using System;

namespace Arena.Core
{
    // Kadencja broni: strzał nie częściej niż shotsPerSecond razy na sekundę.
    // Nie zna Unity ani Time.time — czas dostaje jako argument, więc w testach przesuwamy go ręcznie.
    public sealed class Weapon
    {
        private readonly float cooldown;
        private float nextShotTime = float.NegativeInfinity;

        public Weapon(float shotsPerSecond)
        {
            if (shotsPerSecond <= 0f)
                throw new ArgumentOutOfRangeException(nameof(shotsPerSecond), shotsPerSecond, "Musi być dodatnie.");
            cooldown = 1f / shotsPerSecond;
        }

        // true = strzał wolno oddać teraz (i od tej chwili liczy się przerwa do następnego).
        public bool TryFire(float now)
        {
            if (now < nextShotTime) return false;
            nextShotTime = now + cooldown;
            return true;
        }
    }
}
