using System;
using UnityEngine;

// Prototyp: wróg idzie prosto na gracza, ma kilka punktów życia, błyska przy trafieniu.
[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
    // Statyczne zdarzenie: jedno dla wszystkich wrogów, licznik nie musi znać każdego z osobna.
    public static event Action Killed;

    [SerializeField] private float speed = 3f;
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private float hitFlashDuration = 0.08f;

    [Header("Separacja: odsuwanie się od sąsiadów (boids)")]
    [SerializeField] private float separationRadius = 2f;
    [SerializeField] private float separationStrength = 3f;

    // Wspólny bufor na wyniki zapytań fizyki — bez tworzenia nowej tablicy co krok.
    private static readonly Collider2D[] Neighbours = new Collider2D[32];

    private Rigidbody2D body;
    private SpriteRenderer spriteRenderer;
    private Transform target;
    private Color baseColor;
    private int health;
    private float flashUntil;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        baseColor = spriteRenderer.color;
        health = maxHealth;
    }

    private void Start()
    {
        // Prototyp: szukamy gracza po tagu. Docelowo referencja podana z zewnątrz.
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null) target = player.transform;
    }

    private void Update()
    {
        spriteRenderer.color = Time.time < flashUntil ? Color.white : baseColor;
    }

    private void FixedUpdate()
    {
        if (target == null) { body.linearVelocity = Vector2.zero; return; }

        Vector2 toTarget = ((Vector2)target.position - body.position).normalized;
        Vector2 direction = toTarget + Separation() * separationStrength;
        body.linearVelocity = direction.normalized * speed;
    }

    // Suma wektorów „od sąsiada”: im bliżej sąsiad, tym mocniej odpycha.
    private Vector2 Separation()
    {
        Vector2 push = Vector2.zero;
        int count = Physics2D.OverlapCircle(body.position, separationRadius, ContactFilter2D.noFilter, Neighbours);
        for (int i = 0; i < count; i++)
        {
            Collider2D other = Neighbours[i];
            if (other.attachedRigidbody == body || other.GetComponent<Enemy>() == null) continue;

            Vector2 away = body.position - (Vector2)other.transform.position;
            float distance = away.magnitude;
            if (distance < 0.001f) away = UnityEngine.Random.insideUnitCircle; // idealnie na sobie: losowy kierunek
            push += away.normalized * (1f - distance / separationRadius);
        }
        return push;
    }

    public void AddSpeed(float bonus) => speed += bonus;

    public void TakeDamage(int amount)
    {
        if (health <= 0) return; // kilka pocisków w jednej klatce nie zabija dwa razy

        health -= amount;
        flashUntil = Time.time + hitFlashDuration;
        if (health <= 0)
        {
            Killed?.Invoke();
            Destroy(gameObject);
        }
    }
}
