using UnityEngine;

public enum BodyPart
{
    Head,
    Torso,
    Arms,
    Legs
}

public class Enemy : Entity
{
    private bool isMyTurn = false;
    private bool turnFinished = false;

    [SerializeField] private DamageType weakness;
    [SerializeField] private DamageType resistance;

    [SerializeField] string specialMove;
    [SerializeField] int specialMoveDamage;
    [Range(0, 1)]
    [SerializeField] float attackChance;

    [SerializeField] private HarvestItem dropItem;
    [SerializeField] private float baseDropChance;

    private Player player;

    [System.Serializable]
    public struct BodyPartData
    {
        public BodyPart part;
        public float damageMultiplier;
    }

    [SerializeField] private BodyPartData[] bodyParts;

    public override bool TakeTurn()
    {
        if (!isMyTurn) 
        {
            Debug.Log(name + " takes its turn");

            isMyTurn = true;
            turnFinished = false;
            // Placeholder AI
            // Later: attack player, special move, etc.
            PerformAction();

        }
        return turnFinished;
    }

    public override void TakeDamage(float damage)
    {
        // Reduce or increase damage from weakness/resistance, then pass it to the rest of the TakeDamage calculation
        base.TakeDamage(damage);
    }

    private void PerformAction()
    {
        if (player == null)
        {
            Debug.Log("Enemy has no player target");
            EndTurn();
            return;
        }

        float roll = Random.value;

        if (roll < attackChance)
        {
            BasicAttack();
        }
        else
        {
            SpecialAttack();
        }
    }

    private void BasicAttack()
    {
        float damage = strength;

        player.TakeDamage(damage);

        Debug.Log(name + " attacks for " + damage);

        EndTurn();
    }

    private void SpecialAttack()
    {
        float damage = specialMoveDamage;

        player.TakeDamage(damage);

        Debug.Log(name + " uses " + specialMove + " for " + damage);

        EndTurn();
    }

    private void EndTurn()
    {
        isMyTurn = false;
        turnFinished = true;

        Debug.Log(name + " ends its turn");
    }

    public float GetMultiplier(BodyPart targetPart)
    {
        foreach(var part in bodyParts)
        {
            if(part.part == targetPart)
            {
                return part.damageMultiplier;
            }
        }

        return 1f; // no damage multiplier
    }

    public HarvestItem TryDrop()
    {
        float roll = Random.value;

        if(roll <= baseDropChance)
        {
            return dropItem;
        }

        return null;
    }

    // call in Encounter or turn 1, so enemy stores a reference to the player
    public void SetPlayer(Player target)
    {
        player = target;
    }
}
