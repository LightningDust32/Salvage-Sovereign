using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyButtons : MonoBehaviour
{
    [SerializeField] TMP_Text goldtext;

    private void Awake()
    {
        SetGoldText();
    }

    public void UpgradeHealth(int cost)
    {
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

        SetGoldText();
        PersistentData.Save();

        Debug.Log("Health upgraded");
    }

    public void UpgradeStrength(int cost)
    {
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
        SetGoldText();
        PersistentData.Save();

        Debug.Log("Strength upgraded");
    }

    public void UpgradeDefense(int cost)
    {
        if(PersistentData.bonusDefense > 0)
        {
            cost *= PersistentData.bonusDefense;
        }

        if (PersistentData.Gold < cost)
        {
            Debug.Log("Not enough Gold");
            return;
        }

        PersistentData.Gold -= cost;
        PersistentData.bonusDefense++;
        SetGoldText();
        PersistentData.Save();

        Debug.Log("Defense upgraded");
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
                Debug.Log("Weapon already selected as primary");
                return;
            }

            PersistentData.secondaryWeaponIndex = index;
            Debug.Log("Secondary weapon selected: " + index);
        }
        else
        {
            Debug.Log("Both weapons already selected");
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

       SceneManager.LoadScene(2);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void SetGoldText()
    {
        goldtext.text = "Gold: " + PersistentData.Gold;
    }
}
