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

        [Header("Scena")]
        [SerializeField] private PlayerMover player;

        private void Awake()
        {
            player.Init(playerDefinition);
        }
    }
}
