using UnityEngine;

public abstract class Encounter : MonoBehaviour
{
    [Header("Encounter Settings")]
    [SerializeField] protected bool triggerOnEnter = true;
    [SerializeField] protected bool disableWhenCleared = true;
    [SerializeField] string encounterPrompt;

    protected bool isCleared = false;

    protected Room room;

    protected virtual void Awake()
    {
        room = GetComponentInParent<Room>();
    }

    // Called when player enters the room
    public void OnRoomEntered(Player player)
    {
        if (isCleared && disableWhenCleared)
            return;

        if (triggerOnEnter)
        {
            TriggerEncounter(player);
        }
    }

    // Core behaviour
    protected abstract void TriggerEncounter(Player player);

    // Call this to disable the encounter
    protected virtual void CompleteEncounter()
    {
        isCleared = true;

        if (disableWhenCleared)
        {
            gameObject.SetActive(false);
        }
    }

    public bool IsCleared()
    {
        return isCleared;
    }

    public string GetPrompt()
    {
        return encounterPrompt;
    }
}