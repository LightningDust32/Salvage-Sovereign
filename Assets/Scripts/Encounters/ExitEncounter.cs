using UnityEngine;

public class ExitEncounter : Encounter
{
    protected override void TriggerEncounter(Player player)
    {
        Debug.Log("Fight Boss");

        // Start Combat encounter with Boss, then choose what to do on complete
    }
}
