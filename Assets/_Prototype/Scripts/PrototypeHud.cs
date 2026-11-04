using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

// Prototyp: napisy na ekranie przez OnGUI (UI „deweloperskie”, do wyrzucenia) + Game Over i restart.
public class PrototypeHud : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;

    private bool isGameOver;
    private GUIStyle bigLabel;

    private void OnEnable() => playerHealth.Died += OnPlayerDied;
    private void OnDisable() => playerHealth.Died -= OnPlayerDied;

    private void OnPlayerDied()
    {
        isGameOver = true;
        Time.timeScale = 0f; // zatrzymuje fizykę i wszystko, co liczy czas
    }

    private void Update()
    {
        if (isGameOver && Keyboard.current.rKey.wasPressedThisFrame)
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private void OnGUI()
    {
        bigLabel ??= new GUIStyle(GUI.skin.label) { fontSize = 48, alignment = TextAnchor.MiddleCenter };

        GUI.Label(new Rect(20, 20, 300, 30), $"HP: {playerHealth.Health} / {playerHealth.MaxHealth}");

        if (isGameOver)
        {
            GUI.Label(new Rect(0, Screen.height / 2f - 60, Screen.width, 60), "GAME OVER", bigLabel);
            GUI.Label(new Rect(0, Screen.height / 2f + 10, Screen.width, 30), "R — restart",
                new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontSize = 20 });
        }
    }
}
