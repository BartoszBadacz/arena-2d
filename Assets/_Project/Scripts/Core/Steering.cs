using System;
using System.Numerics;

namespace Arena.Core
{
    // Ruch wroga jako czysta matematyka: „do celu” + „od sąsiadów” (separacja z boidów).
    // Wektory z System.Numerics, nie z Unity — Gameplay przelicza je na granicy.
    public static class Steering
    {
        private const float Epsilon = 0.001f;

        // Suma wektorów „od sąsiada”: im bliżej sąsiad, tym mocniej odpycha; za promieniem nie działa.
        public static Vector2 Separation(Vector2 self, ReadOnlySpan<Vector2> neighbours, float radius, Random random)
        {
            Vector2 push = Vector2.Zero;
            foreach (Vector2 other in neighbours)
            {
                Vector2 away = self - other;
                float distance = away.Length();
                if (distance >= radius) continue;

                // Idealnie na sobie: kierunek losowy, żeby dwa wrogi mogły się rozejść.
                Vector2 direction = distance < Epsilon ? RandomDirection(random) : away / distance;
                push += direction * (1f - distance / radius);
            }
            return push;
        }

        // Prędkość: kierunek do celu skorygowany separacją, zawsze o długości speed (albo zero).
        public static Vector2 Chase(Vector2 self, Vector2 target, Vector2 separation, float separationStrength, float speed)
        {
            Vector2 toTarget = NormalizeOrZero(target - self);
            return NormalizeOrZero(toTarget + separation * separationStrength) * speed;
        }

        // Vector2.Normalize(zero) daje NaN — a wróg stojący dokładnie na graczu to normalna sytuacja.
        private static Vector2 NormalizeOrZero(Vector2 v)
        {
            float length = v.Length();
            return length < Epsilon ? Vector2.Zero : v / length;
        }

        private static Vector2 RandomDirection(Random random)
        {
            double angle = random.NextDouble() * 2.0 * Math.PI;
            return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }
    }
}
