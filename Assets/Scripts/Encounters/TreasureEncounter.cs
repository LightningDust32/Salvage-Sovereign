using UnityEngine;

public class TreasureEncounter : Encounter
{
    int goldAmount;

    protected override void TriggerEncounter(Player player)
    {
        goldAmount = Random.Range(10, 30);

        player.ChangeGold(goldAmount);

        CompleteEncounter();
    }
}
