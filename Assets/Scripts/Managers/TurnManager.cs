using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Progress;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;

    private List<Entity> entities;
    private List<Entity> turnOrder;
    private int currentTurnIndex = 0;

    private Player player;
    private bool battleactive;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void InitializeBattle(List<Entity> newEntities)
    {
        entities = newEntities;

        player = newEntities.OfType<Player>().FirstOrDefault();

        StartNewRound();
    }

    private void Update()
    {
        if (!battleactive) return;

        if (!BattleIsActive()) return;

        // Safety check
        if (turnOrder == null || turnOrder.Count == 0) return;

        Entity current = turnOrder[currentTurnIndex];

        if (!current.IsAlive())
        {
            NextTurn();
            return;
        }

        bool finished = current.TakeTurn();

        if (finished)
        {
            NextTurn();
        }
    }

    void StartNewRound()
    {
        battleactive = true;

        // fill in the turn order list based on speed highest first
        turnOrder = entities
            .Where(entity => entity != null && entity.IsAlive())
            .OrderByDescending(entity => entity.GetSpeed())
            .ToList();
        
        currentTurnIndex = 0;

        foreach (Entity entity in turnOrder)
        {
            entity.ResetTurn();
        }
    }

    void NextTurn()
    {
        currentTurnIndex++;

        if (currentTurnIndex >= turnOrder.Count)
        {
            StartNewRound();
        }
    }

    bool BattleIsActive()
    {
        if (player == null)
            return true;

        if (!player.IsAlive())
        {
            EndBattle(false);
            return false;
        }

        bool anyEnemiesAlive = entities.Any(entity => entity is Enemy && entity.IsAlive());

        if (!anyEnemiesAlive)
        {
            EndBattle(true);
            return false;
        }

        return true;
    }

    public Enemy GetFirstAliveEnemy()
    {
        foreach (Entity entity in entities)
        {
            if (entity is Enemy enemy && enemy.IsAlive())
            {
                return enemy;
            }
        }

        return null;
    }

    public void EndBattle(bool playerWon)
    {
        battleactive = false;
        UIManager.instance.ShowDialogue(playerWon ? "Player Victory" : "Player Defeat");

        // Notify player
        if (player != null)
        {
            player.ExitCombat();
        }

        // Handle enemies
        foreach (Entity entity in entities)
        {
            if (entity is Enemy enemy)
            {
                if (playerWon)
                {
                    HarvestItem drop = enemy.TryDrop();

                    if (drop != null)
                    {
                        if (player.AddHarvestItem(drop))
                        {
                            Debug.Log("Collected: " + drop.itemName);
                        }
                    }
                }

                Destroy(enemy.gameObject);
            }
        }

        // Clear list
        entities.Clear();

        // Hide UI
        UIManager.instance.ShowCombatUI(false);
    }
}
