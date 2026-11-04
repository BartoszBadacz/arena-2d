using UnityEngine;

// Prototyp: pocisk leci prosto i znika po czasie albo po trafieniu w coś.
[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 18f;
    [SerializeField] private float lifetime = 2f;
    [SerializeField] private int damage = 1;

    public void Launch(Vector2 direction)
    {
        GetComponent<Rigidbody2D>().linearVelocity = direction * speed;
        Destroy(gameObject, lifetime);
    }

    // Collider pocisku jest „triggerem”: nie odbija się, tylko zgłasza, że w coś wszedł.
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Pociski się nawzajem ignorują. Docelowo: warstwy + Layer Collision Matrix.
        if (other.GetComponent<Bullet>() != null) return;

        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null) enemy.TakeDamage(damage);

        Destroy(gameObject);
    }
}
