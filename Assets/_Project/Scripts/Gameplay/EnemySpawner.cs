using System;
using System.Collections.Generic;
using Arena.Core;
using Arena.Data;
using UnityEngine;

namespace Arena.Gameplay
{
    // Klej fal: podaje WaveDirector (Core) upływ czasu, na jego prośbę (SpawnRequested) tworzy wrogów
    // w bezpiecznych punktach (SpawnPointPicker) i melduje mu każdą śmierć. Jakiego typu — mówi EnemyMixer (Core).
    public class EnemySpawner : MonoBehaviour
    {
        // Wrogowie powstają w trakcie gry, więc nikt nie podepnie się do każdego z osobna.
        // Spawner słucha śmierci każdego, kogo stworzył, i ogłasza jedno wspólne zdarzenie (jak rodzic zbierający @Output dzieci).
        public event Action<EnemyDefinition> EnemyKilled;

        private WaveDirector director;
        private EnemyMixer<EnemyDefinition> mixer;
        private SpawnPointPicker spawnPoints;
        private Transform target;
        private EnemyContext context;
        private readonly List<EnemyAgent> alive = new(); // żywi wrogowie — wśród nich chowają się HideInCrowd

        public void Init(WaveDirector director, EnemyMixer<EnemyDefinition> mixer, SpawnPointPicker spawnPoints,
                         Transform target, System.Random random, System.Numerics.Vector2 arenaHalfSize)
        {
            this.director = director;
            this.mixer = mixer;
            this.spawnPoints = spawnPoints;
            this.target = target;
            context = new EnemyContext(target, random, arenaHalfSize, alive);
            director.SpawnRequested += SpawnBatch;
        }

        private void Start()
        {
            if (director != null) return;
            Debug.LogError($"{nameof(EnemySpawner)}: brak Init — czy w scenie jest GameBootstrap?", this);
            enabled = false;
        }

        private void OnDestroy()
        {
            if (director != null) director.SpawnRequested -= SpawnBatch;
        }

        private void Update() => director.Tick(Time.deltaTime);

        // Każdy wróg z paczki w innym, losowym punkcie na krawędzi.
        private void SpawnBatch(int count)
        {
            Vector2 player = target.position;
            for (int i = 0; i < count; i++) Spawn(mixer.Next(), spawnPoints.Pick(player.ToNumerics()).ToUnity());
        }

        private void Spawn(EnemyDefinition enemy, Vector2 position)
        {
            GameObject instance = Instantiate(enemy.Prefab, position, Quaternion.identity);
            if (!instance.TryGetComponent(out EnemyAgent agent))
            {
                Debug.LogError($"{enemy.name}: prefab wroga nie ma komponentu {nameof(EnemyAgent)}.", enemy);
                Destroy(instance);
                director.EnemyDied(); // fala liczy go jako żywego — inaczej nigdy by się nie skończyła
                return;
            }
            agent.Init(enemy, context, director.SpeedBonus);
            alive.Add(agent);
            agent.Health.Died += () =>
            {
                alive.Remove(agent);
                director.EnemyDied();
                EnemyKilled?.Invoke(enemy);
            };
        }
    }
}
