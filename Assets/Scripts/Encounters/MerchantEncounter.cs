using UnityEngine;

public class MerchantEncounter : Encounter
{
    protected override void TriggerEncounter(Player player)
    {
        if (player == null) return;

        player.UpdateMerchantUI();

        player.SetInteractionState(true);
    }
}
