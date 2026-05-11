using UnityEngine;

public enum BodyPart
{
    Head,
    Body,
    WeakPoint,
    Legs
}

public abstract class Enemy : Entity
{
    private bool isMyTurn = false;
    private bool turnFinished = false;

    [SerializeField] private DamageType weakness;
    [SerializeField] private DamageType resistance;

    private BodyPartTarget[] targets;

    [Header("Stamina")]
    [SerializeField] protected float maxStamina = 10f;
    [SerializeField] protected float currentStamina = 10f;
    [SerializeField] protected float specialAttackCost = 5f;

    [SerializeField] protected string specialMove;
    [SerializeField] protected int specialMoveDamage;
    [Range(0, 1)]
    [SerializeField] float attackChance;

    [SerializeField] private HarvestItem dropItem;
    [SerializeField] private float baseDropChance;

    protected Player player;
    bool isAlive;

    [System.Serializable]
    public struct BodyPartData
    {
        public BodyPart part;
        public float damageMultiplier;

        public DamageType weakness;
        public DamageType resistance;

        public GameObject partObject;
    }

    [SerializeField] private BodyPartData[] bodyParts;

    private DamageType lastDamageType;
    private BodyPart lastTargetPart;
    private bool hasTargetPart = false;

    private void Start()
    {
        foreach(BodyPartData partData in bodyParts)
        {
            if (partData.partObject == null) continue;

            EnemyBodyPart clickable = partData.partObject.GetComponent<EnemyBodyPart>();

            if (clickable == null)
            {
                clickable = partData.partObject.AddComponent<EnemyBodyPart>();
            }

            clickable.Initialize(this, partData.part);
            clickable.SetActive(false);
        }
    }

    public override bool TakeTurn()
    {
        if (!isMyTurn) 
        {
            Debug.Log(name + " takes its turn");

            isMyTurn = true;
            turnFinished = false;

            PerformAction();

        }
        return turnFinished;
    }

    public override void TakeDamage(float damage)
    {
        float finalDamage = damage;

        if (lastDamageType == weakness)
        {
            finalDamage *= 1.5f;
            Debug.Log(name + " is weak to " + lastDamageType);
        }
        else if (lastDamageType == resistance)
        {
            finalDamage *= 0.5f;
            Debug.Log(name + " resists " + lastDamageType);
        }

        if (hasTargetPart)
        {
            foreach (BodyPartData partData in bodyParts)
            {
                if (partData.part == lastTargetPart)
                {
                    // Apply body part multiplier
                    finalDamage *= partData.damageMultiplier;

                    // Apply body part weakness/resistance
                    if (lastDamageType == partData.weakness)
                    {
                        finalDamage *= 1.25f;
                        Debug.Log(name + "'s " + partData.part + " is weak to " + lastDamageType);
                    }
                    else if (lastDamageType == partData.resistance)
                    {
                        finalDamage *= 0.75f;
                        Debug.Log(name + "'s " + partData.part + " resists " + lastDamageType);
                    }

                    break;
                }
            }
        }

        UIManager.instance.ShowDialogue(name + " takes " + finalDamage + " damage");

        base.TakeDamage(finalDamage);

        ClearHitData();
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
            if(currentStamina > specialAttackCost)
            {
                currentStamina -= specialAttackCost;
                SpecialAttack();
            }
            else
            {
                BasicAttack();
            }
        }
    }

    private void BasicAttack()
    {
        float damage = strength;

        player.TakeDamage(damage);

        UIManager.instance.ShowDialogue(name + " attacks for " + damage);

        EndTurn();
    }

    protected abstract void SpecialAttack();

    protected abstract void EndSpecialAttack();

    protected void EndTurn()
    {
        isMyTurn = false;
        turnFinished = true;

        EndSpecialAttack();

        Debug.Log(name + " ends its turn");
    }

    public override void Die()
    {
        currentHealth = -5;
    }

    public override void ResetTurn()
    {
        turnFinished = false;
        isMyTurn = false;
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

    public void SetLastDamageType(DamageType type)
    {
        lastDamageType = type;
    }

    public void SetTargetBodyPart(BodyPart part)
    {
        lastTargetPart = part;
        hasTargetPart = true;
    }

    public BodyPartTarget[] GetBodyPartTargets()
    {
        targets = GetComponentsInChildren<BodyPartTarget>();
        return targets;
    }

    private void ClearHitData()
    {
        hasTargetPart = false;
    }

    private bool targetingActive = false;

    public void SetTargetingActive(bool state)
    {
        targetingActive = state;

        BodyPartTarget[] bodyParts = GetComponentsInChildren<BodyPartTarget>();

        foreach (BodyPartTarget part in bodyParts)
        {
            part.gameObject.SetActive(state);
            Debug.Log("Part active: " + part.name + ": " + state);
        }
    }

    public bool IsTargetingActive()
    {
        return targetingActive;
    }

    public void FaceTarget(Transform target)
    {
        Vector3 direction = (target.position - transform.position);
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.01f) return;

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = lookRotation;
    }
}
