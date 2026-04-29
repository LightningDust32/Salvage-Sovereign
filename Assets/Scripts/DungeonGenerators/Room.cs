using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public enum RoomType
    {
        Corridor,
        Corner,
        Crossroads,
        DeadEnd,
        Section
    }

    [SerializeField] private RoomType roomType;

    [SerializeField] GameObject entrancePoint;
    [SerializeField] GameObject[] exits;

    [SerializeField] GameObject[] furnitureSlots;
    [SerializeField] GameObject[] wallSlots;
    [SerializeField] GameObject[] furniture;
    [SerializeField] GameObject[] wallFurniture;
    [SerializeField] GameObject[] encounters;

    [SerializeField] GameObject roomCentre;

    List<GameObject> usedFurniture = new List<GameObject>();


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
                if (Random.value < 0.2f) // 20% chance per room to include an encounter (if applicable)
                {
                    prefabToSpawn = encounters[Random.Range(0, encounters.Length)];
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

    public GameObject GetEntrance()
    {
        return entrancePoint;
    }

    public GameObject[] GetExits()
    {
        return exits;
    }

    public RoomType GetRoomType()
    {
        return roomType;
    }

    public GameObject GetRoomCentre()
    {
        return roomCentre;
    }
}
