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
    [SerializeField] private Image staminaBar;
    [SerializeField] private Image healthBar;
    [SerializeField] GameObject endingScreen;
    [SerializeField] GameObject nextLevelScreen;
    [SerializeField] GameObject deathScreen;
    [SerializeField] GameObject controlsScreen;
    [SerializeField] GameObject combatScreen;
   

    [Header("Merchant UI")]
    [SerializeField] private GameObject merchantScreen;
    [SerializeField] private Button[] sellButtons;
    [SerializeField] private TMP_Text[] sellButtonTexts;
    [SerializeField] private GameObject HealButton;


    [Header("Pause UI")]
    [SerializeField] GameObject pauseScreen;
    // Also would like to show the player stats here
    // along with their equipped weapon and its stats added to the players

    [Header("Inventory UI")]
    [SerializeField] GameObject inventoryScreen;

    // Specific fields based on the example UI
    [SerializeField] private TMP_Text inventoryListText; // left hand list

    [SerializeField] private TMP_Text armourStatsText; // middle stats of equipped item
    [SerializeField] private TMP_Text gearStatsText;
    [SerializeField] private TMP_Text primaryWeaponStatsText;
    [SerializeField] private TMP_Text secondaryWeaponStatsText;

    [SerializeField] private Button armourButton; // right hand buttons to socket the item
    [SerializeField] private Button gearButton;
    [SerializeField] private Button primaryWeaponButton;
    [SerializeField] private Button secondaryWeaponButton;

    [SerializeField] private Button[] itemButtons; // Buttons that show up after the right hand button is pressed
    [SerializeField] private TMP_Text[] itemButtonTexts;
    [SerializeField] private TMP_Text playerGold;

    [Header("Dialogue Settings")]
    [SerializeField] private float defaultTime;

    private float dialogueTimer = 0f;

    [Header("Compendium")]
    [SerializeField] GameObject compendiumScreen;
    [SerializeField] TMP_Text entryText;

    private List<string> collectedEntries = new List<string>();
    private StringBuilder entryBuilder = new StringBuilder();

    private Player player;


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

    private void Start()
    {
        player = FindFirstObjectByType<Player>();
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

    public void ShowCombatUI(bool state)
    {
        combatScreen.SetActive(state);
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
            inventoryScreen.SetActive(false);
        }
    }

    public void Inventory()
    {
        if (Time.timeScale > 0f)
        {
            Time.timeScale = 0.0f;
            inventoryScreen.SetActive(true);
            pauseScreen.SetActive(false);

            RefreshInventoryUI();
        }
        else
        {
            Time.timeScale = 1.0f;
            inventoryScreen.SetActive(false);
            pauseScreen.SetActive(true);
        }
    }
    public void SetGoldText(int gold)
    {
        playerGold.text = $"Gold: {gold}";
    }

    public void RefreshInventoryUI()
    {
        if (player == null)
            return;

        UpdateInventoryList();

        armourStatsText.text = BuildItemStats(player.GetCurrentArmour());

        gearStatsText.text = BuildItemStats(player.GetCurrentGear());

        primaryWeaponStatsText.text = BuildWeaponStats(player.GetPrimaryWeapon());

        secondaryWeaponStatsText.text = BuildWeaponStats(player.GetSecondaryWeapon());
    }

    private void UpdateInventoryList()
    {
        List<HarvestItem> inventory = player.GetInventory();

        StringBuilder builder = new StringBuilder();

        builder.AppendLine("INVENTORY");
        builder.AppendLine();

        if (inventory.Count == 0)
        {
            builder.AppendLine("Empty");
        }
        else
        {
            for (int i = 0; i < inventory.Count; i++)
            {
                builder.AppendLine($"{i + 1}. {inventory[i].itemName}");
            }
        }

        inventoryListText.text = builder.ToString();
    }

    private string BuildItemStats(HarvestItem item)
    {
        if (item == null)
            return "Empty";

        StringBuilder builder = new StringBuilder();

        builder.AppendLine(item.itemName);
        builder.AppendLine();

        if (item.GetHealthBonus() != 0)
            builder.AppendLine($"Health: +{item.GetHealthBonus()}");

        if (item.GetStaminaBonus() != 0)
            builder.AppendLine($"Stamina: +{item.GetStaminaBonus()}");

        if (item.GetStrengthBonus() != 0)
            builder.AppendLine($"Strength: +{item.GetStrengthBonus()}");

        if (item.GetDefenseBonus() != 0)
            builder.AppendLine($"Defense: +{item.GetDefenseBonus()}");

        if (item.GetSpeedBonus() != 0)
            builder.AppendLine($"Speed: +{item.GetSpeedBonus()}");

        if (item.ChangesDamageType())
            builder.AppendLine($"Type: {item.GetDamageType()}");

        return builder.ToString();
    }

    private string BuildWeaponStats(Weapon weapon)
    {
        if (weapon == null)
            return "No Weapon";

        StringBuilder builder = new StringBuilder();

        builder.AppendLine(weapon.name);
        builder.AppendLine();

        builder.AppendLine($"Damage: {weapon.GetDamage()}");
        builder.AppendLine($"Type: {weapon.GetDamageType()}");

        HarvestItem mod = weapon.GetCurrentMod();

        if (mod != null)
        {
            builder.AppendLine();
            builder.AppendLine("MOD:");
            builder.AppendLine(mod.itemName);
        }

        return builder.ToString();
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

    public void SetMerchantText(string[] itemNames)
    {
        if (sellButtons == null || sellButtonTexts == null)
            return;

        merchantScreen.SetActive(true);

        for (int i = 0; i < sellButtons.Length; i++)
        {
            int index = i;

            if (i < itemNames.Length)
            {
                // Enable button
                sellButtons[i].gameObject.SetActive(true);

                // Set text
                sellButtonTexts[i].text = "Sell " + itemNames[i];

                // Clear previous listeners (IMPORTANT)
                sellButtons[i].onClick.RemoveAllListeners();

                // Add new listener
                sellButtons[i].onClick.AddListener(() =>
                {
                    if (player != null)
                    {
                        player.SellItem(index);
                    }
                });
            }
            else
            {
                // Disable unused slots
                sellButtons[i].gameObject.SetActive(false);
            }
        }
    }

    public void BuyHeal(int cost)
    {


        if (player != null && player.GetGold() >= cost)
        {
            player.Heal(player.GetMaxHealth() / 2);
            player.ChangeGold(-cost);

            HealButton.SetActive(false);
        }


    }

    public void CloseMerchant()
    {
        merchantScreen.SetActive(false);

        if (player != null)
        {
            player.SetInteractionState(false);
        }
    }

    public void End()
    {
        Time.timeScale = 0.0f;
        endingScreen.SetActive(true);
    }

    public void ShowNextLevel()
    {
        Time.timeScale = 0.0f;
        nextLevelScreen.SetActive(true);
    }

    public void NextLevel()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Death()
    {
        Time.timeScale = 0.0f;
        deathScreen.SetActive(true);
    }

    public void ContinueAfterDeath()
    {
        Time.timeScale = 1f;

        if (player != null)
        {
            player.EndRun();
        }
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
        inventoryScreen.SetActive(false);
    }

    public void CloseCompendium()
    {
        compendiumScreen.SetActive(false);
        controlsScreen.SetActive(false);
        inventoryScreen.SetActive(true);
    }

    public void OpenControls()
    {
        controlsScreen.SetActive(true);
        inventoryScreen.SetActive(false);
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
