using System;

namespace Arena.Core
{
    // Reguła domieszki: „od fali FromWave, szansa Chance na każde miejsce w fali, najwyżej MaxPerWave na falę”;
    // limit może rosnąć o 1 co MaxGrowthEveryWaves fal (0 = stały).
    // TKind to rodzaj wroga — Core nie wie, czym on jest (w grze EnemyDefinition, w testach np. string).
    public sealed class MixInRule<TKind>
    {
        public MixInRule(TKind kind, int fromWave, float chance, int maxPerWave = 0, int maxGrowthEveryWaves = 0)
        {
            if (chance < 0f || chance > 1f) throw new ArgumentOutOfRangeException(nameof(chance), chance, "Szansa od 0 do 1.");
            Kind = kind;
            FromWave = fromWave;
            Chance = chance;
            MaxPerWave = maxPerWave;
            MaxGrowthEveryWaves = maxGrowthEveryWaves;
        }

        public TKind Kind { get; }
        public int FromWave { get; }
        public float Chance { get; }
        public int MaxPerWave { get; } // 0 = bez limitu
        public int MaxGrowthEveryWaves { get; } // 0 = limit stały

        // Limit dla danej fali; 0 = bez limitu.
        public int MaxFor(int wave)
        {
            if (MaxPerWave == 0) return 0;
            if (MaxGrowthEveryWaves <= 0 || wave <= FromWave) return MaxPerWave;
            return MaxPerWave + (wave - FromWave) / MaxGrowthEveryWaves;
        }
    }
}
