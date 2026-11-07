using System;

namespace Arena.Core
{
    // Życie: wspólne dla gracza i wrogów. Opcjonalne okno nietykalności po trafieniu (wrogowie mają 0).
    // Stan (Current, IsDead) się czyta; fakty (Damaged, Died) ogłaszają zdarzenia.
    public sealed class Health
    {
        private readonly float invulnerabilityDuration;
        private float invulnerableUntil = float.NegativeInfinity;

        public Health(int max, float invulnerabilityDuration = 0f)
        {
            if (max <= 0) throw new ArgumentOutOfRangeException(nameof(max), max, "Musi być dodatnie.");
            Max = max;
            Current = max;
            this.invulnerabilityDuration = invulnerabilityDuration;
        }

        public event Action Damaged;
        public event Action Died;

        public int Max { get; }
        public int Current { get; private set; }
        public bool IsDead => Current <= 0;
        public bool IsInvulnerable(float now) => now < invulnerableUntil;

        // true = obrażenia przyjęte. Martwy albo nietykalny ich nie przyjmuje
        // (np. kilka pocisków w jednej klatce nie zabija dwa razy).
        public bool TakeDamage(int amount, float now)
        {
            if (amount <= 0 || IsDead || IsInvulnerable(now)) return false;

            Current = Math.Max(0, Current - amount);
            invulnerableUntil = now + invulnerabilityDuration;
            Damaged?.Invoke();
            if (IsDead) Died?.Invoke();
            return true;
        }
    }
}
