using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

// Prototyp: napisy na ekranie przez OnGUI (UI „deweloperskie”, do wyrzucenia),
// licznik czasu/fali/zabitych, Game Over, restart i rekord w obrębie sesji.
public class PrototypeHud : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private EnemySpawner spawner;
    [SerializeField] private float waveBannerDuration = 2f;

    private const float ReferenceHeight = 720f;

    // static = pamięć C#, nie scena: przeżywa przeładowanie sceny, ginie po zamknięciu gry.
    private static float bestTime;

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
        if (survivedTime > bestTime)
        {
            bestTime = survivedTime;
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
            $"HP: {playerHealth.Health}/{playerHealth.MaxHealth}   Fala: {spawner.Wave}   Zabici: {kills}", hudLabel);
        GUI.Label(new Rect(20, 50, 800, 36), $"Czas: {survivedTime:0.0} s   Rekord: {bestTime:0.0} s", hudLabel);

        if (!isGameOver && Time.time < waveBannerUntil)
            GUI.Label(new Rect(0, 100, width, 60), $"FALA {spawner.Wave}", bigLabel);

        if (isGameOver)
        {
            float y = ReferenceHeight / 2f;
            GUI.Label(new Rect(0, y - 80, width, 60), "GAME OVER", bigLabel);
            GUI.Label(new Rect(0, y - 10, width, 36),
                $"Przetrwałeś {survivedTime:0.0} s (fala {spawner.Wave}, zabici: {kills})", centeredLabel);
            GUI.Label(new Rect(0, y + 30, width, 36),
                isNewRecord ? "NOWY REKORD!" : $"Rekord: {bestTime:0.0} s", centeredLabel);
            GUI.Label(new Rect(0, y + 80, width, 36), "R — restart", centeredLabel);
        }
    }
}
