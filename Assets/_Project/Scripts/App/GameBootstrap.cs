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
        [Header("Dane")]
        [SerializeField] private PlayerDefinition playerDefinition;
        [SerializeField] private WeaponDefinition weaponDefinition;
        [SerializeField] private EnemyDefinition enemyDefinition; // tymczasowo: w kroku 5 wrogowie przyjdą z WaveConfig

        [Header("Scena")]
        [SerializeField] private Camera mainCamera;
        [SerializeField] private PlayerMover playerMover;
        [SerializeField] private PlayerShooter playerShooter;
        [SerializeField] private EnemySpawner enemySpawner;

        private void Awake()
        {
            var random = new System.Random();
            var weapon = new Weapon(weaponDefinition.ShotsPerSecond);

            playerMover.Init(playerDefinition);
            playerShooter.Init(weapon, weaponDefinition, mainCamera);
            enemySpawner.Init(enemyDefinition, playerMover.transform, random);
        }
    }
}
