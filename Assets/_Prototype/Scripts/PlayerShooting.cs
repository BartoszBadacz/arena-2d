using UnityEngine;
using UnityEngine.InputSystem;

// Prototyp: przytrzymany lewy przycisk myszy = ogień ciągły w stronę kursora.
public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private float shotsPerSecond = 8f;
    [SerializeField] private float muzzleOffset = 0.7f;

    private InputAction attackAction;
    private Camera mainCamera;
    private Collider2D ownCollider;
    private float nextShotTime;

    private void Awake()
    {
        attackAction = InputSystem.actions.FindAction("Player/Attack", throwIfNotFound: true);
        mainCamera = Camera.main;
        ownCollider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (!attackAction.IsPressed() || Time.time < nextShotTime) return;

        nextShotTime = Time.time + 1f / shotsPerSecond;
        Shoot(AimDirection());
    }

    // Pozycja myszy jest w pikselach ekranu — przeliczamy ją na jednostki świata gry.
    private Vector2 AimDirection()
    {
        Vector2 mouseScreen = Mouse.current.position.ReadValue();
        Vector2 mouseWorld = mainCamera.ScreenToWorldPoint(mouseScreen);
        Vector2 direction = mouseWorld - (Vector2)transform.position;
        return direction.sqrMagnitude > 0.0001f ? direction.normalized : Vector2.right;
    }

    private void Shoot(Vector2 direction)
    {
        Vector2 spawnPosition = (Vector2)transform.position + direction * muzzleOffset;
        Bullet bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
        Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), ownCollider);
        bullet.Launch(direction);
    }
}
