using UnityEngine;

public class WolfSpider : Enemy
{
    [SerializeField] int effectTurns = 1;
    [SerializeField] int speedBoost = 2;

    protected override void SpecialAttack()
    {
        float damage = specialMoveDamage;

        player.TakeDamage(damage);

        speed += speedBoost;

        UIManager.instance.ShowDialogue(name + " uses " + specialMove + "! It got faster!");

        EndTurn();
    }
}
