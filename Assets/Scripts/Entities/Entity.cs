using UnityEngine;

public class Entity : MonoBehaviour, ITurnActor
{
    [SerializeField] int maxHealth;
    private int currentHealth;

    [SerializeField] int strength;
    [SerializeField] int defense;
    [SerializeField] int speed;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeTurn()
    {

    }

    public int GetSpeed()
    {
        return speed;
    }

    public bool IsAlive()
    {
        return currentHealth <= 0;
    }

    public void TakeDamage(int damage)
    {
        damage = damage - defense;

        if (damage < 1)
        {
            damage = 1;
        }

        currentHealth -= damage;
    }
}
