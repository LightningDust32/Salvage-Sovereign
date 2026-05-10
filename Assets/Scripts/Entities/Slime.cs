using UnityEngine;

public class Slime : Enemy
{
    [SerializeField] int effectTurns = 2;
    [SerializeField] int weaknessAmount = 4;

    protected override void SpecialAttack()
    {
        float damage = specialMoveDamage;

        player.TakeDamage(damage);

        player.SetStatusTurns(effectTurns);

        player.ChangeDefense(-weaknessAmount);

        UIManager.instance.ShowDialogue(name + " uses " + specialMove + "! You are weakened!");

        EndTurn();
    }
}
