using Arena.Core;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Arena.Gameplay
{
    // Klej przebiegu rozgrywki: podaje RunSession upływ czasu, przy Game Over zatrzymuje świat,
    // a potem czeka na R — restart to przeładowanie sceny (świat od zera, rekord żyje w bootstrapie).
    public class RunFlow : MonoBehaviour
    {
        private RunSession session;

        public void Init(RunSession session)
        {
            this.session = session;
            session.GameOver += OnGameOver;
        }

        private void Awake()
        {
            // timeScale przeżywa przeładowanie sceny — po restarcie świat musi znowu ruszyć.
            Time.timeScale = 1f;
        }

        private void Start()
        {
            if (session != null) return;
            Debug.LogError($"{nameof(RunFlow)}: brak Init — czy w scenie jest GameBootstrap?", this);
            enabled = false;
        }

        private void OnDestroy()
        {
            if (session != null) session.GameOver -= OnGameOver;
        }

        private void Update()
        {
            if (!session.IsOver)
            {
                session.Tick(Time.deltaTime);
                return;
            }

            if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
                SceneManager.LoadScene(gameObject.scene.buildIndex);
        }

        // Wynik pokazuje HUD (Presentation) — tu tylko zatrzymanie świata.
        private void OnGameOver(RunResult result) => Time.timeScale = 0f; // stop fizyki i wszystkiego, co liczy czas gry
    }
}
