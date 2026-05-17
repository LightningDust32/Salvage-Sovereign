using UnityEngine;

public class HealingPotion : Consumable
{
    [SerializeField] int healAmount;

    public override void Use(Player player)
    {
        player.Heal(healAmount);
    }
}
