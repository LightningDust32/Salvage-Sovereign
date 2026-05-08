using UnityEngine;

public class Slime : Enemy
{
    protected override void SpecialAttack()
    {
        float damage = specialMoveDamage;

        player.TakeDamage(damage);

        UIManager.instance.ShowDialogue(name + " uses " + specialMove + " for " + damage);

        EndTurn();
    }
}
