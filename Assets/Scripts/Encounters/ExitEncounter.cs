using UnityEngine;

public class ExitEncounter : Encounter
{
    protected override void TriggerEncounter(Player player)
    {
        Debug.Log("Next Level");

        // UIManager.ShowEndingScreen (make ending screen take back to Main Menu, or to level 2 if we have one)
    }
}
