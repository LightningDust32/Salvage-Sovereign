using System.Collections.Generic;
using UnityEngine;

public class CombatEncounter : Encounter
{
    [Header("Enemy Pool")]
    [SerializeField] private Enemy[] enemyPrefabs;

    private Enemy spawnedEnemy;

    protected override void TriggerEncounter(Player player)
    {
        StartCombat(player);

        CompleteEncounter();
    }

    private void StartCombat(Player player)
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0)
        {
            Debug.LogWarning("No enemies assigned to CombatEncounter");
            CompleteEncounter();
            return;
        }

        Enemy prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

        spawnedEnemy = Instantiate(prefab, transform.position, transform.rotation);

        player.FaceTarget(spawnedEnemy.transform);

        spawnedEnemy.FaceTarget(player.transform);

        spawnedEnemy.SetPlayer(player);

        List<Entity> entities = new List<Entity>
        {
            player,
            spawnedEnemy
        };

        if (TurnManager.Instance != null)
        {
            TurnManager.Instance.InitializeBattle(entities);
        }
        else
        {
            Debug.LogWarning("TurnManager missing in scene");
        }

        player.EnterCombat();

        if (UIManager.instance != null)
        {
            UIManager.instance.ShowCombatUI(true);
        }
    }

}
