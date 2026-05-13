using UnityEngine;

public class WeaponEncounter : Encounter
{
    [SerializeField] GameObject weaponScreen;

    protected override void TriggerEncounter(Player player)
    {
        player.SetInteractionState(true);

        weaponScreen.SetActive(player.GetInteractionState());

        player.FaceTarget(transform);
    }
}
