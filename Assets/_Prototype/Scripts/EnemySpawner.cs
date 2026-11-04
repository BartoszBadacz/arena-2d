using UnityEngine;

// Prototyp: co kilka sekund nowy wróg przy losowej krawędzi areny. Fale dojdą w kroku 5.
public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Enemy enemyPrefab;
    [SerializeField] private float interval = 1.5f;
    [SerializeField] private Vector2 arenaHalfSize = new Vector2(13f, 7f);

    private float nextSpawnTime;

    private void Update()
    {
        if (Time.time < nextSpawnTime) return;
        nextSpawnTime = Time.time + interval;
        Instantiate(enemyPrefab, RandomEdgePoint(), Quaternion.identity);
    }

    private Vector2 RandomEdgePoint()
    {
        float x = Random.Range(-arenaHalfSize.x, arenaHalfSize.x);
        float y = Random.Range(-arenaHalfSize.y, arenaHalfSize.y);
        // Przyklejamy losowy punkt do jednej z czterech krawędzi.
        switch (Random.Range(0, 4))
        {
            case 0: return new Vector2(x, arenaHalfSize.y);
            case 1: return new Vector2(x, -arenaHalfSize.y);
            case 2: return new Vector2(arenaHalfSize.x, y);
            default: return new Vector2(-arenaHalfSize.x, y);
        }
    }
}
