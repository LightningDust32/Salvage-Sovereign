using UnityEngine;

public enum ItemType
{
    Armour,
    Gear,
    WeaponMod,
    None
}

public enum Rarity
{
    Common,
    Uncommon,
    Rare
}

[CreateAssetMenu(fileName = "HarvestItem", menuName = "Combat/HarvestItem")]
public class HarvestItem : ScriptableObject
{
    [Header("Info")]
    public string itemName;
    public int sellValue;
    [SerializeField] private Rarity rarity;

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
    [SerializeField] private Material material;

    bool isEquipped = false;

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

    public Rarity GetRarity()
    {
        return rarity;
    }

    public void SetEquipped(bool state)
    {
        isEquipped = state;
    }

    public Material GetMaterial()
    {
        return material;
    }

    public bool GetIsEquipped()
    {
        return isEquipped;
    }
}
