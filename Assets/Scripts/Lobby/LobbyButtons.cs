using UnityEngine;

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
        PersistentData.Save();
    }

    public void SelectSecondary(int index)
    {
        PersistentData.secondaryWeaponIndex = index;
        PersistentData.Save();
    }
}
