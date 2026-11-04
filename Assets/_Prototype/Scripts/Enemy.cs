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
        Vector2 direction = ((Vector2)target.position - body.position).normalized;
        body.linearVelocity = direction * speed;
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
