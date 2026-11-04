using System;
using UnityEngine;

// Prototyp: fale „do wybicia”. Fala = określona liczba wrogów; kończy się, gdy zginie ostatni,
// potem krótka przerwa i następna, trudniejsza fala. Prosta maszyna stanów: Break → Spawning → Fighting → Break.
public class EnemySpawner : MonoBehaviour
{
    public enum State { Break, Spawning, Fighting }

    [SerializeField] private Enemy enemyPrefab;
    [SerializeField] private Transform player;
    [SerializeField] private Vector2 arenaHalfSize = new Vector2(13f, 7f);
    [SerializeField] private float minDistanceFromPlayer = 6f;

    [Header("Fale")]
    [SerializeField] private float breakDuration = 3f;
    [SerializeField] private int firstWaveCount = 5;
    [SerializeField] private int extraEnemiesPerWave = 3;
    [SerializeField] private float startInterval = 1.2f;
    [SerializeField] private float intervalMultiplierPerWave = 0.85f;
    [SerializeField] private float minInterval = 0.2f;
    [SerializeField] private float speedBonusPerWave = 0.3f;

    public event Action<int> WaveStarted;
    public event Action<int> WaveCleared;

    public State CurrentState { get; private set; } = State.Break;
    public int Wave { get; private set; }                  // 0 = jeszcze przed pierwszą falą
    public int EnemiesRemaining => toSpawn + alive;        // do pojawienia się + żywi
    public float BreakTimeLeft => Mathf.Max(0f, stateEndsAt - Time.timeSinceLevelLoad);

    private int toSpawn;
    private int alive;
    private float stateEndsAt;
    private float nextSpawnTime;

    private void OnEnable() => Enemy.Killed += OnEnemyKilled;
    private void OnDisable() => Enemy.Killed -= OnEnemyKilled;

    private void Start() => stateEndsAt = Time.timeSinceLevelLoad + breakDuration;

    private void Update()
    {
        float now = Time.timeSinceLevelLoad;

        switch (CurrentState)
        {
            case State.Break:
                if (now >= stateEndsAt) StartNextWave();
                break;

            case State.Spawning:
                if (now < nextSpawnTime) break;
                nextSpawnTime = now + CurrentInterval();
                SpawnEnemy();
                if (toSpawn == 0) CurrentState = State.Fighting;
                break;

            case State.Fighting:
                // Koniec fali sprawdzany w OnEnemyKilled.
                break;
        }
    }

    private void StartNextWave()
    {
        Wave++;
        toSpawn = firstWaveCount + extraEnemiesPerWave * (Wave - 1);
        nextSpawnTime = Time.timeSinceLevelLoad;
        CurrentState = State.Spawning;
        WaveStarted?.Invoke(Wave);
    }

    private void OnEnemyKilled()
    {
        alive--;
        if (CurrentState == State.Fighting && alive == 0)
        {
            CurrentState = State.Break;
            stateEndsAt = Time.timeSinceLevelLoad + breakDuration;
            WaveCleared?.Invoke(Wave);
        }
    }

    private void SpawnEnemy()
    {
        Enemy enemy = Instantiate(enemyPrefab, SafeSpawnPoint(), Quaternion.identity);
        enemy.AddSpeed(speedBonusPerWave * (Wave - 1));
        toSpawn--;
        alive++;
    }

    private float CurrentInterval()
    {
        float interval = startInterval * Mathf.Pow(intervalMultiplierPerWave, Wave - 1);
        return Mathf.Max(interval, minInterval);
    }

    // Losujemy punkt na krawędzi, aż trafimy dość daleko od gracza (z limitem prób).
    private Vector2 SafeSpawnPoint()
    {
        Vector2 point = RandomEdgePoint();
        for (int attempt = 0; attempt < 20; attempt++)
        {
            if (Vector2.Distance(point, player.position) >= minDistanceFromPlayer) return point;
            point = RandomEdgePoint();
        }
        // Awaryjnie: punkt po przeciwnej stronie areny niż gracz.
        return -(Vector2)player.position.normalized * arenaHalfSize;
    }

    private Vector2 RandomEdgePoint()
    {
        float x = UnityEngine.Random.Range(-arenaHalfSize.x, arenaHalfSize.x);
        float y = UnityEngine.Random.Range(-arenaHalfSize.y, arenaHalfSize.y);
        // Przyklejamy losowy punkt do jednej z czterech krawędzi.
        switch (UnityEngine.Random.Range(0, 4))
        {
            case 0: return new Vector2(x, arenaHalfSize.y);
            case 1: return new Vector2(x, -arenaHalfSize.y);
            case 2: return new Vector2(arenaHalfSize.x, y);
            default: return new Vector2(-arenaHalfSize.x, y);
        }
    }
}
