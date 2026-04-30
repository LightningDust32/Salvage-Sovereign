using UnityEngine;

public class Enemy : Entity
{
    public override void TakeTurn()
    {
        Debug.Log(name + " takes its turn");

        // Placeholder AI
        // Later: attack player, move, etc.
    }
}
