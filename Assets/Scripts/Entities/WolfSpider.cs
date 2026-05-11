using UnityEngine;

public class WolfSpider : Enemy
{
    [SerializeField] int effectTurns = 1;
    [SerializeField] int speedBoost = 2;

    bool effectActive;

    protected override void SpecialAttack()
    {
        float damage = specialMoveDamage;

        player.TakeDamage(damage);

        speed += speedBoost;
        effectActive = true;

        UIManager.instance.ShowDialogue(name + " uses " + specialMove + "! It got faster!");

        EndTurn();
    }

    protected override void EndSpecialAttack()
    {
        if(effectTurns == 0 && effectActive)
        {
            speed -= speedBoost;
            effectActive = false;
        }
        else if(effectTurns > 0 &&  effectActive)
        {
            effectTurns--;
        }
    }
}
