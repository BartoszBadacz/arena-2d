using Arena.Data;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Arena.Gameplay
{
    // Ruch gracza WASD / lewa gałka pada. Cienki klej: wejście → prędkość ciała.
    // Liczby dostaje przez Init od GameBootstrap (wstrzykiwanie metodą, bo komponent tworzy Unity, nie my).
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMover : MonoBehaviour
    {
        private PlayerDefinition definition;
        private Rigidbody2D body;
        private InputAction moveAction;
        private Vector2 moveInput;

        public void Init(PlayerDefinition definition) => this.definition = definition;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            // Akcje „projektowe” z Assets/Settings/InputSystem_Actions (mapa Player, akcja Move).
            moveAction = InputSystem.actions.FindAction("Player/Move", throwIfNotFound: true);
        }

        private void Start()
        {
            // Start rusza po Awake wszystkich obiektów, więc bootstrap zdążył już wywołać Init.
            if (definition != null) return;
            Debug.LogError($"{nameof(PlayerMover)}: brak Init — czy w scenie jest GameBootstrap?", this);
            enabled = false;
        }

        // Co klatkę: czytamy wejście.
        private void Update() => moveInput = moveAction.ReadValue<Vector2>();

        // Co krok fizyki (stałe 50 Hz): ustawiamy prędkość ciała.
        private void FixedUpdate() => body.linearVelocity = moveInput * definition.MoveSpeed;
    }
}
