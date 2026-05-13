using UnityEngine;

public class UpgradeEncounter : Encounter
{
    [SerializeField] GameObject upgradeScreen;

    protected override void TriggerEncounter(Player player)
    {
        player.SetInteractionState(true);

        upgradeScreen.SetActive(true);
    }
}
