using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    /*

        things needed:
        List of entities
        Sort the entites by speed
        Battle function that loops entites based on speed and ends when the player or all enemies are dead.

    */

    private List<Entity> entities;
    private List<Entity> turnOrder;
    private int currentTurnIndex = 0;

    private bool battleactive;

    void Start()
    {
        // get all entitnes and put them in a list
        entities = FindObjectsByType<Entity>(FindObjectsSortMode.None).ToList();

        StartNewRound();
    }

    private void Update()
    {
        if (!battleactive) return;

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

        // fill in the turn order list based on speed highses first
        turnOrder = entities
            .Where(e => e.IsAlive())                // check if alive
            .OrderByDescending(e => e.GetSpeed())   // order by speed
            .ToList();                              // add to list
        
        currentTurnIndex = 0;

        foreach (Entity entity in turnOrder)
        {
            entity.ResetTurn();
        }
    }

    void NextTurn()
    {
        // once the first turn has happened next turn
        currentTurnIndex++;
        // add 1 to the current index and start the round again

        // if reached the end of the list
        // all entities have taken their turn
        // run startNewRound
        if (currentTurnIndex >= turnOrder.Count)
        {
            StartNewRound();
        }
    }

    bool BattleIsActive()
    {
        // add conditions for ending
        // player dies
        // all enemies killed

        return battleactive;
    }
}
