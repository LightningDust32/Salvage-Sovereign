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

    
    public virtual void Use(Entity attacker, Entity target)
    {
        if (target == null || !target.IsAlive())
        {
            Debug.Log("Invalid target");
            return;
        }

        Debug.Log(attacker.name + " attacks " + target.name + " with " + damageType);

        // For now: simple direct damage
        target.TakeDamage(damage);
    }

    public float GetDamage()
    {
        return damage;
    }

    public float GetSpeed()
    {
        return speed;
    }

    public DamageType GetDamageType()
    {
        return damageType;
    }

    public void ApplyUpgrade(HarvestItem item)
    {
        damage += item.damageChange;
        speed += item.speedChange;

        if(item.changeDamageType)
        {
            damageType = item.damageTypeChange;
        }
    }
}