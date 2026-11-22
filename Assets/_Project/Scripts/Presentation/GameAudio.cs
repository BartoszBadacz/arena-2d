using Arena.Core;
using Arena.Data;
using UnityEngine;

namespace Arena.Presentation
{
    // Dźwięki gry: nasłuchuje faktów z Core (strzał, trafienie i leczenie gracza, fale, start, Game Over)
    // i gra odpowiedni sygnał z SoundBank. Fakty o wrogach przychodzą przez bootstrap (OnEnemyHit/OnEnemyKilled),
    // bo ogłasza je spawner z Gameplay, którego Presentation nie zna.
    public class GameAudio : MonoBehaviour
    {
        [Tooltip("Ile dźwięków może grać naraz. Najstarszy jest przerywany, gdy zabraknie źródeł.")]
        [SerializeField, Min(1)] private int voices = 12;

        private SoundBank sounds;
        private Weapon weapon;
        private Health playerHealth;
        private WaveDirector waves;
        private RunSession session;

        private AudioSource[] sources;
        private int nextSource;

        public void Init(SoundBank sounds, Weapon weapon, Health playerHealth, WaveDirector waves, RunSession session)
        {
            this.sounds = sounds;
            this.weapon = weapon;
            this.playerHealth = playerHealth;
            this.waves = waves;
            this.session = session;
            CreateSources(); // tu, nie w Awake: przy restarcie bootstrap woła session.Begin() (→ dźwięk) przed naszym Awake

            weapon.Fired += OnFired;
            playerHealth.Damaged += OnPlayerHurt;
            playerHealth.Healed += OnPlayerHealed;
            waves.WaveStarted += OnWaveStarted;
            waves.WaveCleared += OnWaveCleared;
            session.Started += OnRunStarted;
            session.GameOver += OnGameOver;
        }

        // Pula źródeł: wysokość tonu ustawia się na całym AudioSource, więc każdy dźwięk dostaje własne źródło.
        private void CreateSources()
        {
            sources = new AudioSource[voices];
            for (int i = 0; i < voices; i++)
            {
                AudioSource source = gameObject.AddComponent<AudioSource>();
                source.playOnAwake = false;
                source.spatialBlend = 0f; // 2D: głośność nie zależy od odległości od kamery
                sources[i] = source;
            }
        }

        private void Start()
        {
            if (sounds != null) return;
            Debug.LogError($"{nameof(GameAudio)}: brak Init — czy w scenie jest GameBootstrap?", this);
            enabled = false;
        }

        private void OnDestroy()
        {
            if (sounds == null) return;
            weapon.Fired -= OnFired;
            playerHealth.Damaged -= OnPlayerHurt;
            playerHealth.Healed -= OnPlayerHealed;
            waves.WaveStarted -= OnWaveStarted;
            waves.WaveCleared -= OnWaveCleared;
            session.Started -= OnRunStarted;
            session.GameOver -= OnGameOver;
        }

        public void OnEnemyHit() => Play(sounds.EnemyHit);
        public void OnEnemyKilled() => Play(sounds.EnemyKilled);

        private void OnFired() => Play(sounds.Shot);
        private void OnPlayerHurt() => Play(sounds.PlayerHurt);
        private void OnPlayerHealed() => Play(sounds.PlayerHealed);
        private void OnWaveStarted(int wave) => Play(sounds.WaveStarted);
        private void OnWaveCleared(int wave) => Play(sounds.WaveCleared);
        private void OnRunStarted() => Play(sounds.RunStarted);
        private void OnGameOver(RunResult result) => Play(sounds.GameOver);

        private void Play(SoundCue cue)
        {
            if (!enabled || cue.Clips.Length == 0) return;
            AudioClip clip = cue.Clips[Random.Range(0, cue.Clips.Length)];
            if (clip == null) return;

            AudioSource source = sources[nextSource];
            nextSource = (nextSource + 1) % sources.Length;
            source.pitch = 1f + Random.Range(-cue.PitchJitter, cue.PitchJitter);
            source.clip = clip;
            source.volume = cue.Volume;
            source.Play();
        }
    }
}
