using UnityEngine;

public abstract class Entity : MonoBehaviour, ITurnActor
{
    [Header("Stats")]
    [SerializeField] protected float maxHealth = 10f;
    [SerializeField] protected float strength = 2f;
    [SerializeField] protected float defense = 1f;
    [SerializeField] protected float speed = 1f;

    protected float currentHealth;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
    }

    public abstract void TakeTurn();

    public float GetSpeed()
    {
        return speed;
    }

    public bool IsAlive()
    {
        return currentHealth <= 0;
    }

    public virtual void TakeDamage(float damage)
    {
        damage = damage - defense;

        if (damage < 1)
        {
            damage = 1;
        }

        currentHealth -= damage;

        if (currentHealth < 0)
        {
            currentHealth = 0;
        }
    }

    public float GetHealthPercent()
    {
        return currentHealth / maxHealth;
    }
}
