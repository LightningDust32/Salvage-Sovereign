using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitEncounter : Encounter
{
    protected override void TriggerEncounter(Player player)
    {
        player.SetInteractionState(true);

        if(SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCount)
        {
            Debug.Log("Last Level won");
            UIManager.instance.End();
        }
        else
        {
            Debug.Log(SceneManager.GetActiveScene().buildIndex);
            Debug.Log(SceneManager.sceneCount);
            UIManager.instance.ShowNextLevel();
        }
    }
}
