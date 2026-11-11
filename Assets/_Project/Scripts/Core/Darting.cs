using System;
using System.Numerics;

namespace Arena.Core
{
    // Ustawienia skoków w bok. Tworzy je EnemyDefinition.ToDartSettings() (Data).
    public sealed class DartSettings
    {
        public float IntervalMin { get; set; } = 0.3f;  // przerwa między skokami: losowo od..do (sekundy)
        public float IntervalMax { get; set; } = 0.8f;
        public float Duration { get; set; } = 0.15f;    // jak długo trwa jeden skok
        public float SpeedMultiplier { get; set; } = 1.8f;
    }

    // Skoki w bok jak u muchy: co losowy czas krótki zryw w lewo albo w prawo względem kierunku ruchu, szybciej niż zwykle,
    // potem powrót na kurs. Modyfikuje prędkość policzoną przez Steering; czas dostaje jako argument.
    public sealed class Darting
    {
        private const float ForwardShare = 0.35f; // ile „do przodu” zostaje w skoku — skos zamiast czystego bokiem
        private const float Epsilon = 0.001f;

        private readonly DartSettings settings;
        private readonly Random random;
        private float nextDartAt = float.NaN;
        private float dartUntil = float.NegativeInfinity;
        private int side;

        public Darting(DartSettings settings, Random random)
        {
            this.settings = settings;
            this.random = random;
        }

        public bool IsDarting(float now) => now < dartUntil;

        public Vector2 Apply(Vector2 velocity, float now)
        {
            if (float.IsNaN(nextDartAt)) nextDartAt = now + NextInterval(); // pierwszy skok nie od razu po pojawieniu się
            if (now >= nextDartAt)
            {
                side = random.Next(2) == 0 ? -1 : 1;
                dartUntil = now + settings.Duration;
                nextDartAt = now + NextInterval();
            }
            if (!IsDarting(now)) return velocity;

            float speed = velocity.Length();
            if (speed < Epsilon) return velocity; // stoi (np. w rogu) — nie ma względem czego skakać w bok

            Vector2 forward = velocity / speed;
            Vector2 sideways = new Vector2(-forward.Y, forward.X) * side;
            return Vector2.Normalize(forward * ForwardShare + sideways) * speed * settings.SpeedMultiplier;
        }

        private float NextInterval() =>
            settings.IntervalMin + (float)random.NextDouble() * Math.Max(0f, settings.IntervalMax - settings.IntervalMin);
    }
}
