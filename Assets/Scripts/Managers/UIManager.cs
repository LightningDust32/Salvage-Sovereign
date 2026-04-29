using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("UI Elements")]
    [SerializeField] private TMP_Text promptBox;
    [SerializeField] private TMP_Text dialogueBox;
    [SerializeField] GameObject pauseScreen;
    [SerializeField] private Image staminaBar;
    [SerializeField] private Image healthBar;
    [SerializeField] GameObject endingScreen;
    [SerializeField] GameObject controlsScreen;

    [Header("UI Puzzle")]
    [SerializeField] GameObject questionObject;
    [SerializeField] private TMP_Text questionText;
    [SerializeField] TMP_InputField answerBox;

    [Header("Dialogue Settings")]
    [SerializeField] private float defaultTime;

    private float dialogueTimer = 0f;

    [Header("Compendium")]
    [SerializeField] GameObject compendiumScreen;
    [SerializeField] TMP_Text entryText;

    private List<string> collectedEntries = new List<string>();
    private StringBuilder entryBuilder = new StringBuilder();


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    private void Update()
    {
        if (dialogueTimer > 0f)
        {
            dialogueTimer -= 1 * Time.deltaTime;

            if (dialogueTimer <= 0f)
            {
                dialogueBox.text = "";
            }
        }
    }

    public void SetPrompt(string prompt)
    {
        promptBox.text = prompt;
    }

    public void ClearPrompt()
    {
        promptBox.text = "";
    }
    // Overloaded ShowDialogue to use default timer if no float is passed in, or set the timer to the float passed in.
    public void ShowDialogue(string message)
    {
        ShowDialogue(message, defaultTime);
    }

    public void ShowDialogue(string message, float time)
    {
        dialogueBox.text = message;
        dialogueTimer = time;
    }

    public void Pause()
    {
        if (Time.timeScale > 0f)
        {
            Time.timeScale = 0.0f;
            pauseScreen.SetActive(true);
        }
        else
        {
            Time.timeScale = 1.0f;
            pauseScreen.SetActive(false);
        }
    }

    // All bar setters do the same things, setting the percentage of the UI bar proportional to their related stat elswhere.
    public void SetHealthBar(float percent)
    {
        if (healthBar == null)
        {
            return;
        }  

        healthBar.fillAmount = percent;
    }

    public void SetStaminaBar(float percent)
    {
        if (staminaBar == null)
            return;

        staminaBar.fillAmount = Mathf.Lerp(staminaBar.fillAmount, percent, 10f * Time.deltaTime);
    }

    public void End()
    {
        Time.timeScale = 0.0f;
        endingScreen.SetActive(true);
    }

    // Compiles compendium entries into the compendium's text
    public void AddEntry(string entryFound)
    {
        if (collectedEntries.Contains(entryFound))
            return;

        collectedEntries.Add(entryFound);

        entryBuilder.Clear();

        foreach (string entry in collectedEntries)
        {
            entryBuilder.AppendLine(entry);
        }

        entryText.text = entryBuilder.ToString();

        ShowDialogue("Entry Added!");
    }

    // Screen activity handlers, setting appropriate UI elements on and off.
    public void OpenCompendium()
    {
        compendiumScreen.SetActive(true);
        pauseScreen.SetActive(false);
    }

    public void CloseCompendium()
    {
        compendiumScreen.SetActive(false);
        controlsScreen.SetActive(false);
        pauseScreen.SetActive(true);
    }

    public void OpenControls()
    {
        controlsScreen.SetActive(true);
        pauseScreen.SetActive(false);
    }

    public void BackToMenu()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(0);
    }

    public void ResetLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(1);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
