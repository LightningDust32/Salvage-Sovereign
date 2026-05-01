using UnityEngine;
// Manager for permanent upgrades from the lobby area, and weapon selecting
public static class PersistentData
{
    // Currency
    public static int Gold;

    // Permanent stats
    public static int bonusHealth;
    public static int bonusStrength;
    public static int bonusDefense;

    // Weapon selection (store as index)
    public static int primaryWeaponIndex = -1;
    public static int secondaryWeaponIndex = -1;

    public static void Save()
    {
        PlayerPrefs.SetInt("Gold", Gold);

        PlayerPrefs.SetInt("BonusHealth", bonusHealth);
        PlayerPrefs.SetInt("BonusStrength", bonusStrength);
        PlayerPrefs.SetInt("BonusDefense", bonusDefense);

        PlayerPrefs.SetInt("PrimaryWeapon", primaryWeaponIndex);
        PlayerPrefs.SetInt("SecondaryWeapon", secondaryWeaponIndex);

        PlayerPrefs.Save();
    }

    public static void Load()
    {
        Gold = PlayerPrefs.GetInt("Gold", 0);

        bonusHealth = PlayerPrefs.GetInt("BonusHealth", 0);
        bonusStrength = PlayerPrefs.GetInt("BonusStrength", 0);
        bonusDefense = PlayerPrefs.GetInt("BonusDefense", 0);

        primaryWeaponIndex = PlayerPrefs.GetInt("PrimaryWeapon", -1);
        secondaryWeaponIndex = PlayerPrefs.GetInt("SecondaryWeapon", -1);
    }

    public static void ResetRunSelections()
    {
        primaryWeaponIndex = -1;
        secondaryWeaponIndex = -1;
    }
}