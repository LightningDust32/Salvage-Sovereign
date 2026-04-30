using UnityEngine;

public class MerchantEncounter : Encounter
{
    protected override void TriggerEncounter(Player player)
    {
        Debug.Log("Open Merchant UI");
    }
}
