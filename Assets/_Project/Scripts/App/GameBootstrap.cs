using Arena.Core;
using Arena.Data;
using Arena.Gameplay;
using Arena.Presentation;
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
        [SerializeField] private WaveConfig waveConfig;
        [SerializeField] private ArenaConfig arenaConfig;

        [Header("Scena")]
        [SerializeField] private Camera mainCamera;
        [SerializeField] private PlayerMover playerMover;
        [SerializeField] private PlayerShooter playerShooter;
        [SerializeField] private PlayerBody playerBody;
        [SerializeField] private EnemySpawner enemySpawner;
        [SerializeField] private RunFlow runFlow;
        [SerializeField] private Hud hud;

        private void Awake()
        {
            sessionRecord ??= new SessionRecord();
            var random = new System.Random();

            // Logika (Core)
            var weapon = new Weapon(weaponDefinition.ShotsPerSecond);
            var playerHealth = new Health(playerDefinition.MaxHealth, playerDefinition.InvulnerabilityDuration);
            var stats = new RunStats();
            var session = new RunSession(playerHealth, stats, sessionRecord);
            var waves = new WaveDirector(waveConfig.ToSettings());
            var mixer = new EnemyMixer<EnemyDefinition>(waveConfig.BaseEnemy, waveConfig.ToMixInRules(), random);
            var arenaHalfSize = new System.Numerics.Vector2(arenaConfig.HalfSize.x, arenaConfig.HalfSize.y);
            var spawnPoints = new SpawnPointPicker(arenaHalfSize, arenaConfig.MinSpawnDistance, random);

            // Fakty → logika
            waves.WaveStarted += stats.ReachWave;
            waves.WaveStarted += wave => mixer.StartWave(wave, waves.WaveSize); // skład fali przed pierwszym spawnem
            enemySpawner.EnemyKilled += enemy =>
            {
                stats.RegisterKill(enemy.ScoreValue);
                playerHealth.Heal(enemy.HealOnKill);
            };

            // Wstrzyknięcie do obiektów sceny
            playerMover.Init(playerDefinition);
            playerShooter.Init(weapon, weaponDefinition, mainCamera);
            playerBody.Init(playerHealth);
            enemySpawner.Init(waves, mixer, spawnPoints, playerMover.transform, random, arenaHalfSize);
            runFlow.Init(session);
            hud.Init(playerHealth, stats, sessionRecord, session, waves);
        }
    }
}
