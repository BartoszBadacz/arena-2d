using System;
using System.Collections.Generic;

namespace Arena.Core
{
    // Skład fali: na starcie fali losuje rodzaj wroga dla każdego miejsca, potem tasuje kolejność.
    // Domieszka zastępuje podstawowego wroga, więc łączna liczba wrogów zostaje z krzywej fal.
    // Reguły sprawdzane po kolei — dla danego miejsca wygrywa pierwsza aktywna, która wylosowała swoją szansę.
    public sealed class EnemyMixer<TKind>
    {
        private readonly TKind baseKind;
        private readonly IReadOnlyList<MixInRule<TKind>> rules;
        private readonly Random random;
        private readonly int[] usedThisWave;
        private readonly List<TKind> composition = new List<TKind>();
        private int nextIndex;

        public EnemyMixer(TKind baseKind, IReadOnlyList<MixInRule<TKind>> rules, Random random)
        {
            this.baseKind = baseKind;
            this.rules = rules;
            this.random = random;
            usedThisWave = new int[rules.Count];
        }

        public IReadOnlyList<TKind> Composition => composition;

        public void StartWave(int wave, int size)
        {
            Array.Clear(usedThisWave, 0, usedThisWave.Length);
            composition.Clear();
            nextIndex = 0;

            for (int slot = 0; slot < size; slot++) composition.Add(PickKind(wave));
            Shuffle();
        }

        // Kolejny wróg z bieżącego składu; gdyby zabrakło — podstawowy.
        public TKind Next() => nextIndex < composition.Count ? composition[nextIndex++] : baseKind;

        private TKind PickKind(int wave)
        {
            for (int i = 0; i < rules.Count; i++)
            {
                MixInRule<TKind> rule = rules[i];
                int max = rule.MaxFor(wave);
                bool active = wave >= rule.FromWave && (max == 0 || usedThisWave[i] < max);
                if (!active || random.NextDouble() >= rule.Chance) continue;

                usedThisWave[i]++;
                return rule.Kind;
            }
            return baseKind;
        }

        // Fisher–Yates: bez tasowania tank z regułą „1 na falę” zawsze wychodziłby pierwszy.
        private void Shuffle()
        {
            for (int i = composition.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (composition[i], composition[j]) = (composition[j], composition[i]);
            }
        }
    }
}
