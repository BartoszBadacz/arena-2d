using Arena.Data;
using UnityEngine;

namespace Arena.Gameplay
{
    // TYMCZASOWY (krok 3 przepisania): jeden wróg co 2 s w losowym rogu areny.
    // W kroku 5 o tym, kiedy i ilu wrogów, zdecyduje WaveDirector (Core), a liczby przyjdą z WaveConfig.
    public class EnemySpawner : MonoBehaviour
    {
        private const float Interval = 2f;
        private static readonly Vector2[] Corners = { new(-12f, 6f), new(12f, 6f), new(-12f, -6f), new(12f, -6f) };

        private EnemyDefinition enemy;
        private Transform target;
        private System.Random random;
        private float nextSpawnTime;

        public void Init(EnemyDefinition enemy, Transform target, System.Random random)
        {
            this.enemy = enemy;
            this.target = target;
            this.random = random;
        }

        private void Start()
        {
            if (enemy != null) return;
            Debug.LogError($"{nameof(EnemySpawner)}: brak Init — czy w scenie jest GameBootstrap?", this);
            enabled = false;
        }

        private void Update()
        {
            if (Time.time < nextSpawnTime) return;
            nextSpawnTime = Time.time + Interval;
            Spawn(Corners[random.Next(Corners.Length)]);
        }

        private void Spawn(Vector2 position)
        {
            GameObject instance = Instantiate(enemy.Prefab, position, Quaternion.identity);
            if (!instance.TryGetComponent(out EnemyAgent agent))
            {
                Debug.LogError($"{enemy.name}: prefab wroga nie ma komponentu {nameof(EnemyAgent)}.", enemy);
                Destroy(instance);
                return;
            }
            agent.Init(enemy, target, random);
        }
    }
}
