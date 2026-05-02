using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyButtons : MonoBehaviour
{
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

        PersistentData.Save();

        Debug.Log("Defense upgraded");
    }

    public void SelectPrimary(int index)
    {
        PersistentData.primaryWeaponIndex = index;
        Debug.Log("Primary Weapon: " + index);
        PersistentData.Save();
    }

    public void SelectSecondary(int index)
    {
        PersistentData.secondaryWeaponIndex = index;
        Debug.Log("Secondary Weapon: " + index);
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
}
