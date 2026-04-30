using UnityEngine;

public class TreasureEncounter : Encounter
{
    protected override void TriggerEncounter(Player player)
    {
        Debug.Log("Treasure Opened");

        // Give loot here

        CompleteEncounter();
    }
}
