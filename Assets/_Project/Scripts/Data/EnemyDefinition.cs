using Arena.Core;
using UnityEngine;

namespace Arena.Data
{
    // Jak wróg porusza się względem gracza.
    public enum EnemyBehavior
    {
        Chase,       // goni gracza
        Flee,        // ucieka przed graczem — trzeba go dopaść
        HideInCrowd, // chowa się przy losowym innym wrogu, co chwilę zmienia kryjówkę (bez innych wrogów: ucieka)
    }

    // Typ wroga jako plik danych (Data/Slime.asset, później Runner.asset, Tank.asset).
    // Nowy typ z jednym z istniejących zachowań (EnemyBehavior) = nowy plik + wariant prefabu, bez nowego kodu.
    [CreateAssetMenu(fileName = "Enemy", menuName = "Arena/Enemy Definition")]
    public class EnemyDefinition : ScriptableObject
    {
        // GameObject, nie EnemyAgent: Data nie zna Gameplay (patrz WeaponDefinition).
        [SerializeField] private GameObject prefab;

        [SerializeField] private EnemyBehavior behavior = EnemyBehavior.Chase;
        [Tooltip("HideInCrowd: co ile sekund losuje nową kryjówkę.")]
        [SerializeField, Min(0.1f)] private float retargetInterval = 2.5f;
        [SerializeField, Min(1)] private int maxHealth = 3;
        [SerializeField, Min(0f)] private float speed = 3f;
        [Tooltip("Obrażenia zadawane graczowi przy kontakcie.")]
        [SerializeField, Min(0)] private int contactDamage = 1;

        [Header("Nagroda za zabicie")]
        [SerializeField, Min(0)] private int scoreValue = 10;
        [Tooltip("Ile życia odzyskuje gracz (najwyżej do pełnego).")]
        [SerializeField, Min(0)] private int healOnKill;

        [Header("Skoki w bok (jak mucha). Czas skoku 0 = bez skoków")]
        [Tooltip("Jak długo trwa jeden skok (s).")]
        [SerializeField, Min(0f)] private float dartDuration;
        [Tooltip("Przerwa między skokami: losowo od (s).")]
        [SerializeField, Min(0.05f)] private float dartIntervalMin = 0.3f;
        [Tooltip("Przerwa między skokami: losowo do (s).")]
        [SerializeField, Min(0.05f)] private float dartIntervalMax = 0.8f;
        [Tooltip("Ile razy szybciej w trakcie skoku.")]
        [SerializeField, Min(1f)] private float dartSpeedMultiplier = 1.8f;

        [Header("Separacja: odsuwanie się od sąsiadów (boids)")]
        [SerializeField, Min(0.01f)] private float separationRadius = 2f;
        [SerializeField, Min(0f)] private float separationStrength = 3f;

        public GameObject Prefab => prefab;
        public EnemyBehavior Behavior => behavior;
        public float RetargetInterval => retargetInterval;
        public int ScoreValue => scoreValue;
        public int HealOnKill => healOnKill;
        public int MaxHealth => maxHealth;
        public float Speed => speed;
        public int ContactDamage => contactDamage;
        public float SeparationRadius => separationRadius;
        public float SeparationStrength => separationStrength;

        // null = ten typ nie skacze w bok.
        public DartSettings ToDartSettings() => dartDuration <= 0f ? null : new DartSettings
        {
            IntervalMin = dartIntervalMin,
            IntervalMax = Mathf.Max(dartIntervalMin, dartIntervalMax),
            Duration = dartDuration,
            SpeedMultiplier = dartSpeedMultiplier,
        };
    }
}
