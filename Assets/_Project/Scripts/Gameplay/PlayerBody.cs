using Arena.Core;
using UnityEngine;

namespace Arena.Gameplay
{
    // Ciało gracza: kontakt z wrogiem zabiera życie. Ile — mówi definicja wroga; czy w ogóle — Health (nietykalność).
    // Health tworzy GameBootstrap, bo czytają je też RunSession i HUD.
    public class PlayerBody : MonoBehaviour, IHasHealth
    {
        public Health Health { get; private set; }

        public void Init(Health health) => Health = health;

        private void Start()
        {
            if (Health != null) return;
            Debug.LogError($"{nameof(PlayerBody)}: brak Init — czy w scenie jest GameBootstrap?", this);
            enabled = false;
        }

        // Wywoływane co krok fizyki, dopóki gracz dotyka jakiegoś collidera.
        private void OnCollisionStay2D(Collision2D collision)
        {
            if (!enabled) return; // magiczne metody fizyki działają nawet przy wyłączonym komponencie
            if (collision.collider.TryGetComponent(out EnemyAgent enemy))
                Health.TakeDamage(enemy.Definition.ContactDamage, Time.time);
        }
    }
}
