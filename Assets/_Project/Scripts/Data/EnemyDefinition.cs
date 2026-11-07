using UnityEngine;

namespace Arena.Data
{
    // Typ wroga jako plik danych (Data/Slime.asset, później Runner.asset, Tank.asset).
    // Nowy typ o tym samym zachowaniu = nowy plik + wariant prefabu, bez nowego kodu.
    [CreateAssetMenu(fileName = "Enemy", menuName = "Arena/Enemy Definition")]
    public class EnemyDefinition : ScriptableObject
    {
        // GameObject, nie EnemyAgent: Data nie zna Gameplay (patrz WeaponDefinition).
        [SerializeField] private GameObject prefab;

        [SerializeField, Min(1)] private int maxHealth = 3;
        [SerializeField, Min(0f)] private float speed = 3f;
        [Tooltip("Obrażenia zadawane graczowi przy kontakcie.")]
        [SerializeField, Min(0)] private int contactDamage = 1;

        [Header("Separacja: odsuwanie się od sąsiadów (boids)")]
        [SerializeField, Min(0.01f)] private float separationRadius = 2f;
        [SerializeField, Min(0f)] private float separationStrength = 3f;

        public GameObject Prefab => prefab;
        public int MaxHealth => maxHealth;
        public float Speed => speed;
        public int ContactDamage => contactDamage;
        public float SeparationRadius => separationRadius;
        public float SeparationStrength => separationStrength;
    }
}
