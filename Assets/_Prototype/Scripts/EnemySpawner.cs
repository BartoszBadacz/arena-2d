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
    [SerializeField] private int firstWaveCount = 6;
    [SerializeField] private int extraEnemiesPerWave = 4;
    [SerializeField] private float startInterval = 1.5f;
    [SerializeField] private float intervalMultiplierPerWave = 0.9f;
    [SerializeField] private float minInterval = 0.4f;

    [Header("Kilku wrogów naraz, każdy w innym miejscu")]
    [SerializeField] private int wavesPerBatchGrowth = 2;   // liczba wrogów na raz rośnie o 1 co tyle fal
    [SerializeField] private int maxBatchSize = 4;

    [Header("Szybkość: skokowo co kilka fal")]
    [SerializeField] private int wavesPerSpeedUp = 3;
    [SerializeField] private float speedBonusPerSpeedUp = 0.6f;

    [Header("Testy")]
    [Tooltip("Od której fali zaczynać — do strojenia późniejszych fal bez grania od początku.")]
    [SerializeField] private int startWave = 1;

    public event Action<int> WaveStarted;
    public event Action<int> WaveCleared;

    public State CurrentState { get; private set; } = State.Break;
    public int Wave { get; private set; }                  // 0 = jeszcze przed pierwszą falą
    public int EnemiesRemaining => toSpawn + alive;        // do pojawienia się + żywi
    public float BreakTimeLeft => Mathf.Max(0f, stateEndsAt - Time.timeSinceLevelLoad);
    public bool IsSpeedUpWave => Wave > 1 && (Wave - 1) % wavesPerSpeedUp == 0;
    private int BatchSize => Mathf.Min(1 + (Wave - 1) / wavesPerBatchGrowth, maxBatchSize);
    private float SpeedBonus => (Wave - 1) / wavesPerSpeedUp * speedBonusPerSpeedUp;

    private int toSpawn;
    private int alive;
    private float stateEndsAt;
    private float nextSpawnTime;

    private void OnEnable() => Enemy.Killed += OnEnemyKilled;
    private void OnDisable() => Enemy.Killed -= OnEnemyKilled;

    private void Start()
    {
        Wave = Mathf.Max(0, startWave - 1);
        stateEndsAt = Time.timeSinceLevelLoad + breakDuration;
    }

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
                SpawnBatch();
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

    private void SpawnBatch()
    {
        int count = Mathf.Min(BatchSize, toSpawn);
        for (int i = 0; i < count; i++)
        {
            Enemy enemy = Instantiate(enemyPrefab, SafeSpawnPoint(), Quaternion.identity);
            enemy.AddSpeed(SpeedBonus);
        }
        toSpawn -= count;
        alive += count;
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
