using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    /*

        things needed:
        List of entities
        Sort the entites by speed
        Battle function that loops entites based on speed and ends when the player or all enemies are dead.

    */

    private List<Entity> Entities;
    private List<Entity> TurnOrder;
    private int currentTurn = 0;

    private bool battleactive;

    void Start()
    {
        // get all entitnes and put them in a list

        startNewRound();
    }

    private void Update()
    {
        if (!battleactive) return;

        // sort all the entities and get the first one in the list

        // check if they are alive 
        // if not skip them and go to the next turn

        // have the entity do their turn

        // have a condition to have them be 'finished' letting the entity do its act before moving on
        // then move onto the next turn once finished

        /*
        bool finished = ...;
        if (finished) 
        {
            nextTurn();
        }
        */
    }

    void startNewRound()
    {
        // starts the combat
        battleactive = true;

        // fill in the turn order list based on speed highses first

        // set the index to 0

       // reset each entity
    }

    void nextTurn()
    {
        // once the first turn has happened next turn

        // add 1 to the current index and start the round again

        // if reached the end of the list
        // all entities have taken their turn
        // run startNewRound
    }

    bool BattleIsActive()
    {
        // add conditions for ending
        // player dies
        // all enemies killed
        return battleactive;
    }
}
