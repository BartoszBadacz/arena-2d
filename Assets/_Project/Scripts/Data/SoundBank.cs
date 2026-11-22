using UnityEngine;

namespace Arena.Data
{
    // Wszystkie dźwięki gry w jednym pliku danych (Sounds.asset). Podmiana dźwięku = podmiana klipu w Inspectorze.
    [CreateAssetMenu(fileName = "Sounds", menuName = "Arena/Sound Bank")]
    public class SoundBank : ScriptableObject
    {
        [Header("Przebieg gry")]
        [SerializeField] private SoundCue runStarted = new();
        [SerializeField] private SoundCue waveStarted = new();
        [SerializeField] private SoundCue waveCleared = new();
        [SerializeField] private SoundCue gameOver = new();

        [Header("Gracz")]
        [SerializeField] private SoundCue shot = new();
        [SerializeField] private SoundCue playerHurt = new();
        [SerializeField] private SoundCue playerHealed = new();

        [Header("Wrogowie")]
        [SerializeField] private SoundCue enemyHit = new();
        [SerializeField] private SoundCue enemyKilled = new();

        public SoundCue RunStarted => runStarted;
        public SoundCue WaveStarted => waveStarted;
        public SoundCue WaveCleared => waveCleared;
        public SoundCue GameOver => gameOver;
        public SoundCue Shot => shot;
        public SoundCue PlayerHurt => playerHurt;
        public SoundCue PlayerHealed => playerHealed;
        public SoundCue EnemyHit => enemyHit;
        public SoundCue EnemyKilled => enemyKilled;
    }
}
