using Arena.Core;
using Arena.Data;
using Arena.Gameplay;
using UnityEngine;

namespace Arena.App
{
    // Composition root: jedyne miejsce, które tworzy logikę (Core) i łączy ją z obiektami sceny.
    // Działa raz, w Awake, przed innymi skryptami. Nie ma Update ani logiki gry — dlatego to nie GameManager.
    [DefaultExecutionOrder(-1000)]
    public class GameBootstrap : MonoBehaviour
    {
        // Jedyny świadomy wyjątek od „bez statyków”: rekord ma przeżyć restart (przeładowanie sceny),
        // a zginąć po zamknięciu gry. Trwały zapis jest poza zakresem.
        private static SessionRecord sessionRecord;

        [Header("Dane")]
        [SerializeField] private PlayerDefinition playerDefinition;
        [SerializeField] private WeaponDefinition weaponDefinition;
        [SerializeField] private EnemyDefinition enemyDefinition; // tymczasowo: w kroku 5 wrogowie przyjdą z WaveConfig

        [Header("Scena")]
        [SerializeField] private Camera mainCamera;
        [SerializeField] private PlayerMover playerMover;
        [SerializeField] private PlayerShooter playerShooter;
        [SerializeField] private PlayerBody playerBody;
        [SerializeField] private EnemySpawner enemySpawner;
        [SerializeField] private RunFlow runFlow;

        private void Awake()
        {
            sessionRecord ??= new SessionRecord();
            var random = new System.Random();

            // Logika (Core)
            var weapon = new Weapon(weaponDefinition.ShotsPerSecond);
            var playerHealth = new Health(playerDefinition.MaxHealth, playerDefinition.InvulnerabilityDuration);
            var stats = new RunStats();
            var session = new RunSession(playerHealth, stats, sessionRecord);

            // Fakty z Gameplay → logika
            enemySpawner.EnemyKilled += _ => stats.RegisterKill();

            // Wstrzyknięcie do obiektów sceny
            playerMover.Init(playerDefinition);
            playerShooter.Init(weapon, weaponDefinition, mainCamera);
            playerBody.Init(playerHealth);
            enemySpawner.Init(enemyDefinition, playerMover.transform, random);
            runFlow.Init(session);
        }
    }
}
