using UnityEngine;

namespace Arena.Gameplay
{
    // Pocisk leci prosto i znika po czasie albo po trafieniu w coś.
    // Co z czym się zderza, decydują warstwy (Layer Collision Matrix), nie kod: pocisk gracza ignoruje gracza i inne pociski.
    [RequireComponent(typeof(Rigidbody2D))]
    public class Projectile : MonoBehaviour
    {
        public void Launch(Vector2 velocity, float lifetime)
        {
            GetComponent<Rigidbody2D>().linearVelocity = velocity;
            Destroy(gameObject, lifetime);
        }

        // Collider pocisku jest „triggerem”: nie odbija się, tylko zgłasza, że w coś wszedł.
        private void OnTriggerEnter2D(Collider2D other)
        {
            Destroy(gameObject);
        }
    }
}
