using Arena.Core;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Arena.Gameplay
{
    // Klej przebiegu rozgrywki: świat stoi do startu (ekran startowy), podaje RunSession upływ czasu,
    // przy Game Over znowu zatrzymuje świat, a potem czeka na R — restart to przeładowanie sceny
    // (świat od zera, rekord żyje w bootstrapie).
    public class RunFlow : MonoBehaviour
    {
        private RunSession session;

        public void Init(RunSession session)
        {
            this.session = session;
            session.Started += OnStarted;
            session.GameOver += OnGameOver;
            // Ustawiane tu, nie w Awake: bootstrap woła Init wcześniej (-1000), a Awake by to nadpisał.
            // timeScale przeżywa przeładowanie sceny — dlatego zawsze ustawiamy go od nowa.
            Time.timeScale = session.HasStarted ? 1f : 0f;
        }

        private void Start()
        {
            if (session != null) return;
            Debug.LogError($"{nameof(RunFlow)}: brak Init — czy w scenie jest GameBootstrap?", this);
            enabled = false;
        }

        private void OnDestroy()
        {
            if (session == null) return;
            session.Started -= OnStarted;
            session.GameOver -= OnGameOver;
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

        private void OnStarted() => Time.timeScale = 1f;

        // Wynik pokazuje HUD (Presentation) — tu tylko zatrzymanie świata.
        private void OnGameOver(RunResult result) => Time.timeScale = 0f; // stop fizyki i wszystkiego, co liczy czas gry
    }
}
