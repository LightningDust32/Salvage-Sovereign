using UnityEngine;

public class StrengthPotion : Consumable
{
    [SerializeField] int buffAmount;

    public override void Use(Player player)
    {
        player.ChangeStrength(buffAmount);
    }
}
