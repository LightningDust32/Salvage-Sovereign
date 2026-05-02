using UnityEditor.Experimental.GraphView;
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

        Debug.Log(name + " takes " + finalDamage + " damage");

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

    public void SetLastDamageType(DamageType type)
    {
        lastDamageType = type;
    }

    public void SetTargetBodyPart(BodyPart part)
    {
        lastTargetPart = part;
        hasTargetPart = true;
    }

    private void ClearHitData()
    {
        hasTargetPart = false;
    }

    public void SetTargetingActive(bool active)
    {
        foreach (BodyPartData partData in bodyParts)
        {
            if (partData.partObject == null) continue;

            var clickable = partData.partObject.GetComponent<EnemyBodyPart>();
            if (clickable != null)
            {
                clickable.SetActive(active);
            }
        }
    }

    public void OnBodyPartClicked(BodyPart part)
    {
        Debug.Log(name + " body part clicked: " + part);

        SetTargetingActive(false);

        // Tell player to execute power attack
        Player player = TurnManager.Instance.GetComponent<TurnManager>() != null ? FindFirstObjectByType<Player>() : null;

        if (player != null)
        {
            player.ExecutePowerAttack(part, this);
        }
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
