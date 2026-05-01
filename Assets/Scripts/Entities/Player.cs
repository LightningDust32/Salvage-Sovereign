using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player : Entity
{
    private PlayerInput inputActions;

    private CharacterController controller;

    [Header("Movement")]
    [Range(1, 10)]
    [SerializeField] float moveSpeed = 5f;
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


    [Header("Gameplay")]
    [SerializeField] GameObject[] items;
    [SerializeField] private Weapon primaryWeapon;
    [SerializeField] private Weapon secondaryWeapon;

    private Weapon currentWeapon;

    private int currentItemIndex = -1;
    bool[] itemsUnlocked;

    private bool isMyTurn = false;
    private bool turnFinished = false;
    private bool inCombat = false;

    private int currentGold;

    protected override void Awake()
    {
        base.Awake();
        controller = GetComponent<CharacterController>();

        inputActions = new PlayerInput();

        itemsUnlocked = new bool[items.Length];

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
    }

    private void OnEnable()
    {
        inputActions.Enable();

        // Movement
        inputActions.Player.Move.performed += ctx => OnMove(ctx.ReadValue<Vector2>());
        inputActions.Player.Move.canceled += ctx => OnMove(Vector2.zero);

        // Items
        inputActions.Player.Attack.performed += context => TryUseItem();
        inputActions.Player.Interact.performed += context => TryInteract();
        inputActions.Player.Previous.performed += context => EquipPrevious();
        inputActions.Player.Next.performed += context => EquipNext();

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
            else
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

        // only allow combat actions here
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

    public void UnlockItem(int itemID)
    {
        if (itemID < 0 || itemID >= itemsUnlocked.Length)
            return;

        itemsUnlocked[itemID] = true;

        // If no item currently equipped, equip this one
        if (currentItemIndex == -1)
        {
            EquipItem(itemID);
        }
    }

    private void EquipItem(int index)
    {
        if (index < 0 || index >= items.Length)
            return;

        if (!itemsUnlocked[index])
            return;

        if (currentItemIndex >= 0 && currentItemIndex < items.Length)
        {
            items[currentItemIndex].SetActive(false);
        }

        currentItemIndex = index;
        items[currentItemIndex].SetActive(true);
    }

    private void TryInteract()
    {
        
    }

    private void CheckInteractPrompt()
    {
        
    }

    private void EquipNext()
    {
        if (currentItemIndex == -1)
            return;

        int startIndex = currentItemIndex;
        int newIndex = currentItemIndex;

        do
        {
            newIndex = (newIndex + 1) % items.Length;
        }
        while (!itemsUnlocked[newIndex] && newIndex != startIndex);

        if (newIndex != currentItemIndex)
        {
            EquipItem(newIndex); 
        }
    }

    private void EquipPrevious()
    {
        if (currentItemIndex == -1)
            return;

        int startIndex = currentItemIndex;
        int newIndex = currentItemIndex;

        do
        {
            newIndex--;
            if (newIndex < 0)
                newIndex = items.Length - 1;
        }
        while (!itemsUnlocked[newIndex] && newIndex != startIndex);

        if (newIndex != currentItemIndex)
            EquipItem(newIndex);
    }

    private void TryUseItem()
    {
        if (currentItemIndex < 0 || currentItemIndex >= items.Length)
            return;

        GameObject currentItem = items[currentItemIndex];

        if (currentItem == null)
            return;
    }

    public Transform GetCameraTransform()
    {
        return cameraTransform;
    }

    public override bool TakeTurn()
    {
        // if turn hasn't started initialize it
        if (!isMyTurn)
        {

        Debug.Log("Player Turn");

        isMyTurn = true;
        turnFinished = false;

        // Later: Activate the Battle UI

        }
        
        return turnFinished;
    }

    public void Attack(Entity target)
    {
        if (!isMyTurn) return;

        float damage = strength + currentWeapon.GetDamage();

        target.TakeDamage(damage);

        Debug.Log("Player attacked with:" + currentWeapon.name + " for: " + damage + " damage");

        EndTurn();
    }

    public void PowerAttack()
    {
        if (!isMyTurn) return;

        Debug.Log("Player performed power attack");

        EndTurn();
    }

    public void SwitchWeapon()
    {
        if (!isMyTurn) return;

        if(currentWeapon == primaryWeapon)
        {
            currentWeapon = secondaryWeapon;
        }
        else
        {
            currentWeapon = primaryWeapon;
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

    public override void TakeDamage(float amount)
    {
        base.TakeDamage(amount);
        UIManager.instance.SetHealthBar(GetHealthPercent());
    }

    public void ChangeGold(int amount)
    {
        currentGold += amount;
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


