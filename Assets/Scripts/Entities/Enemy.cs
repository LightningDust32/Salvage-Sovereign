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

    [SerializeField] private HarvestItem dropItem;
    [SerializeField] private float baseDropChance;

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
            // Later: attack player, move, etc.
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
        Debug.Log(name + " attacks!");

        // TODO: pick a target and deal damage

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
}
