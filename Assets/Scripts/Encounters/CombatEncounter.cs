using UnityEngine;

public class CombatEncounter : Encounter
{
    protected override void TriggerEncounter(Player player)
    {
        Debug.Log("Combat Started");

        // TODO: Start combat system

        CompleteEncounter();
    }
}
