using Arena.Core;
using UnityEngine;

namespace Arena.Data
{
    // Krzywa fal w jednym pliku danych (Data/Waves.asset). Balans = edycja tego pliku, nie kodu.
    [CreateAssetMenu(fileName = "Waves", menuName = "Arena/Wave Config")]
    public class WaveConfig : ScriptableObject
    {
        [Tooltip("Podstawowy wróg każdej fali.")]
        [SerializeField] private EnemyDefinition baseEnemy;

        [Header("Fale")]
        [SerializeField, Min(0f)] private float breakDuration = 3f;
        [SerializeField, Min(1)] private int firstWaveCount = 6;
        [SerializeField, Min(0)] private int extraEnemiesPerWave = 4;
        [SerializeField, Min(0.05f)] private float startInterval = 1.5f;
        [SerializeField, Range(0.1f, 1f)] private float intervalMultiplierPerWave = 0.9f;
        [SerializeField, Min(0.05f)] private float minInterval = 0.4f;

        [Header("Kilku wrogów naraz, każdy w innym miejscu")]
        [Tooltip("Liczba wrogów wychodzących naraz rośnie o 1 co tyle fal.")]
        [SerializeField, Min(1)] private int wavesPerBatchGrowth = 2;
        [SerializeField, Min(1)] private int maxBatchSize = 4;

        [Header("Szybkość: skokowo co kilka fal")]
        [SerializeField, Min(1)] private int wavesPerSpeedUp = 3;
        [SerializeField, Min(0f)] private float speedBonusPerSpeedUp = 0.6f;

        [Header("Testy")]
        [Tooltip("Od której fali zaczynać — do strojenia późniejszych fal bez grania od początku.")]
        [SerializeField, Min(1)] private int startWave = 1;

        public EnemyDefinition BaseEnemy => baseEnemy;

        public WaveSettings ToSettings() => new WaveSettings
        {
            BreakDuration = breakDuration,
            FirstWaveCount = firstWaveCount,
            ExtraEnemiesPerWave = extraEnemiesPerWave,
            StartInterval = startInterval,
            IntervalMultiplierPerWave = intervalMultiplierPerWave,
            MinInterval = minInterval,
            WavesPerBatchGrowth = wavesPerBatchGrowth,
            MaxBatchSize = maxBatchSize,
            WavesPerSpeedUp = wavesPerSpeedUp,
            SpeedBonusPerSpeedUp = speedBonusPerSpeedUp,
            StartWave = startWave,
        };
    }
}
