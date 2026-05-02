using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance;

    [Header("Settings")]
    [SerializeField] private float connectionThreshold = 0.5f;

    private List<Room> rooms = new List<Room>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        Initialize();
    }

    private void Initialize()
    {
        FindAllRooms();
        LinkAllRooms();
    }

    private void FindAllRooms()
    {
        Room[] foundRooms = FindObjectsByType<Room>(FindObjectsSortMode.None);

        rooms = new List<Room>(foundRooms);
    }

    private void LinkAllRooms()
    {
        List<Transform> allDoors = new List<Transform>();
        HashSet<Transform> usedDoors = new HashSet<Transform>();

        // Collect all doors
        foreach (Room room in rooms)
        {
            foreach (GameObject door in room.GetEntrances())
            {
                if (door != null && door.activeSelf)
                    allDoors.Add(door.transform);
            }
        }

        // Pair doors using best match
        foreach (Transform doorA in allDoors)
        {
            if (usedDoors.Contains(doorA))
                continue;

            Transform bestMatch = null;
            float bestDistance = float.MaxValue;

            foreach (Transform doorB in allDoors)
            {
                if (doorA == doorB || usedDoors.Contains(doorB))
                    continue;

                float distance = Vector3.Distance(doorA.position, doorB.position);

                if (distance > connectionThreshold)
                    continue;

                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    bestMatch = doorB;
                }
            }

            if (bestMatch != null)
            {
                Room roomA = doorA.GetComponentInParent<Room>();
                Room roomB = bestMatch.GetComponentInParent<Room>();

                Direction dirA = GetDirectionFromTransform(doorA);
                Direction dirB = GetDirectionFromTransform(bestMatch);

                roomA.SetConnection(dirA, roomB);
                roomB.SetConnection(dirB, roomA);

                usedDoors.Add(doorA);
                usedDoors.Add(bestMatch);
            }
            else
            {
                Debug.LogWarning($"No match found for door: {doorA.name}");
            }
        }
    }

    private Direction GetDirectionFromTransform(Transform t)
    {
        Vector3 forward = t.forward;

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

    public Room GetStartingRoom()
    {
        // Simple version: return first room
        return rooms.Count > 0 ? rooms[0] : null;
    }

    public List<Room> GetAllRooms()
    {
        return rooms;
    }
}