using UnityEngine;

namespace Arena.Data
{
    // Liczby gracza w jednym pliku danych (Data/Player.asset). Kod ich nie zawiera — dostaje je z zewnątrz.
    // Dane są tylko do odczytu: zmiana ScriptableObjectu w trybie Play zostaje w pliku na stałe.
    [CreateAssetMenu(fileName = "Player", menuName = "Arena/Player Definition")]
    public class PlayerDefinition : ScriptableObject
    {
        [SerializeField, Min(0f)] private float moveSpeed = 6f;
        [SerializeField, Min(1)] private int maxHealth = 5;
        [Tooltip("Sekundy nietykalności po trafieniu.")]
        [SerializeField, Min(0f)] private float invulnerabilityDuration = 1f;

        public float MoveSpeed => moveSpeed;
        public int MaxHealth => maxHealth;
        public float InvulnerabilityDuration => invulnerabilityDuration;
    }
}
