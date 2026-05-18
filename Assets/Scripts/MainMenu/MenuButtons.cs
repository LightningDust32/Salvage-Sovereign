using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    [SerializeField] GameObject menuScreen;
    [SerializeField] GameObject creditsScreen;
    [SerializeField] GameObject settingsScreen;

    public void StartGame()
    {
        SceneManager.LoadScene(2);
    }
    public void GameCredits(bool credits)
    {
        if (credits != true)
        {
            creditsScreen.SetActive(true);
            menuScreen.SetActive(false);
        }
        else
        {
            creditsScreen.SetActive(false);
            menuScreen.SetActive(true);
        }

    }
    public void GameSettings(bool settings)
    {
        if (settings != true)
        {
            settingsScreen.SetActive(true);
            menuScreen.SetActive(false);
        }
        else
        {
            settingsScreen.SetActive(false);
            menuScreen.SetActive(true);
        }
    }


    public void QuitGame()
    {
        Application.Quit();
    }
}
