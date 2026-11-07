using Arena.Core;
using UnityEngine;

namespace Arena.Presentation
{
    // Krótki błysk przy trafieniu. Nie zna wroga ani gracza — szuka na swoim obiekcie czegoś, co „ma życie” (IHasHealth),
    // i nasłuchuje faktu Damaged. Czas i kolor błysku to strojenie efektu, więc zostają w prefabie.
    [RequireComponent(typeof(SpriteRenderer))]
    public class HitFlash : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float duration = 0.08f;
        [SerializeField] private Color flashColor = Color.white;

        private SpriteRenderer spriteRenderer;
        private Color baseColor;
        private Health health;
        private float flashUntil;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            baseColor = spriteRenderer.color;
        }

        // Start, nie Awake: życie powstaje w Init, które spawner woła zaraz po utworzeniu obiektu.
        private void Start()
        {
            health = GetComponent<IHasHealth>()?.Health;
            if (health == null)
            {
                Debug.LogError($"{nameof(HitFlash)}: na obiekcie nie ma nic z życiem ({nameof(IHasHealth)}).", this);
                enabled = false;
                return;
            }
            health.Damaged += OnDamaged;
        }

        private void OnDestroy()
        {
            if (health != null) health.Damaged -= OnDamaged;
        }

        private void OnDamaged() => flashUntil = Time.time + duration;

        private void Update() => spriteRenderer.color = Time.time < flashUntil ? flashColor : baseColor;
    }
}
