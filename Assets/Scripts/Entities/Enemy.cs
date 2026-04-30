using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Enemy : Entity
{
    private bool isMyTurn = false;
    private bool turnFinished = false;

    public override bool TakeTurn()
    {
        if (!isMyTurn) 
        {
            Debug.Log(name + " takes its turn");

            isMyTurn = true;
            turnFinished = false;
            // Placeholder AI
            // Later: attack player, move, etc.
            PerformAction();

        }
        return turnFinished;
    }

    private void PerformAction()
    {
        Debug.Log(name + " attacks!");

        // TODO: pick a target and deal damage

        EndTurn();
    }
    private void EndTurn()
    {
        isMyTurn = false;
        turnFinished = true;

        Debug.Log(name + " ends its turn");
    }
}
