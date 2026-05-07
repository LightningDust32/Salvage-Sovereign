using UnityEngine;

public enum ItemType
{
    Armour, // Armour is for items in the top centre slot of inventory mockup
    Gear, // Gear is for second slot down
    WeaponMod // WeaponMod is for equipping to weapon
}

[CreateAssetMenu(fileName = "HarvestItem", menuName = "Combat/HarvestItem")]
public class HarvestItem : ScriptableObject
{
    [Header("Info")]
    public string itemName;
    public int sellValue;

    [Header("Type")]
    [SerializeField] private ItemType itemType;

    [Header("Stat Bonuses")]
    [SerializeField] private float healthBonus;
    [SerializeField] private float staminaBonus;
    [SerializeField] private float strengthBonus;
    [SerializeField] private float defenseBonus;
    [SerializeField] private float speedBonus;

    [Header("Weapon Modifiers")]
    [SerializeField] private DamageType overrideDamageType;
    [SerializeField] private bool changeDamageType;

    public ItemType GetItemType()
    {
        return itemType;
    }

    public float GetHealthBonus()
    {
        return healthBonus;
    }

    public float GetStaminaBonus()
    {
        return staminaBonus;
    }

    public float GetStrengthBonus()
    {
        return strengthBonus;
    }

    public float GetDefenseBonus()
    {
        return defenseBonus;
    }

    public float GetSpeedBonus()
    {
        return speedBonus;
    }

    public bool ChangesDamageType()
    {
        return changeDamageType;
    }

    public DamageType GetDamageType()
    {
        return overrideDamageType;
    }
}
