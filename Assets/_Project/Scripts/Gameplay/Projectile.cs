using Arena.Core;
using UnityEngine;

namespace Arena.Gameplay
{
    // Pocisk leci prosto i znika po czasie albo po trafieniu w coś.
    // Co z czym się zderza, decydują warstwy (Layer Collision Matrix), nie kod: pocisk gracza ignoruje gracza i inne pociski.
    [RequireComponent(typeof(Rigidbody2D))]
    public class Projectile : MonoBehaviour
    {
        private int damage;
        private bool spent;

        public void Launch(Vector2 velocity, float lifetime, int damage)
        {
            this.damage = damage;
            GetComponent<Rigidbody2D>().linearVelocity = velocity;
            Destroy(gameObject, lifetime);
        }

        // Collider pocisku jest „triggerem”: nie odbija się, tylko zgłasza, że w coś wszedł.
        private void OnTriggerEnter2D(Collider2D other)
        {
            // Destroy działa dopiero na końcu klatki — bez tej flagi pocisk mógłby trafić dwóch wrogów naraz.
            if (spent) return;
            spent = true;

            if (other.TryGetComponent(out IHasHealth target)) target.Health.TakeDamage(damage, Time.time);
            Destroy(gameObject);
        }
    }
}
