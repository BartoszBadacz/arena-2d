using Arena.Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace Arena.Presentation
{
    // HUD w UI Toolkit (Assets/_Project/UI/Hud.uxml + Hud.uss).
    // Stan (HP, zabici, czas, rekord) czyta co klatkę; na fakt GameOver pokazuje ekran podsumowania.
    [RequireComponent(typeof(UIDocument))]
    public class Hud : MonoBehaviour
    {
        private const string HiddenClass = "game-over--hidden";
        private const string RecordClass = "game-over__record";

        private Health playerHealth;
        private RunStats stats;
        private SessionRecord record;
        private RunSession session;

        private Label healthLabel;
        private Label killsLabel;
        private Label timeLabel;
        private Label recordLabel;
        private VisualElement gameOverPanel;
        private Label summaryLabel;
        private Label recordResultLabel;

        // Ostatnio pokazane liczby: napis zmieniamy tylko, gdy liczba się zmieni — bez nowego stringa co klatkę.
        private int shownHealth = -1;
        private int shownKills = -1;
        private int shownSeconds = -1;
        private int shownBestWave = -1;

        public void Init(Health playerHealth, RunStats stats, SessionRecord record, RunSession session)
        {
            this.playerHealth = playerHealth;
            this.stats = stats;
            this.record = record;
            this.session = session;
            session.GameOver += ShowGameOver;
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
            killsLabel = root.Q<Label>("killsLabel");
            timeLabel = root.Q<Label>("timeLabel");
            recordLabel = root.Q<Label>("recordLabel");
            gameOverPanel = root.Q("gameOverPanel");
            summaryLabel = root.Q<Label>("summaryLabel");
            recordResultLabel = root.Q<Label>("recordResultLabel");
        }

        private void OnDestroy()
        {
            if (session != null) session.GameOver -= ShowGameOver;
        }

        private void Update()
        {
            if (playerHealth.Current != shownHealth)
            {
                shownHealth = playerHealth.Current;
                healthLabel.text = $"HP: {shownHealth}/{playerHealth.Max}";
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
            if (record.BestWave != shownBestWave)
            {
                shownBestWave = record.BestWave;
                recordLabel.text = $"Rekord: fala {shownBestWave}";
            }
        }

        private void ShowGameOver(RunResult result)
        {
            summaryLabel.text = $"Fala: {result.Wave}   Zabici: {result.Kills}   Czas: {result.SurvivedTime:0} s";
            recordResultLabel.text = result.IsNewRecord ? "NOWY REKORD!" : $"Rekord: fala {result.BestWave}";
            recordResultLabel.EnableInClassList(RecordClass, result.IsNewRecord);
            gameOverPanel.RemoveFromClassList(HiddenClass);
        }
    }
}
