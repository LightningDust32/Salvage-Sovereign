using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyButtons : MonoBehaviour
{
    [SerializeField] GameObject upgradeScreen;
    [SerializeField] GameObject weaponScreen;
    [SerializeField] TMP_Text goldtext;
    [SerializeField] TMP_Text healthCost;
    [SerializeField] TMP_Text staminaCost;
    [SerializeField] TMP_Text strengthCost;
    [SerializeField] TMP_Text luckCost;
    [SerializeField] TMP_Text speedCost;

    [SerializeField] int upgradeLimit = 5;

    private void Awake()
    {
        PersistentData.Load();
        SetGoldText();

        SetCost(healthCost, CalculateUpgradeCost(25, PersistentData.bonusHealth));
        SetCost(staminaCost, CalculateUpgradeCost(25, PersistentData.bonusStamina));
        SetCost(strengthCost, CalculateUpgradeCost(25, PersistentData.bonusStrength));
        SetCost(speedCost, CalculateUpgradeCost(25, PersistentData.bonusSpeed));
        SetCost(luckCost, CalculateUpgradeCost(25, PersistentData.bonusLuck));
    }

    public void UpgradeHealth(int cost)
    {
        if (PersistentData.bonusHealth == upgradeLimit)
        {
            Debug.Log("Maxed upgrade");
            return;
        }

        if (PersistentData.bonusHealth > 0)
        {
            cost *= PersistentData.bonusHealth;
        }

        if (PersistentData.Gold < cost)
        {
            Debug.Log("Not enough Gold");
            return;
        }

        PersistentData.Gold -= cost;
        PersistentData.bonusHealth++;

        SetCost(healthCost, CalculateUpgradeCost(cost, PersistentData.bonusHealth));

        SetGoldText();
        PersistentData.Save();

        Debug.Log("Health upgraded");
    }

    public void UpgradeStrength(int cost)
    {
        if (PersistentData.bonusStrength == upgradeLimit)
        {
            Debug.Log("Maxed upgrade");
            return;
        }

        if (PersistentData.bonusStrength > 0)
        {
            cost *= PersistentData.bonusStrength;
        }

        if (PersistentData.Gold < cost)
        {
            Debug.Log("Not enough Gold");
            return;
        }

        PersistentData.Gold -= cost;
        PersistentData.bonusStrength++;

        SetCost(strengthCost, CalculateUpgradeCost(cost, PersistentData.bonusStrength));

        SetGoldText();
        PersistentData.Save();

        Debug.Log("Strengthupgraded");
    }

    public void UpgradeStamina(int cost)
    {
        if (PersistentData.bonusStamina == upgradeLimit)
        {
            Debug.Log("Maxed upgrade");
            return;
        }

        if (PersistentData.bonusStamina > 0)
        {
            cost *= PersistentData.bonusStamina;
        }

        if (PersistentData.Gold < cost)
        {
            Debug.Log("Not enough Gold");
            return;
        }

        PersistentData.Gold -= cost;
        PersistentData.bonusStamina++;

        SetCost(staminaCost, CalculateUpgradeCost(cost, PersistentData.bonusStamina));

        SetGoldText();
        PersistentData.Save();

        Debug.Log("Stamina upgraded");
    }

    public void UpgradeSpeed(int cost)
    {
        if (PersistentData.bonusSpeed == upgradeLimit)
        {
            Debug.Log("Maxed upgrade");
            return;
        }

        if (PersistentData.bonusSpeed > 0)
        {
            cost *= PersistentData.bonusSpeed;
        }

        if (PersistentData.Gold < cost)
        {
            Debug.Log("Not enough Gold");
            return;
        }

        PersistentData.Gold -= cost;
        PersistentData.bonusSpeed++;

        SetCost(speedCost, CalculateUpgradeCost(cost, PersistentData.bonusSpeed));

        SetGoldText();
        PersistentData.Save();

        Debug.Log("Speed upgraded");
    }

    public void UpgradeLuck(int cost)
    {
        if (PersistentData.bonusLuck == upgradeLimit)
        {
            Debug.Log("Maxed upgrade");
            return;
        }

        if (PersistentData.bonusLuck > 0)
        {
            cost *= PersistentData.bonusLuck;
        }

        if (PersistentData.Gold < cost)
        {
            Debug.Log("Not enough Gold");
            return;
        }

        PersistentData.Gold -= cost;
        PersistentData.bonusLuck++;

        SetCost(luckCost, CalculateUpgradeCost(cost, PersistentData.bonusLuck));

        SetGoldText();
        PersistentData.Save();

        Debug.Log("Luck upgraded");
    }

    public void ChooseWeapon(int index)
    {
        
        if (PersistentData.primaryWeaponIndex == -1)
        {
            PersistentData.primaryWeaponIndex = index;
            Debug.Log("Primary weapon selected: " + index);
        }
        else if (PersistentData.secondaryWeaponIndex == -1)
        {
            // Prevent duplicate selection
            if (index == PersistentData.primaryWeaponIndex)
            {
                PersistentData.primaryWeaponIndex = -1;
                Debug.Log("Primary Weapon Unequipped");
                return;
            }

            PersistentData.secondaryWeaponIndex = index;
            Debug.Log("Secondary weapon selected: " + index);
        }
        else
        {
            PersistentData.secondaryWeaponIndex = -1;
            Debug.Log("Secondary Weapon Unequipped");
            return;
        }

        PersistentData.Save();
    }

    public bool CanStartRun()
    {
        return PersistentData.primaryWeaponIndex != -1 &&
               PersistentData.secondaryWeaponIndex != -1;
    }

    public void StartRun()
    {
        if (!CanStartRun()) return;

       SceneManager.LoadScene(3);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void SetGoldText()
    {
        goldtext.text = "Gold: " + PersistentData.Gold;
    }

    public void SetCost(TMP_Text text, int cost)
    {
        text.text = "Cost: " + cost;
    }

    private int CalculateUpgradeCost(int baseCost, int currentLevel)
    {
        if (currentLevel > 0)
        {
            return baseCost * currentLevel;
        }

        return baseCost;
    }

    public void DisableScreens()
    {
        upgradeScreen.SetActive(false);
        weaponScreen.SetActive(false);
    }
}
