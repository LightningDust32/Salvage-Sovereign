using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    [SerializeField] GameObject menu;
    [SerializeField] GameObject credits;

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
    public void GameCredits()
    {
        credits.SetActive(true);
        menu.SetActive(false);
    }

    public void Back()
    {
        credits.SetActive(false);
        menu.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
