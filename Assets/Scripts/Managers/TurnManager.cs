using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    /*

        things needed:
        Battle function that loops entites based on speed and ends when the player or all enemies are dead.

    */

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

    void Start()
    {
        // get all entities and put them in a list
        entities = FindObjectsByType<Entity>(FindObjectsSortMode.None).ToList();

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
        if (player == null || !player.IsAlive())
        {
            battleactive = false;
            Debug.Log("Battle ended: Player defeated");
            return battleactive;
        }

        bool anyEnemiesAlive = entities.Any(enemy => enemy is Enemy && enemy.IsAlive());

        if (!anyEnemiesAlive)
        {
            battleactive = false;
            Debug.Log("Battle ended: All enemies defeated");
            return battleactive;
        }


        return battleactive;
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
}
