using System;
using Arena.Core;
using Arena.Data;
using UnityEngine;

namespace Arena.Gameplay
{
    // Wróg: cienki klej. Pyta fizykę o sąsiadów, kierunek liczy Steering (Core), życie trzyma Health (Core).
    // Tworzy go spawner i od razu wywołuje Init — przed pierwszym FixedUpdate.
    [RequireComponent(typeof(Rigidbody2D))]
    public class EnemyAgent : MonoBehaviour, IHasHealth
    {
        // Wspólne bufory na wyniki zapytań fizyki — bez tworzenia nowych tablic co krok.
        private static readonly Collider2D[] Hits = new Collider2D[32];
        private static readonly System.Numerics.Vector2[] NeighbourPositions = new System.Numerics.Vector2[32];

        private EnemyDefinition definition;
        private Transform target;
        private System.Random random;
        private Rigidbody2D body;
        private ContactFilter2D neighbourFilter;

        public Health Health { get; private set; }
        public EnemyDefinition Definition => definition;

        public void Init(EnemyDefinition definition, Transform target, System.Random random)
        {
            this.definition = definition;
            this.target = target;
            this.random = random;
            Health = new Health(definition.MaxHealth);
            Health.Died += OnDied;
        }

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            // Sąsiedzi = tylko obiekty z tej samej warstwy (Enemy), bez sprawdzania typu w kodzie.
            neighbourFilter = new ContactFilter2D { useTriggers = false };
            neighbourFilter.SetLayerMask(1 << gameObject.layer);
        }

        private void Start()
        {
            if (Health != null) return;
            Debug.LogError($"{nameof(EnemyAgent)}: brak Init — wroga tworzy spawner, nie scena.", this);
            enabled = false;
        }

        private void FixedUpdate()
        {
            if (target == null) { body.linearVelocity = Vector2.zero; return; }

            System.Numerics.Vector2 self = body.position.ToNumerics();
            System.Numerics.Vector2 separation = Steering.Separation(self, Neighbours(), definition.SeparationRadius, random);
            System.Numerics.Vector2 velocity = Steering.Chase(
                self, ((Vector2)target.position).ToNumerics(), separation, definition.SeparationStrength, definition.Speed);
            body.linearVelocity = velocity.ToUnity();
        }

        private ReadOnlySpan<System.Numerics.Vector2> Neighbours()
        {
            int hitCount = Physics2D.OverlapCircle(body.position, definition.SeparationRadius, neighbourFilter, Hits);
            int count = 0;
            for (int i = 0; i < hitCount; i++)
            {
                Rigidbody2D other = Hits[i].attachedRigidbody;
                if (other == null || other == body) continue;
                NeighbourPositions[count++] = other.position.ToNumerics();
            }
            return new ReadOnlySpan<System.Numerics.Vector2>(NeighbourPositions, 0, count);
        }

        private void OnDied() => Destroy(gameObject);

        private void OnDestroy()
        {
            if (Health != null) Health.Died -= OnDied;
        }
    }
}
