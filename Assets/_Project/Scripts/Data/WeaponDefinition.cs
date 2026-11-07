using UnityEngine;

namespace Arena.Data
{
    // Liczby broni gracza (Data/Weapon.asset).
    [CreateAssetMenu(fileName = "Weapon", menuName = "Arena/Weapon Definition")]
    public class WeaponDefinition : ScriptableObject
    {
        [SerializeField, Min(0.1f)] private float shotsPerSecond = 8f;
        [SerializeField, Min(0f)] private float projectileSpeed = 18f;
        [SerializeField, Min(0f)] private float projectileLifetime = 2f;
        [SerializeField, Min(1)] private int projectileDamage = 1;
        [Tooltip("Jak daleko od środka gracza pojawia się pocisk.")]
        [SerializeField, Min(0f)] private float muzzleOffset = 0.7f;

        // Typ ogólny GameObject, bo Data nie może znać Gameplay (tam jest Projectile) — byłby cykl zależności.
        // Gameplay sprawdza przy strzale, czy prefab ma komponent Projectile.
        [SerializeField] private GameObject projectilePrefab;

        public float ShotsPerSecond => shotsPerSecond;
        public float ProjectileSpeed => projectileSpeed;
        public float ProjectileLifetime => projectileLifetime;
        public int ProjectileDamage => projectileDamage;
        public float MuzzleOffset => muzzleOffset;
        public GameObject ProjectilePrefab => projectilePrefab;
    }
}
