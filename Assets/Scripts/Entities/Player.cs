using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]
public class Player : Entity
{
    private PlayerInput inputActions;

    private CharacterController controller;

    [Header("Movement")]
    [SerializeField] float rotationSpeed = 120f;
    [Range(0.5f, 5)]
    [SerializeField] float maxStamina = 5.0f;

    private Room currentRoom;
    [SerializeField] private float moveCooldown = 0.2f;
    private float moveTimer = 0f;


    private Vector2 moveInput;
    private float currentStamina;

    [SerializeField] private float moveDuration = 0.25f;
    [SerializeField] private AnimationCurve moveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    bool isMoving;

    [SerializeField] Transform cameraTransform;
    [SerializeField] float cameraHeight = 1.0f;

    [SerializeField] private Camera playerCamera;
    [SerializeField] private Camera minimapCamera;

    [Header("Gameplay")]
    [SerializeField] GameObject[] items;
    [SerializeField] private Weapon primaryWeapon;
    [SerializeField] private Weapon secondaryWeapon;

    private Weapon currentWeapon;
    private Enemy currentEnemy;


    private bool isMyTurn = false;
    private bool turnFinished = false;
    private bool inCombat = false;

    private int currentGold;

    [Header("Inventory")]
    [SerializeField, Range(5, 10)] private int maxInventorySize = 5;

    private List<HarvestItem> inventory = new List<HarvestItem>();

    [SerializeField] private Weapon[] weaponPool;

    private bool isInteracting = false;

    protected override void Awake()
    {
        base.Awake();
        controller = GetComponent<CharacterController>();

        inputActions = new PlayerInput();

        currentStamina = maxStamina;

        cameraTransform.localPosition = new Vector3(cameraTransform.localPosition.x, cameraHeight, cameraTransform.localPosition.z);  
    }

    private void Start()
    {
        if (RoomManager.Instance == null)
        {
            Debug.Log("RoomManager not found in scene");
            return;
        }

        currentRoom = RoomManager.Instance.GetStartingRoom();

        if (currentRoom != null)
        {
            transform.position = currentRoom.GetCentre();
        }
        else
        {
            Debug.Log("No starting room found");
        }

        ApplyPersistentData();

        if(currentWeapon != null)
        {
            currentWeapon.gameObject.SetActive(true);
        }
    }

    private void ApplyPersistentData()
    {
        // Apply permanent stat upgrades
        maxHealth += PersistentData.bonusHealth;
        strength += PersistentData.bonusStrength;
        defense += PersistentData.bonusDefense;

        // Equip weapons from weapon selection
        if (PersistentData.primaryWeaponIndex >= 0 && PersistentData.primaryWeaponIndex < weaponPool.Length)
        {
            primaryWeapon = weaponPool[PersistentData.primaryWeaponIndex];
        }

        if (PersistentData.secondaryWeaponIndex >= 0 && PersistentData.secondaryWeaponIndex < weaponPool.Length)
        {
            secondaryWeapon = weaponPool[PersistentData.secondaryWeaponIndex];
        }

        currentWeapon = primaryWeapon;
    }

    private void OnEnable()
    {
        inputActions.Enable();

        // Movement
        inputActions.Player.Move.performed += ctx => OnMove(ctx.ReadValue<Vector2>());
        inputActions.Player.Move.canceled += ctx => OnMove(Vector2.zero);

        // Items
        inputActions.Player.Interact.performed += context => TryInteract();

        // Map
        inputActions.Player.ToggleMiniMap.performed += ctx => ToggleMiniMap();

        // Pause
        inputActions.Player.Pause.performed += context => UIManager.instance.Pause();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void Update()
    {
        if(Time.timeScale > 0)
        {
            if (inCombat)
            {
                HandleCombat();
            }
            else if (!isInteracting)
            {
                HandleExploration();
            }
        }
    }

    void HandleExploration()
    {
        HandleStamina();
        HandleMovement();
        CheckInteractPrompt();
    }

    void HandleCombat()
    {
        if (!isMyTurn) return;

    }

    private void OnMove(Vector2 input)
    {
        moveInput = input;
    }

    private void HandleMovement()
    {
        if (!controller.enabled) return;

        if (isMoving) return;

        // Cooldown to prevent spam movement
        moveTimer -= Time.deltaTime;
        if (moveTimer > 0f) return;

        // Rotation (A/D)
        float rotation = moveInput.x * rotationSpeed * Time.deltaTime;
        transform.Rotate(Vector3.up * rotation);

        // Forward movement (W only)
        if (moveInput.y > 0.5f)
        {
            TryMoveForward();
        }
    }

    private void ToggleMiniMap() 
    {
        bool isActive = minimapCamera.enabled;

        minimapCamera.enabled = !isActive;
        playerCamera.enabled = isActive;
    }

    private void TryMoveForward()
    {
        if (currentRoom == null) return;

        Direction dir = GetFacingDirection();

        if (!currentRoom.HasConnection(dir))
        {
            Debug.Log("Blocked - no room in that direction");
            return;
        }

        Room nextRoom = currentRoom.GetConnectedRoom(dir);

        if (nextRoom == null)
        {
            Debug.Log("Connection exists but no room found");
            return;
        }

        MoveToRoom(nextRoom);
    }

    private void MoveToRoom(Room room)
    {
        if (isMoving) return;

        currentRoom = room;
        StartCoroutine(SmoothMove(room));
    }

    private Direction GetFacingDirection()
    {
        Vector3 forward = transform.forward;

        float dotForward = Vector3.Dot(forward, Vector3.forward);
        float dotRight = Vector3.Dot(forward, Vector3.right);

        if (Mathf.Abs(dotForward) > Mathf.Abs(dotRight))
        {
            return (dotForward > 0) ? Direction.North : Direction.South;
        }
        else
        {
            return (dotRight > 0) ? Direction.East : Direction.West;
        }
    }

    private void HandleStamina()
    {
        float percent = currentStamina / maxStamina;
        UIManager.instance.SetStaminaBar(percent);
    }

    private void TryInteract()
    {
        
    }

    private void CheckInteractPrompt()
    {
        
    }

    public Transform GetCameraTransform()
    {
        return cameraTransform;
    }

    public override bool TakeTurn()
    {
        if (!isMyTurn && !turnFinished)
        {
            Debug.Log("Player Turn Start");

            isMyTurn = true;
        }

        return turnFinished;
    }

    public void Attack()
    {
        if (!isMyTurn) return;

        if (currentWeapon == null)
        {
            Debug.Log("No weapon equipped");
            EndTurn();
            return;
        }

        currentEnemy = TurnManager.Instance.GetFirstAliveEnemy();

        float damage = strength + currentWeapon.GetDamage();

        currentEnemy.SetLastDamageType(currentWeapon.GetDamageType());
        currentEnemy.TakeDamage(damage);

        EndTurn();
    }

    public void PowerAttack()
    {
        if (!isMyTurn) return;

        currentEnemy = TurnManager.Instance.GetFirstAliveEnemy();

        if (currentEnemy == null)
        {
            Debug.Log("No enemy to target");
            return;
        }

        UIManager.instance.ShowDialogue("Select a body part to target", 10f);

        currentEnemy.SetTargetingActive(true);
    }

    public void ExecutePowerAttack(BodyPart part, Enemy target)
    {
        if (!isMyTurn) return;
        if (currentWeapon == null) return;

        if (currentStamina <= 0)
        {
            Debug.Log("No Stamina");
            return;
        }

        currentEnemy = TurnManager.Instance.GetFirstAliveEnemy();

        currentEnemy.SetLastDamageType(currentWeapon.GetDamageType());
        currentEnemy.SetTargetBodyPart(part);

        float multiplier = currentEnemy.GetMultiplier(part);
        float damage = (strength + currentWeapon.GetDamage()) * multiplier;

        currentEnemy.TakeDamage(damage);

        currentStamina -= currentStamina * 0.2f; // five power attacks limit for now

        UIManager.instance.ShowDialogue($"Power attack ({part}) for {damage}");
        currentEnemy.SetTargetingActive(false);

        EndTurn();
    }

    public void SwitchWeapon()
    {
        if (!isMyTurn) return;

        if(currentWeapon == primaryWeapon)
        {
            currentWeapon.gameObject.SetActive(false);
            currentWeapon = secondaryWeapon;
            currentWeapon.gameObject.SetActive(true);
        }
        else
        {
            currentWeapon.gameObject.SetActive(false);
            currentWeapon = primaryWeapon;
            currentWeapon.gameObject.SetActive(true);
        }

        Debug.Log("Switched weapon to: " + currentWeapon.name);

        EndTurn();
    }

    private void EndTurn()
    {
        isMyTurn = false;
        turnFinished = true;

        Debug.Log("Player Turn Ended");
    }

    public override void ResetTurn()
    {
        turnFinished = false;
        isMyTurn = false;
    }

    public override void TakeDamage(float amount)
    {
        base.TakeDamage(amount);
        UIManager.instance.SetHealthBar(GetHealthPercent());
    }

    public void ChangeGold(int amount)
    {
        currentGold += amount;
    }

    public bool AddHarvestItem(HarvestItem item)
    {
        if (item == null) return false;

        if (inventory.Count >= maxInventorySize)
        {
            Debug.Log("Inventory Full");
            return false;
        }

        inventory.Add(item);

        return true;
    }

    public List<HarvestItem> GetInventory()
    {
        return inventory;
    }

    public void RemoveHarvestItem(HarvestItem item)
    {
        if (inventory.Contains(item))
        {
            inventory.Remove(item);
        }
    }

    public bool IsInventoryFull()
    {
        return inventory.Count >= maxInventorySize;
    }


    public void EndRun()
    {
        int sellValue = 0;

        foreach (HarvestItem item in inventory)
        {
            sellValue += Mathf.RoundToInt(item.sellValue * 0.5f); // cheap sell
        }

        PersistentData.Gold += currentGold + sellValue;

        Debug.Log($"Run ended. Gold gained: {currentGold} + {sellValue}");
        PersistentData.ResetRunSelections();

        PersistentData.Save();

        // Reset run data
        inventory.Clear();
        currentGold = 0;

        SceneManager.LoadScene(1); // Lobby
    }

    public void EnterCombat()
    {
        inCombat = true;

        Debug.Log("Player entered combat");

        moveInput = Vector2.zero;
    }

    public void ExitCombat()
    {
        inCombat = false;

        Debug.Log("Player exited combat");

        isMyTurn = false;
        turnFinished = false;
    }

    public void FaceTarget(Transform target)
    {
        Vector3 direction = (target.position - transform.position);
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.01f) return;

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = lookRotation;
    }

    public void SellItem(int index)
    {
        if (index < 0 || index >= inventory.Count)
            return;

        HarvestItem item = inventory[index];

        if (item == null)
            return;

        int value = item.sellValue;

        inventory.RemoveAt(index);
        ChangeGold(value);

        Debug.Log($"Sold {item.itemName} for {value} gold");

        // Refresh merchant UI
        UpdateMerchantUI();
    }

    public void UpdateMerchantUI()
    {
        string[] itemNames = new string[inventory.Count];

        for (int i = 0; i < inventory.Count; i++)
        {
            itemNames[i] = inventory[i].itemName;
        }

        UIManager.instance.SetMerchantText(itemNames);
    }

    // Helper function to lock movement in states other than combat
    public void SetInteractionState(bool state)
    {
        isInteracting = state;
    }

    private IEnumerator SmoothMove(Room targetRoom)
    {
        isMoving = true;

        Vector3 startPos = transform.position;
        Vector3 endPos = targetRoom.GetCentre();

        float time = 0f;

        // Disable controller to prevent interference
        controller.enabled = false;

        while (time < moveDuration)
        {
            time += Time.deltaTime;

            float t = time / moveDuration;
            float curvedT = moveCurve.Evaluate(t);

            transform.position = Vector3.Lerp(startPos, endPos, curvedT);

            yield return null;
        }

        transform.position = endPos;

        controller.enabled = true;

        isMoving = false;

        // Trigger encounter AFTER movement completes
        targetRoom.OnPlayerEntered(this);
    }
}


