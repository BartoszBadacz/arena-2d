using System.Collections.Generic;
using UnityEngine;

namespace Arena.Gameplay
{
    // Wspólne otoczenie wszystkich wrogów: kogo gonią, granice areny, losowość i lista żywych (do chowania się w tłumie).
    // Spawner tworzy je raz i podaje każdemu wrogowi w Init — zamiast pięciu osobnych parametrów.
    public sealed class EnemyContext
    {
        public EnemyContext(Transform player, System.Random random, System.Numerics.Vector2 arenaHalfSize,
                            IReadOnlyList<EnemyAgent> alive)
        {
            Player = player;
            Random = random;
            ArenaHalfSize = arenaHalfSize;
            Alive = alive;
        }

        public Transform Player { get; }
        public System.Random Random { get; }
        public System.Numerics.Vector2 ArenaHalfSize { get; }
        public IReadOnlyList<EnemyAgent> Alive { get; }
    }
}
