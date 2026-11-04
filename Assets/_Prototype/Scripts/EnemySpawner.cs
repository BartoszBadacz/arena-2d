using System;
using UnityEngine;

// Prototyp: fale „na czas”. Co waveDuration sekund nowa fala: wrogowie pojawiają się częściej i są szybsi.
public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Enemy enemyPrefab;
    [SerializeField] private Vector2 arenaHalfSize = new Vector2(13f, 7f);

    [Header("Fale")]
    [SerializeField] private float waveDuration = 15f;
    [SerializeField] private float startInterval = 1.5f;
    [SerializeField] private float intervalMultiplierPerWave = 0.8f;
    [SerializeField] private float minInterval = 0.2f;
    [SerializeField] private float speedBonusPerWave = 0.3f;

    public event Action<int> WaveStarted;
    public int Wave { get; private set; }

    private float nextSpawnTime;

    private void Update()
    {
        // timeSinceLevelLoad liczy od załadowania sceny — po restarcie zaczyna od zera (Time.time nie).
        float elapsed = Time.timeSinceLevelLoad;

        int wave = 1 + Mathf.FloorToInt(elapsed / waveDuration);
        if (wave != Wave)
        {
            Wave = wave;
            WaveStarted?.Invoke(Wave);
        }

        if (elapsed < nextSpawnTime) return;
        nextSpawnTime = elapsed + CurrentInterval();

        Enemy enemy = Instantiate(enemyPrefab, RandomEdgePoint(), Quaternion.identity);
        enemy.AddSpeed(speedBonusPerWave * (Wave - 1));
    }

    private float CurrentInterval()
    {
        float interval = startInterval * Mathf.Pow(intervalMultiplierPerWave, Wave - 1);
        return Mathf.Max(interval, minInterval);
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
