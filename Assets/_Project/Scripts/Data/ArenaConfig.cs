using UnityEngine;

namespace Arena.Data
{
    // Wymiary areny ważne dla logiki (Data/Arena.asset). Ściany w prefabie Arena muszą się z nimi zgadzać.
    [CreateAssetMenu(fileName = "Arena", menuName = "Arena/Arena Config")]
    public class ArenaConfig : ScriptableObject
    {
        [Tooltip("Połowa szerokości i wysokości obszaru, na którego krawędzi pojawiają się wrogowie.")]
        [SerializeField] private Vector2 halfSize = new(13f, 7f);
        [Tooltip("Wrogowie nie pojawiają się bliżej gracza niż ta odległość.")]
        [SerializeField, Min(0f)] private float minSpawnDistance = 6f;

        public Vector2 HalfSize => halfSize;
        public float MinSpawnDistance => minSpawnDistance;
    }
}
