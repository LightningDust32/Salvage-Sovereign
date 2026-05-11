using UnityEngine;

public class Slime : Enemy
{
    [SerializeField] int effectTurns = 2;
    [SerializeField] int weaknessAmount = 4;

    bool effectActive;

    protected override void SpecialAttack()
    {
        float damage = specialMoveDamage;

        player.TakeDamage(damage);

        player.SetStatusTurns(effectTurns);

        player.ChangeDefense(-weaknessAmount);

        UIManager.instance.ShowDialogue(name + " uses " + specialMove + "! You are weakened!");

        EndTurn();
    }

    protected override void EndSpecialAttack()
    {
        if(player.RemainingStatusTurns() == 0 && effectActive)
        {
            player.ChangeDefense(weaknessAmount);
            effectActive = false;
        }
    }
}
