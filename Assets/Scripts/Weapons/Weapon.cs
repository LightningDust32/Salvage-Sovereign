using UnityEngine;
public enum DamageType
{
    Slash,
    Smash,
    Pierce
}

public abstract class Weapon : MonoBehaviour
{
    [Header("Weapon Stats")]
    [SerializeField] protected float damage = 2f;
    [SerializeField] protected float speed = 1f; 
    [SerializeField] protected DamageType damageType;

    [SerializeField] private HarvestItem currentMod; // serialized so you can see if it equipped in editor


    public virtual void Use(Entity attacker, Entity target)
    {
        if (target == null || !target.IsAlive())
        {
            Debug.Log("Invalid target");
            return;
        }

        Debug.Log(attacker.name + " attacks " + target.name + " with " + damageType);

        target.TakeDamage(GetDamage());
    }

    public float GetDamage()
    {
        float finalDamage = damage;

        if (currentMod != null)
        {
            finalDamage += currentMod.GetStrengthBonus();
        }

        return finalDamage;
    }

    public float GetSpeed()
    {
        return speed;
    }

    public DamageType GetDamageType()
    {
        if (currentMod != null && currentMod.ChangesDamageType())
        {
            return currentMod.GetDamageType();
        }

        return damageType;
    }

    public HarvestItem GetCurrentMod()
    {
        return currentMod;
    }

    public void EquipMod(HarvestItem mod)
    {
        if (mod == null) return;

        if (mod.GetItemType() != ItemType.WeaponMod)
        {
            Debug.Log("Tried to equip non-weapon mod");
            return;
        }

        currentMod = mod;
    }
}