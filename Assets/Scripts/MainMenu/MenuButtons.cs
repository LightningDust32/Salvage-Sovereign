using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
    public void GameCredits()
    {
        // Switch canvas to show names and such
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
