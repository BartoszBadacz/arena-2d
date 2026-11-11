using Arena.Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace Arena.Presentation
{
    // HUD w UI Toolkit (Assets/_Project/UI/Hud.uxml + Hud.uss).
    // Stan (HP, fala, wrogowie, zabici, czas, rekord, odliczanie przerwy) czyta co klatkę;
    // na fakty reaguje: WaveStarted → baner „FALA N”, GameOver → ekran podsumowania.
    [RequireComponent(typeof(UIDocument))]
    public class Hud : MonoBehaviour
    {
        private const string HiddenGameOverClass = "game-over--hidden";
        private const string HiddenBannerClass = "banner--hidden";
        private const string HiddenSubtitleClass = "banner__subtitle--hidden";
        private const string RecordClass = "game-over__record";

        private enum BannerMode { Hidden, WaveStart, Break }

        [Tooltip("Ile sekund widać baner „FALA N” po starcie fali.")]
        [SerializeField, Min(0f)] private float waveBannerDuration = 2f;

        private Health playerHealth;
        private RunStats stats;
        private SessionRecord record;
        private RunSession session;
        private WaveDirector waves;

        private Label healthLabel;
        private Label waveLabel;
        private Label enemiesLabel;
        private Label scoreLabel;
        private Label killsLabel;
        private Label timeLabel;
        private Label recordLabel;
        private VisualElement banner;
        private Label bannerTitle;
        private Label bannerSubtitle;
        private VisualElement gameOverPanel;
        private Label scoreResultLabel;
        private Label summaryLabel;
        private Label recordResultLabel;

        private float waveBannerUntil;

        // Ostatnio pokazane liczby: napis zmieniamy tylko, gdy liczba się zmieni — bez nowego stringa co klatkę.
        private int shownHealth = -1;
        private int shownWave = -1;
        private int shownEnemies = -1;
        private int shownScore = -1;
        private int shownKills = -1;
        private int shownSeconds = -1;
        private int shownBestScore = -1;
        private BannerMode shownBanner = BannerMode.Hidden;
        private int shownBannerWave = -1;
        private int shownBreakSeconds = -1;

        public void Init(Health playerHealth, RunStats stats, SessionRecord record, RunSession session, WaveDirector waves)
        {
            this.playerHealth = playerHealth;
            this.stats = stats;
            this.record = record;
            this.session = session;
            this.waves = waves;
            session.GameOver += ShowGameOver;
            waves.WaveStarted += OnWaveStarted;
        }

        // Start, nie Awake: UIDocument buduje drzewo elementów w swoim OnEnable.
        private void Start()
        {
            if (session == null)
            {
                Debug.LogError($"{nameof(Hud)}: brak Init — czy w scenie jest GameBootstrap?", this);
                enabled = false;
                return;
            }

            VisualElement root = GetComponent<UIDocument>().rootVisualElement;
            healthLabel = root.Q<Label>("healthLabel");
            waveLabel = root.Q<Label>("waveLabel");
            enemiesLabel = root.Q<Label>("enemiesLabel");
            scoreLabel = root.Q<Label>("scoreLabel");
            killsLabel = root.Q<Label>("killsLabel");
            timeLabel = root.Q<Label>("timeLabel");
            recordLabel = root.Q<Label>("recordLabel");
            banner = root.Q("banner");
            bannerTitle = root.Q<Label>("bannerTitle");
            bannerSubtitle = root.Q<Label>("bannerSubtitle");
            gameOverPanel = root.Q("gameOverPanel");
            scoreResultLabel = root.Q<Label>("scoreResultLabel");
            summaryLabel = root.Q<Label>("summaryLabel");
            recordResultLabel = root.Q<Label>("recordResultLabel");
        }

        private void OnDestroy()
        {
            if (session != null) session.GameOver -= ShowGameOver;
            if (waves != null) waves.WaveStarted -= OnWaveStarted;
        }

        private void OnWaveStarted(int wave) => waveBannerUntil = Time.time + waveBannerDuration;

        private void Update()
        {
            UpdateStats();
            UpdateBanner();
        }

        private void UpdateStats()
        {
            if (playerHealth.Current != shownHealth)
            {
                shownHealth = playerHealth.Current;
                healthLabel.text = $"HP: {shownHealth}/{playerHealth.Max}";
            }
            if (waves.Wave != shownWave)
            {
                shownWave = waves.Wave;
                waveLabel.text = $"Fala: {shownWave}";
            }
            if (waves.EnemiesRemaining != shownEnemies)
            {
                shownEnemies = waves.EnemiesRemaining;
                enemiesLabel.text = $"Wrogowie: {shownEnemies}";
            }
            if (stats.Score != shownScore)
            {
                shownScore = stats.Score;
                scoreLabel.text = $"Punkty: {shownScore}";
            }
            if (stats.Kills != shownKills)
            {
                shownKills = stats.Kills;
                killsLabel.text = $"Zabici: {shownKills}";
            }
            int seconds = Mathf.FloorToInt(stats.SurvivedTime);
            if (seconds != shownSeconds)
            {
                shownSeconds = seconds;
                timeLabel.text = $"Czas: {shownSeconds} s";
            }
            if (record.BestScore != shownBestScore)
            {
                shownBestScore = record.BestScore;
                recordLabel.text = $"Rekord: {shownBestScore}";
            }
        }

        // Przerwa: „FALA N UKOŃCZONA” + odliczanie. Start fali: „FALA N” (+ „WROGOWIE SZYBSI!”) przez kilka sekund.
        private void UpdateBanner()
        {
            BannerMode mode = BannerMode.Hidden;
            if (!session.IsOver)
            {
                if (waves.CurrentState == WaveDirector.State.Break) mode = BannerMode.Break;
                else if (Time.time < waveBannerUntil) mode = BannerMode.WaveStart;
            }
            int breakSeconds = Mathf.CeilToInt(waves.BreakTimeLeft);

            if (mode == shownBanner && waves.Wave == shownBannerWave && breakSeconds == shownBreakSeconds) return;
            shownBanner = mode;
            shownBannerWave = waves.Wave;
            shownBreakSeconds = breakSeconds;

            banner.EnableInClassList(HiddenBannerClass, mode == BannerMode.Hidden);
            switch (mode)
            {
                case BannerMode.Break:
                    // Przed pierwszą falą nie ma czego ogłaszać jako ukończonej — tylko odliczanie.
                    bannerTitle.text = waves.Wave > 0 ? $"FALA {waves.Wave} UKOŃCZONA" : "PRZYGOTUJ SIĘ";
                    bannerSubtitle.text = $"Fala {waves.Wave + 1} za {breakSeconds}…";
                    bannerSubtitle.RemoveFromClassList(HiddenSubtitleClass);
                    break;
                case BannerMode.WaveStart:
                    bannerTitle.text = $"FALA {waves.Wave}";
                    bannerSubtitle.text = "WROGOWIE SZYBSI!";
                    bannerSubtitle.EnableInClassList(HiddenSubtitleClass, !waves.IsSpeedUpWave);
                    break;
            }
        }

        private void ShowGameOver(RunResult result)
        {
            scoreResultLabel.text = $"Punkty: {result.Score}";
            summaryLabel.text = $"Fala: {result.Wave}   Zabici: {result.Kills}   Czas: {result.SurvivedTime:0} s";
            recordResultLabel.text = result.IsNewRecord ? "NOWY REKORD!" : $"Rekord: {result.BestScore}";
            recordResultLabel.EnableInClassList(RecordClass, result.IsNewRecord);
            gameOverPanel.RemoveFromClassList(HiddenGameOverClass);
        }
    }
}
