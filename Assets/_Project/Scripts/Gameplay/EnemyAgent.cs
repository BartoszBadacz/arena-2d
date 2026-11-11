using System;
using Arena.Core;
using Arena.Data;
using UnityEngine;

namespace Arena.Gameplay
{
    // Wróg: cienki klej. Pyta fizykę o sąsiadów, kierunek liczy Steering (Core), życie trzyma Health (Core).
    // Zachowanie z danych: Chase goni gracza, Flee ucieka i ślizga się wzdłuż krawędzi areny (KeepInside),
    // HideInCrowd biegnie do losowego innego wroga i co chwilę zmienia kryjówkę (TargetSwitcher z Core).
    // Opcjonalnie skacze w bok jak mucha (Darting z Core), jeśli definicja ma ustawiony czas skoku.
    // Tworzy go spawner i od razu wywołuje Init — przed pierwszym FixedUpdate.
    [RequireComponent(typeof(Rigidbody2D))]
    public class EnemyAgent : MonoBehaviour, IHasHealth
    {
        // Wspólne bufory na wyniki zapytań fizyki — bez tworzenia nowych tablic co krok.
        private static readonly Collider2D[] Hits = new Collider2D[32];
        private static readonly System.Numerics.Vector2[] NeighbourPositions = new System.Numerics.Vector2[32];

        private EnemyDefinition definition;
        private EnemyContext context;
        private float speed;
        private TargetSwitcher<EnemyAgent> cover;
        private Darting darting; // null = bez skoków w bok
        private Func<EnemyAgent, bool> isValidCover;
        private Rigidbody2D body;
        private ContactFilter2D neighbourFilter;

        public Health Health { get; private set; }
        public EnemyDefinition Definition => definition;

        // speedBonus: przyspieszenie od fali (WaveDirector.SpeedBonus), dodawane do prędkości z definicji.
        public void Init(EnemyDefinition definition, EnemyContext context, float speedBonus = 0f)
        {
            this.definition = definition;
            this.context = context;
            speed = definition.Speed + speedBonus;
            Health = new Health(definition.MaxHealth);
            Health.Died += OnDied;
            if (definition.Behavior == EnemyBehavior.HideInCrowd)
                cover = new TargetSwitcher<EnemyAgent>(definition.RetargetInterval, context.Random);
            DartSettings dartSettings = definition.ToDartSettings();
            if (dartSettings != null) darting = new Darting(dartSettings, context.Random);
        }

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            // Sąsiedzi = tylko obiekty z tej samej warstwy (Enemy), bez sprawdzania typu w kodzie.
            neighbourFilter = new ContactFilter2D { useTriggers = false };
            neighbourFilter.SetLayerMask(1 << gameObject.layer);
            // Kryjówką może być tylko żywy wróg, który goni gracza (nie inny chowający się ani uciekający).
            isValidCover = other => other != null && other != this && !other.Health.IsDead
                                    && other.Definition.Behavior == EnemyBehavior.Chase;
        }

        private void Start()
        {
            if (Health != null) return;
            Debug.LogError($"{nameof(EnemyAgent)}: brak Init — wroga tworzy spawner, nie scena.", this);
            enabled = false;
        }

        private void FixedUpdate()
        {
            if (context.Player == null) { body.linearVelocity = Vector2.zero; return; }

            System.Numerics.Vector2 self = body.position.ToNumerics();
            System.Numerics.Vector2 separation =
                Steering.Separation(self, Neighbours(), definition.SeparationRadius, context.Random);
            System.Numerics.Vector2 player = ((Vector2)context.Player.position).ToNumerics();

            System.Numerics.Vector2 velocity = definition.Behavior switch
            {
                EnemyBehavior.Flee => FleeFrom(self, player, separation),
                EnemyBehavior.HideInCrowd => HideVelocity(self, player, separation),
                _ => Steering.Chase(self, player, separation, definition.SeparationStrength, speed),
            };
            if (darting != null) velocity = darting.Apply(velocity, Time.time);
            // Uciekający i chowający się nie mogą wbić się w ścianę (tarcie by ich zatrzymało) — ślizgają się wzdłuż niej.
            if (definition.Behavior != EnemyBehavior.Chase) velocity = Steering.KeepInside(self, velocity, context.ArenaHalfSize);
            body.linearVelocity = velocity.ToUnity();
        }

        // Do kryjówki (losowy inny wróg); bez kryjówki — ucieczka przed graczem.
        private System.Numerics.Vector2 HideVelocity(System.Numerics.Vector2 self, System.Numerics.Vector2 player,
                                                     System.Numerics.Vector2 separation)
        {
            EnemyAgent hideBehind = cover.Update(Time.time, context.Alive, isValidCover);
            if (hideBehind == null) return FleeFrom(self, player, separation);
            return Steering.Chase(self, hideBehind.body.position.ToNumerics(), separation, definition.SeparationStrength, speed);
        }

        private System.Numerics.Vector2 FleeFrom(System.Numerics.Vector2 self, System.Numerics.Vector2 player,
                                                 System.Numerics.Vector2 separation) =>
            Steering.Flee(self, player, separation, definition.SeparationStrength, speed);

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
