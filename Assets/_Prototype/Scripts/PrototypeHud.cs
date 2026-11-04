using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

// Prototyp: napisy na ekranie przez OnGUI (UI „deweloperskie”, do wyrzucenia),
// licznik fali/wrogów/czasu, odliczanie między falami, Game Over, restart i rekord (najwyższa fala) w obrębie sesji.
public class PrototypeHud : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private EnemySpawner spawner;
    [SerializeField] private float waveBannerDuration = 2f;

    private const float ReferenceHeight = 720f;

    // static = pamięć C#, nie scena: przeżywa przeładowanie sceny, ginie po zamknięciu gry.
    private static int bestWave;

    private float survivedTime;
    private int kills;
    private bool isGameOver;
    private bool isNewRecord;
    private float waveBannerUntil;
    private GUIStyle bigLabel;
    private GUIStyle centeredLabel;
    private GUIStyle hudLabel;

    private void OnEnable()
    {
        playerHealth.Died += OnPlayerDied;
        spawner.WaveStarted += OnWaveStarted;
        Enemy.Killed += OnEnemyKilled;
    }

    private void OnDisable()
    {
        playerHealth.Died -= OnPlayerDied;
        spawner.WaveStarted -= OnWaveStarted;
        Enemy.Killed -= OnEnemyKilled; // statyczne zdarzenie: bez odpięcia trzymałoby martwy HUD po restarcie
    }

    private void OnEnemyKilled() => kills++;

    private void OnWaveStarted(int wave) => waveBannerUntil = Time.time + waveBannerDuration;

    private void OnPlayerDied()
    {
        isGameOver = true;
        Time.timeScale = 0f; // zatrzymuje fizykę i wszystko, co liczy czas
        if (spawner.Wave > bestWave)
        {
            bestWave = spawner.Wave;
            isNewRecord = true;
        }
    }

    private void Update()
    {
        if (!isGameOver)
        {
            survivedTime += Time.deltaTime;
            return;
        }

        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private void OnGUI()
    {
        bigLabel ??= new GUIStyle(GUI.skin.label) { fontSize = 48, alignment = TextAnchor.MiddleCenter };
        centeredLabel ??= new GUIStyle(GUI.skin.label) { fontSize = 24, alignment = TextAnchor.MiddleCenter };
        hudLabel ??= new GUIStyle(GUI.skin.label) { fontSize = 24 };

        // OnGUI liczy w pikselach ekranu — skalujemy tak, jakby ekran zawsze miał 720 px wysokości.
        float scale = Screen.height / ReferenceHeight;
        GUI.matrix = Matrix4x4.Scale(new Vector3(scale, scale, 1f));
        float width = Screen.width / scale;

        GUI.Label(new Rect(20, 15, 800, 36),
            $"HP: {playerHealth.Health}/{playerHealth.MaxHealth}   Fala: {spawner.Wave}   Wrogowie: {spawner.EnemiesRemaining}", hudLabel);
        GUI.Label(new Rect(20, 50, 800, 36),
            $"Zabici: {kills}   Czas: {survivedTime:0} s   Rekord: fala {bestWave}", hudLabel);

        if (!isGameOver)
        {
            if (spawner.CurrentState == EnemySpawner.State.Break)
            {
                // Przerwa: przed pierwszą falą albo po wybiciu poprzedniej.
                if (spawner.Wave > 0)
                    GUI.Label(new Rect(0, 100, width, 60), $"FALA {spawner.Wave} UKOŃCZONA", bigLabel);
                GUI.Label(new Rect(0, 165, width, 36),
                    $"Fala {spawner.Wave + 1} za {Mathf.CeilToInt(spawner.BreakTimeLeft)}…", centeredLabel);
            }
            else if (Time.time < waveBannerUntil)
            {
                GUI.Label(new Rect(0, 100, width, 60), $"FALA {spawner.Wave}", bigLabel);
            }
        }

        if (isGameOver)
        {
            float y = ReferenceHeight / 2f;
            GUI.Label(new Rect(0, y - 80, width, 60), "GAME OVER", bigLabel);
            GUI.Label(new Rect(0, y - 10, width, 36),
                $"Dotarłeś do fali {spawner.Wave} (zabici: {kills}, czas: {survivedTime:0} s)", centeredLabel);
            GUI.Label(new Rect(0, y + 30, width, 36),
                isNewRecord ? "NOWY REKORD!" : $"Rekord: fala {bestWave}", centeredLabel);
            GUI.Label(new Rect(0, y + 80, width, 36), "R — restart", centeredLabel);
        }
    }
}
