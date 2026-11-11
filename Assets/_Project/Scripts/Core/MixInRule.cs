using System;

namespace Arena.Core
{
    // Reguła domieszki: „od fali FromWave, szansa Chance na każde miejsce w fali, najwyżej MaxPerWave na falę”.
    // TKind to rodzaj wroga — Core nie wie, czym on jest (w grze EnemyDefinition, w testach np. string).
    public sealed class MixInRule<TKind>
    {
        public MixInRule(TKind kind, int fromWave, float chance, int maxPerWave = 0)
        {
            if (chance < 0f || chance > 1f) throw new ArgumentOutOfRangeException(nameof(chance), chance, "Szansa od 0 do 1.");
            Kind = kind;
            FromWave = fromWave;
            Chance = chance;
            MaxPerWave = maxPerWave;
        }

        public TKind Kind { get; }
        public int FromWave { get; }
        public float Chance { get; }
        public int MaxPerWave { get; } // 0 = bez limitu
    }
}
