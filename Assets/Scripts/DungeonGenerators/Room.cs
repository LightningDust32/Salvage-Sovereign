using System.Collections.Generic;
using UnityEngine;


public enum Direction
{
    North,
    East,
    South,
    West
}

public class Room : MonoBehaviour
{
    public enum RoomType
    {
        Start,
        Normal,
        Combat,
        Treasure,
        DeadEnd,
        Section,
        Exit
    }

    [SerializeField] private RoomType roomType;

    [SerializeField] GameObject[] entrances;
    [SerializeField] GameObject[] exits;

    [SerializeField] GameObject[] furnitureSlots;
    [SerializeField] GameObject[] wallSlots;
    [SerializeField] GameObject[] furniture;
    [SerializeField] GameObject[] wallFurniture;
    [SerializeField] GameObject[] encounters;

    [SerializeField] GameObject roomCentre;

    List<GameObject> usedFurniture = new List<GameObject>();

    private Dictionary<Direction, Room> connectedRooms = new Dictionary<Direction, Room>();

    [SerializeField] private Encounter currentEncounter;


    private void Start()
    {
        FurnishRoom();
    }


    private void FurnishRoom()
    {
        // Convert available slots to lists, so they can be shuffled
        List<GameObject> freeFurnitureSlots = new List<GameObject>(furnitureSlots);
        List<GameObject> freeWallSlots = new List<GameObject>(wallSlots);

        ShuffleList(freeFurnitureSlots);
        ShuffleList(freeWallSlots);

        bool encounterPlaced = false;

        foreach (GameObject slot in freeFurnitureSlots)
        {
            if (slot == null) continue;

            GameObject prefabToSpawn = null;

            // Small chance to place a puzzle if one hasn’t been placed yet
            if (!encounterPlaced && encounters != null && encounters.Length > 0)
            {
                if (Random.value < 0.5f) // 50% chance per room to include an encounter
                {
                    GameObject encounterPrefab = encounters[Random.Range(0, encounters.Length)];

                    GameObject encounterObj = Instantiate(encounterPrefab, slot.transform.position, Quaternion.identity, transform);

                    currentEncounter = encounterObj.GetComponent<Encounter>();

                    encounterPlaced = true;
                }
            }

            // Otherwise spawn furniture
            if (prefabToSpawn == null && furniture != null && furniture.Length > 0)
            {
                prefabToSpawn = furniture[Random.Range(0, furniture.Length)];
            }


            // Check in place to prevent multiple of the same object placed in the room, break out and place anyway if duplication is guaranteed
            int attempts = 0;

            while (usedFurniture.Contains(prefabToSpawn) && attempts < 10)
            {
                prefabToSpawn = furniture[Random.Range(0, furniture.Length)];
                attempts++;
            }

            if (prefabToSpawn != null)
            {
                Instantiate(prefabToSpawn, slot.transform.position, slot.transform.rotation, transform);
                usedFurniture.Add(prefabToSpawn);
            }
        }

        foreach (GameObject slot in freeWallSlots)
        {
            if (slot == null) continue;

            if (wallFurniture == null || wallFurniture.Length == 0)
                continue;

            GameObject prefab = wallFurniture[Random.Range(0, wallFurniture.Length)];

            Instantiate(prefab, slot.transform.position, slot.transform.rotation, transform);
        }
    }

    private void ShuffleList(List<GameObject> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);
            GameObject temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    public GameObject[] GetEntrances()
    {
        return entrances;
    }

    public GameObject[] GetExits()
    {
        return exits;
    }

    public RoomType GetRoomType()
    {
        return roomType;
    }

    public Vector3 GetCentre()
    {
        if (roomCentre != null)
            return roomCentre.transform.position;

        return transform.position;
    }

    public void SetConnection(Direction dir, Room room)
    {
        if (connectedRooms.ContainsKey(dir))
        {
            connectedRooms[dir] = room;
        }
        else
        {
            connectedRooms.Add(dir, room);
        }
    }

    public bool HasConnection(Direction dir)
    {
        return connectedRooms.ContainsKey(dir);
    }

    public Room GetConnectedRoom(Direction dir)
    {
        if (connectedRooms.TryGetValue(dir, out Room room))
        {
            return room;
        }

        return null;
    }

    public void OnPlayerEntered(Player player)
    {
        if (currentEncounter != null)
        {
            currentEncounter.OnRoomEntered(player);
        }
    }
}
