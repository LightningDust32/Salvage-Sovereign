using UnityEngine;
// Manager for permanent upgrades from the lobby area, and weapon selecting
public static class PersistentData
{
    // Currency
    public static int Gold;

    // Permanent stats
    public static int bonusHealth;
    public static int bonusStrength;
    public static int bonusLuck;
    public static int bonusSpeed;
    public static int bonusStamina;

    // Weapon selection (store as index)
    public static int primaryWeaponIndex = -1;
    public static int secondaryWeaponIndex = -1;

    public static void Save()
    {
        PlayerPrefs.SetInt("Gold", Gold);

        PlayerPrefs.SetInt("BonusHealth", bonusHealth);
        PlayerPrefs.SetInt("swordDamage", bonusStrength);
        PlayerPrefs.SetInt("bonusLuck", bonusLuck);
        PlayerPrefs.SetInt("bonusSpeed", bonusSpeed);
        PlayerPrefs.SetInt("BonusStamina", bonusStamina);

        PlayerPrefs.SetInt("PrimaryWeapon", primaryWeaponIndex);
        PlayerPrefs.SetInt("SecondaryWeapon", secondaryWeaponIndex);

        PlayerPrefs.Save();
    }

    public static void Load()
    {
        Gold = PlayerPrefs.GetInt("Gold", 0);

        bonusHealth = PlayerPrefs.GetInt("BonusHealth", 0);
        bonusStrength = PlayerPrefs.GetInt("swordDamage", 0);
        bonusLuck = PlayerPrefs.GetInt("bonusLuck", 0);
        bonusSpeed = PlayerPrefs.GetInt("bonusSpeed", 0);
        bonusStamina = PlayerPrefs.GetInt("BonusStamina", 0);

        primaryWeaponIndex = PlayerPrefs.GetInt("PrimaryWeapon", -1);
        secondaryWeaponIndex = PlayerPrefs.GetInt("SecondaryWeapon", -1);
    }

    public static void ResetRunSelections()
    {
        primaryWeaponIndex = -1;
        secondaryWeaponIndex = -1;
    }
}