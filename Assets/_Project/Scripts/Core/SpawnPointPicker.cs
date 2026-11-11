using System;
using System.Numerics;

namespace Arena.Core
{
    // Bezpieczny punkt spawnu: losowy punkt na krawędzi areny, co najmniej minDistance od gracza.
    public sealed class SpawnPointPicker
    {
        private const int MaxAttempts = 20;

        private readonly Vector2 halfSize;
        private readonly float minDistance;
        private readonly Random random;

        public SpawnPointPicker(Vector2 arenaHalfSize, float minDistanceFromPlayer, Random random)
        {
            halfSize = arenaHalfSize;
            minDistance = minDistanceFromPlayer;
            this.random = random;
        }

        public Vector2 Pick(Vector2 player)
        {
            for (int attempt = 0; attempt < MaxAttempts; attempt++)
            {
                Vector2 point = RandomEdgePoint();
                if (Vector2.Distance(point, player) >= minDistance) return point;
            }
            return OppositeCorner(player);
        }

        // Awaryjnie (pech w losowaniu): róg areny po przeciwnej stronie niż gracz — najdalszy możliwy punkt.
        private Vector2 OppositeCorner(Vector2 player) =>
            new Vector2(player.X > 0f ? -halfSize.X : halfSize.X, player.Y > 0f ? -halfSize.Y : halfSize.Y);

        // Losowy punkt w prostokącie areny „przyklejony” do jednej z czterech krawędzi.
        private Vector2 RandomEdgePoint()
        {
            float x = Range(-halfSize.X, halfSize.X);
            float y = Range(-halfSize.Y, halfSize.Y);
            switch (random.Next(4))
            {
                case 0: return new Vector2(x, halfSize.Y);
                case 1: return new Vector2(x, -halfSize.Y);
                case 2: return new Vector2(halfSize.X, y);
                default: return new Vector2(-halfSize.X, y);
            }
        }

        private float Range(float min, float max) => min + (float)random.NextDouble() * (max - min);
    }
}
