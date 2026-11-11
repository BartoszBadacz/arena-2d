using Arena.Core;
using UnityEngine;

namespace Arena.Presentation
{
    // Miganie w czasie nietykalności po trafieniu. Czyta stan (IsInvulnerable) co klatkę zamiast słuchać zdarzeń:
    // „czy migać” to stan, nie fakt. Szuka życia przez IHasHealth, jak HitFlash.
    [RequireComponent(typeof(SpriteRenderer))]
    public class HitBlink : MonoBehaviour
    {
        [SerializeField, Min(0.01f)] private float blinkInterval = 0.1f;

        private SpriteRenderer spriteRenderer;
        private Health health;

        private void Awake() => spriteRenderer = GetComponent<SpriteRenderer>();

        private void Start()
        {
            health = GetComponent<IHasHealth>()?.Health;
            if (health != null) return;
            Debug.LogError($"{nameof(HitBlink)}: na obiekcie nie ma nic z życiem ({nameof(IHasHealth)}).", this);
            enabled = false;
        }

        private void Update()
        {
            // Martwy zawsze widoczny — inaczej pauza przy Game Over mogłaby go „zamrozić” w fazie zgaszonej.
            bool visible = health.IsDead
                           || !health.IsInvulnerable(Time.time)
                           || Mathf.FloorToInt(Time.time / blinkInterval) % 2 == 0;
            spriteRenderer.enabled = visible;
        }
    }
}
