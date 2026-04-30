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

    [SerializeField] Transform cameraTransform;
    [SerializeField] float cameraHeight = 1.0f;


    [Header("Gameplay")]
    [SerializeField] GameObject[] items;

    private int currentItemIndex = -1;
    bool[] itemsUnlocked;


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
            transform.position = currentRoom.GetCenter();
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
            HandleStamina();
            HandleMovement();
            CheckInteractPrompt();
        }
    }

    private void OnMove(Vector2 input)
    {
        moveInput = input;
    }

    private void HandleMovement()
    {
        if (!controller.enabled) return;

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
        currentRoom = room;

        // Disable controller briefly to avoid collision issues
        controller.enabled = false;
        transform.position = room.GetCenter();
        controller.enabled = true;

        moveTimer = moveCooldown;
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

    public override void TakeTurn()
    {
        Debug.Log("Player Turn");
        // Later: Activate the Battle UI
    }

    public override void TakeDamage(float amount)
    {
        base.TakeDamage(amount);
        UIManager.instance.SetHealthBar(GetHealthPercent());
    }
}
