using UnityEngine;

public class ExitEncounter : Encounter
{
    protected override void TriggerEncounter(Player player)
    {
        player.SetInteractionState(true);

        UIManager.instance.End();
    }
}
