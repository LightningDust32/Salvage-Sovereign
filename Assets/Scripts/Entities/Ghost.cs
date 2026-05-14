using UnityEngine;

public class Ghost : Enemy
{
    [SerializeField] int effectTurns = 1;
    [SerializeField] int effectStrength = 2;

    bool effectActive;

    protected override void SpecialAttack()
    {
        float damage = specialMoveDamage;

        player.TakeDamage(damage);

        player.SetStatusTurns(effectTurns);

        player.ChangeStrength(-effectStrength);

        UIManager.instance.ShowDialogue(name + " uses " + specialMove + "! You are scared!");

        EndTurn();
    }

    protected override void EndSpecialAttack()
    {
        if (player.RemainingStatusTurns() == 0 && effectActive)
        {
            player.ChangeStrength(effectStrength);
            effectActive = false;
        }
    }
}

