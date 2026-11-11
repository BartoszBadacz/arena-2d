using System;
using System.Collections.Generic;

namespace Arena.Core
{
    // Losowy cel zmieniany co jakiś czas: trzyma wybrany cel przez `interval` sekund, potem losuje inny.
    // Gdy cel przestaje się nadawać (np. zginął), losuje od razu. Brak kandydatów = brak celu (null).
    // T to dowolny obiekt — Core nie wie, że w grze to wróg.
    public sealed class TargetSwitcher<T> where T : class
    {
        private readonly float interval;
        private readonly Random random;
        private float nextSwitchTime = float.NegativeInfinity;

        public TargetSwitcher(float interval, Random random)
        {
            this.interval = interval;
            this.random = random;
        }

        public T Current { get; private set; }

        public T Update(float now, IReadOnlyList<T> candidates, Func<T, bool> isValid)
        {
            bool currentValid = Current != null && isValid(Current);
            if (currentValid && now < nextSwitchTime) return Current;

            // Nowy cel: inny niż obecny, jeśli jest z czego wybierać.
            T previous = currentValid ? Current : null;
            int count = CountChoices(candidates, isValid, previous);
            if (count == 0)
            {
                if (!currentValid) Current = null;
                return Current;
            }

            Current = Nth(candidates, isValid, previous, random.Next(count));
            nextSwitchTime = now + interval;
            return Current;
        }

        private static int CountChoices(IReadOnlyList<T> candidates, Func<T, bool> isValid, T excluded)
        {
            int count = 0;
            foreach (T candidate in candidates)
                if (candidate != excluded && isValid(candidate)) count++;
            return count;
        }

        private static T Nth(IReadOnlyList<T> candidates, Func<T, bool> isValid, T excluded, int n)
        {
            foreach (T candidate in candidates)
            {
                if (candidate == excluded || !isValid(candidate)) continue;
                if (n == 0) return candidate;
                n--;
            }
            return null;
        }
    }
}
