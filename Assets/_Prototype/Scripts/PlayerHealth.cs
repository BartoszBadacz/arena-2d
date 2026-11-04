using System;
using UnityEngine;

// Prototyp: życie gracza, obrażenia przy dotyku wroga, chwilowa nietykalność po trafieniu.
public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 5;
    [SerializeField] private int contactDamage = 1;
    [SerializeField] private float invulnerabilityDuration = 1f;
    [SerializeField] private float blinkInterval = 0.1f;

    public event Action Died;

    public int Health { get; private set; }
    public int MaxHealth => maxHealth;

    private SpriteRenderer spriteRenderer;
    private float invulnerableUntil;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        Health = maxHealth;
    }

    private void Update()
    {
        // Miganie w czasie nietykalności.
        bool invulnerable = Time.time < invulnerableUntil;
        spriteRenderer.enabled = !invulnerable || Mathf.FloorToInt(Time.time / blinkInterval) % 2 == 0;
    }

    // Wywoływane co krok fizyki, dopóki gracz dotyka jakiegoś collidera.
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.GetComponent<Enemy>() != null) TakeDamage(contactDamage);
    }

    private void TakeDamage(int amount)
    {
        if (Health <= 0 || Time.time < invulnerableUntil) return;

        Health -= amount;
        invulnerableUntil = Time.time + invulnerabilityDuration;
        if (Health <= 0) Died?.Invoke();
    }
}
