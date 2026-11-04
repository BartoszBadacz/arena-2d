using UnityEngine;
using UnityEngine.InputSystem;

// Prototyp: ruch gracza WASD / lewa gałka pada.
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 6f;

    private Rigidbody2D body;
    private InputAction moveAction;
    private Vector2 moveInput;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        // Akcje „projektowe” z Assets/Settings/InputSystem_Actions (mapa Player, akcja Move).
        moveAction = InputSystem.actions.FindAction("Player/Move", throwIfNotFound: true);
    }

    // Co klatkę: czytamy wejście.
    private void Update()
    {
        moveInput = moveAction.ReadValue<Vector2>();
    }

    // Co krok fizyki (stałe 50 Hz): ustawiamy prędkość ciała.
    private void FixedUpdate()
    {
        body.linearVelocity = moveInput * speed;
    }
}
