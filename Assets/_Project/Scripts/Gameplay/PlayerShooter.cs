using Arena.Core;
using Arena.Data;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Arena.Gameplay
{
    // Przytrzymany lewy przycisk myszy = ogień ciągły w stronę kursora.
    // Kiedy wolno strzelić, decyduje Weapon (Core); tu tylko wejście, celowanie (z obrotem postaci) i tworzenie pocisku.
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerShooter : MonoBehaviour
    {
        private Weapon weapon;
        private WeaponDefinition definition;
        private Camera aimCamera;
        private InputAction attackAction;
        private Rigidbody2D body;

        public void Init(Weapon weapon, WeaponDefinition definition, Camera aimCamera)
        {
            this.weapon = weapon;
            this.definition = definition;
            this.aimCamera = aimCamera;
        }

        private void Awake()
        {
            attackAction = InputSystem.actions.FindAction("Player/Attack", throwIfNotFound: true);
            body = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            if (weapon != null && definition != null && aimCamera != null) return;
            Debug.LogError($"{nameof(PlayerShooter)}: brak Init — czy w scenie jest GameBootstrap?", this);
            enabled = false;
        }

        private void Update()
        {
            Vector2 aim = AimDirection();
            // Postać patrzy tam, gdzie celuje (sprite patrzy w prawo, +x). Przez Rigidbody2D, bo transform nadpisuje interpolacja.
            // Przy zatrzymanym świecie (ekran startowy, Game Over) postać się nie obraca.
            if (Time.timeScale > 0f) body.rotation = Mathf.Atan2(aim.y, aim.x) * Mathf.Rad2Deg;
            if (attackAction.IsPressed() && weapon.TryFire(Time.time)) Shoot(aim);
        }

        // Pozycja myszy jest w pikselach ekranu — przeliczamy ją na jednostki świata gry.
        private Vector2 AimDirection()
        {
            Vector2 mouseWorld = aimCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Vector2 direction = mouseWorld - (Vector2)transform.position;
            return direction.sqrMagnitude > 0.0001f ? direction.normalized : Vector2.right;
        }

        private void Shoot(Vector2 direction)
        {
            Vector2 spawnPosition = (Vector2)transform.position + direction * definition.MuzzleOffset;
            GameObject instance = Instantiate(definition.ProjectilePrefab, spawnPosition, Quaternion.identity);
            if (!instance.TryGetComponent(out Projectile projectile))
            {
                Debug.LogError($"{definition.name}: prefab pocisku nie ma komponentu {nameof(Projectile)}.", definition);
                Destroy(instance);
                return;
            }
            projectile.Launch(direction * definition.ProjectileSpeed, definition.ProjectileLifetime, definition.ProjectileDamage);
        }
    }
}
