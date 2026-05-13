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
    [SerializeField] private TMP_Text merchantListText;
    [SerializeField] private GameObject HealButton;

    private List<HarvestItem> merchantInventory = new List<HarvestItem>();
    private int selectedMerchantIndex = 0;


    [Header("Pause UI")]
    [SerializeField] GameObject pauseScreen;
    // Also would like to show the player stats here
    // along with their equipped weapon and its stats added to the players

    [Header("Inventory UI")]
    [SerializeField] GameObject inventoryScreen;

    // Updated inventory method
    [SerializeField] private TMP_Text inventoryListText; // centre

    [SerializeField] private TMP_Text selectedItemStats;

    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = Color.yellow;

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

    private List<HarvestItem> currentInventory = new List<HarvestItem>();
    private int selectedItemIndex = 0;

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
        if(prompt == null)
        {
            return;
        }

        if(promptBox == null)
        {
            return;
        }
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
        }
    }

    public void Inventory()
    {
        if (Time.timeScale > 0f)
        {
            Time.timeScale = 0.0f;
            inventoryScreen.SetActive(true);
            currentInventory = player.GetInventory();

            RefreshInventoryUI();
        }
        else
        {
            Time.timeScale = 1.0f;
            inventoryScreen.SetActive(false);
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

        BuildItemStats();
    }

    private void UpdateInventoryList()
    {

        StringBuilder builder = new StringBuilder();

        if (currentInventory.Count == 0)
        {
            builder.AppendLine("Empty");
        }
        else
        {
            for (int i = 0; i < currentInventory.Count; i++)
            {
                HarvestItem item = currentInventory[i];

                bool selected = i == selectedItemIndex;

                string color = selected ? ColorUtility.ToHtmlStringRGB(selectedColor) : ColorUtility.ToHtmlStringRGB(normalColor);

                string equippedMarker = item.GetIsEquipped() ? "E" : "-";

                builder.AppendLine($"<color=#{color}>{equippedMarker} {item.itemName}</color>");
            }
        }

        inventoryListText.text = builder.ToString();
    }

    private void BuildItemStats()
    {
        if (currentInventory.Count == 0)
        {
            selectedItemStats.text = "No Item Selected";
            return;
        }

        HarvestItem item = currentInventory[selectedItemIndex];

        StringBuilder builder = new StringBuilder();

        builder.AppendLine(item.itemName);
        builder.AppendLine();

        builder.AppendLine($"Type: {item.GetItemType()}");
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
            builder.AppendLine($"Damage Type: {item.GetDamageType()}");

        selectedItemStats.text = builder.ToString();
    }

    public void SelectNextInventoryItem()
    {
        if (!inventoryScreen.activeSelf)
            return;

        if (currentInventory.Count == 0)
            return;

        selectedItemIndex++;

        if (selectedItemIndex >= currentInventory.Count)
        {
            selectedItemIndex = 0;
        }

        RefreshInventoryUI();
    }

    public void SelectPreviousInventoryItem()
    {
        if (!inventoryScreen.activeSelf)
            return;

        if (currentInventory.Count == 0)
            return;

        selectedItemIndex--;

        if (selectedItemIndex < 0)
        {
            selectedItemIndex = currentInventory.Count - 1;
        }

        RefreshInventoryUI();
    }

    public void ConfirmInventorySelection()
    {
        if (!inventoryScreen.activeSelf)
            return;

        if (currentInventory.Count == 0)
            return;

        HarvestItem item = currentInventory[selectedItemIndex];

        switch (item.GetItemType())
        {
            case ItemType.Armour:
                player.EquipArmour(selectedItemIndex);
                break;

            case ItemType.Gear:
                player.EquipGear(selectedItemIndex);
                break;

            case ItemType.WeaponMod:
                player.EquipWeaponMod(selectedItemIndex, player.GetCurrentWeapon());
                break;
        }

        currentInventory = player.GetInventory();

        if (selectedItemIndex >= currentInventory.Count)
        {
            selectedItemIndex = Mathf.Max(0, currentInventory.Count - 1);
        }

        RefreshInventoryUI();
    }

    public bool InventoryOpen()
    {
        return inventoryScreen.activeSelf;
    }


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

    public void OpenMerchant(List<HarvestItem> inventory)
    {
        merchantScreen.SetActive(true);

        merchantInventory = inventory;

        selectedMerchantIndex = 0;

        RefreshMerchantUI();
    }

    public void RefreshMerchantUI()
    {
        UpdateMerchantList();
    }

    private void UpdateMerchantList()
    {
        StringBuilder builder = new StringBuilder();

        if (merchantInventory.Count == 0)
        {
            builder.AppendLine("Nothing to Sell");
        }
        else
        {
            for (int i = 0; i < merchantInventory.Count; i++)
            {
                HarvestItem item = merchantInventory[i];

                bool selected = i == selectedMerchantIndex;

                string color = selected ? ColorUtility.ToHtmlStringRGB(selectedColor) : ColorUtility.ToHtmlStringRGB(normalColor);

                string equippedMarker = item.GetIsEquipped() ? "E" : "-";

                builder.AppendLine($"<color=#{color}>{equippedMarker} {item.itemName}</color>");
            }
        }

        merchantListText.text = builder.ToString();
    }

    public void SelectNextMerchantItem()
    {
        if (!merchantScreen.activeSelf)
            return;

        if (merchantInventory.Count == 0)
            return;

        selectedMerchantIndex++;

        if (selectedMerchantIndex >= merchantInventory.Count)
        {
            selectedMerchantIndex = 0;
        }

        RefreshMerchantUI();
    }

    public void SelectPreviousMerchantItem()
    {
        if (!merchantScreen.activeSelf)
            return;

        if (merchantInventory.Count == 0)
            return;

        selectedMerchantIndex--;

        if (selectedMerchantIndex < 0)
        {
            selectedMerchantIndex = merchantInventory.Count - 1;
        }

        RefreshMerchantUI();
    }

    public void ConfirmMerchantSelection()
    {
        if (!merchantScreen.activeSelf)
            return;

        if (merchantInventory.Count == 0)
            return;

        player.SellItem(selectedMerchantIndex);

        merchantInventory = player.GetInventory();

        if (selectedMerchantIndex >= merchantInventory.Count)
        {
            selectedMerchantIndex = Mathf.Max(0, merchantInventory.Count - 1);
        }

        RefreshMerchantUI();
    }

    public bool MerchantOpen()
    {
        return merchantScreen.activeSelf;
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
        PersistentData.Gold += player.GetGold();
        PersistentData.Save();
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
        SceneManager.LoadScene(3);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void ExitEncounters()
    {
        player.SetInteractionState(false);
    }
}
