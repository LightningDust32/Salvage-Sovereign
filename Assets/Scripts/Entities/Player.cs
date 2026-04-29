using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    private PlayerInput inputActions;

    private CharacterController controller;

    [Header("Movement")]
    [Range(1, 10)]
    [SerializeField] float moveSpeed = 5f;
    [Range(0.5f, 5)]
    [SerializeField] float maxStamina = 5.0f;


    private Vector2 moveInput;
    private Vector2 lookInput;
    private float verticalSpeed;
    private float cameraPitch;
    private float currentStamina;

    [SerializeField] Transform cameraTransform;
    [SerializeField] float cameraHeight = 1.0f;


    [Header("Gameplay")]
    [SerializeField] GameObject[] items; 
    [SerializeField] float maxHealth = 5.0f;

    private int currentItemIndex = -1;
    bool[] itemsUnlocked;
    bool[] progressUnlocked;

    private float currentHealth;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        inputActions = new PlayerInput();

        itemsUnlocked = new bool[items.Length];

        progressUnlocked = new bool[5];

        currentStamina = maxStamina;

        currentHealth = maxHealth;

        cameraTransform.localPosition = new Vector3(cameraTransform.localPosition.x, cameraHeight, cameraTransform.localPosition.z);
    }

    private void OnEnable()
    {
        inputActions.Enable();

        // Movement
        inputActions.Player.Move.performed += context => moveInput = context.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += context => moveInput = Vector2.zero;

        inputActions.Player.Look.performed += context => lookInput = context.ReadValue<Vector2>();
        inputActions.Player.Look.canceled += context => lookInput = Vector2.zero;

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
            HandleLook();
            CheckInteractPrompt();
        }
    }

    private void HandleMovement()
    {
        if(controller.enabled == false) return;
        if (controller.isGrounded && verticalSpeed < 0)
        { 
            verticalSpeed = -2f; 
        }

        float speed = moveSpeed;

        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;

        Vector3 velocity = move * speed;
        velocity.y = verticalSpeed;

        controller.Move(velocity * Time.deltaTime);
    }

    private void HandleLook()
    {
        
    }

    private void HandleStamina()
    {
        float percent = currentStamina / maxStamina;
        //UIManager.instance.SetStaminaBar(percent);
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

    public void UnlockProgress(int index)
    {
        progressUnlocked[index] = true;
    }

    public Transform GetCameraTransform()
    {
        return cameraTransform;
    }

    public void ChangeHealth(float health)
    {
        currentHealth += health;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        float percent = currentHealth / maxHealth;

        //UIManager.instance.SetHealthBar(percent);
    }
}
