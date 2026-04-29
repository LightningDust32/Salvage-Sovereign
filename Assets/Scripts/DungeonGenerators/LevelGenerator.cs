using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [Header("Level Container")]
    [SerializeField] private Transform levelContainer;

    [Header("Room Types")]
    [SerializeField] private GameObject[] corridors;
    [SerializeField] private GameObject[] corners;
    [SerializeField] private GameObject[] crossroads;
    [SerializeField] private GameObject[] deadEnds;
    [SerializeField] private GameObject[] sections;

    [Header("Room Type Counts")]
    [SerializeField] private int corridorCount = 5;
    [SerializeField] private int cornerCount = 3;
    [SerializeField] private int crossroadsCount = 2;

    [Header("Objective")]
    [SerializeField] GameObject endObject;

    private int corridorsLeft;
    private int cornersLeft;
    private int crossroadsLeft;

    private int totalRooms;

    private List<GameObject> activeExits = new List<GameObject>();
    private List<Room> spawnedRooms = new List<Room>();

    private void Awake()
    {
        // Initialises how may of each type should spawn between each section.
        if (corridors == null || corridors.Length == 0) corridorCount = 0;
        if (corners == null || corners.Length == 0) cornerCount = 0;
        if (crossroads == null || crossroads.Length == 0) crossroadsCount = 0;

        corridorCount = Mathf.Max(0, corridorCount);
        cornerCount = Mathf.Max(0, cornerCount);
        crossroadsCount = Mathf.Max(0, crossroadsCount);
    }

    private void Start()
    {
        GenerateAllSections();
    }

    // Spawns a section, then a series of corridors, then the next section, then more corridors
    private void GenerateAllSections()
    {
        if (sections == null || sections.Length == 0)
        {
            Debug.Log("No Section Prefabs");
            return;
        }

        GameObject firstRoom = Instantiate(sections[0], Vector3.zero, Quaternion.identity, levelContainer);
        Room firstRoomClass = firstRoom.GetComponent<Room>();
        spawnedRooms.Add(firstRoomClass);
        RegisterExits(firstRoomClass);

        for (int section = 0; section < sections.Length; section++)
        {
            GenerateSectionRooms();

            bool isLastSection = (section == sections.Length - 1);

            if (!isLastSection)
            {
                if (activeExits.Count == 0)
                    return;

                int nextIndex = Random.Range(0, activeExits.Count);
                GameObject nextExit = activeExits[nextIndex];

                // Cap all other exits with rooms
                for (int i = activeExits.Count - 1; i >= 0; i--)
                {
                    if (i == nextIndex)
                        continue;

                    PlaceRoomAtExit(deadEnds, activeExits[i]);
                }

                // Place next section on remaining exit
                PlaceRoomAtExit(new GameObject[] { sections[section + 1] }, nextExit);
            }
            else
            {
                // Final section
                for (int i = activeExits.Count - 1; i >= 0; i--)
                {
                    PlaceRoomAtExit(deadEnds, activeExits[i]);
                }

                activeExits.Clear();
            }
        }
    }

    // Spawns rooms equal to the maximum amount set at the beginning
    private void GenerateSectionRooms()
    {
        corridorsLeft = corridorCount;
        cornersLeft = cornerCount;
        crossroadsLeft = crossroadsCount;

        totalRooms = corridorCount + cornerCount + crossroadsCount;

        for (int i = 0; i < totalRooms; i++)
        {
            SpawnRoomAtRandom();
        }
    }
    

    private void SpawnRoomAtRandom()
    {
        if (activeExits.Count == 0)
        {
            Debug.Log("No active exits left.");
            return;
        }

        GameObject roomPrefab = GetRandomRoomPrefab();
        if (roomPrefab == null) 
        {
            Debug.Log("No Room Prefabs");
            return; 
        }

        GameObject chosenExit = null;
        Room.RoomType newRoomType = roomPrefab.GetComponent<Room>().GetRoomType();

        // If placing a turn, avoid placing at another turn
        if (newRoomType == Room.RoomType.Corner || newRoomType == Room.RoomType.Crossroads)
        {
            int attempts = 5;

            while (attempts > 0)
            {
                GameObject potentialExit = activeExits[Random.Range(0, activeExits.Count)];

                Room parentRoom = potentialExit.transform.parent.GetComponent<Room>();

                if (parentRoom != null)
                {
                    if (parentRoom.GetRoomType() != Room.RoomType.Corner && parentRoom.GetRoomType() != Room.RoomType.Crossroads)
                    {
                        chosenExit = potentialExit;
                        break;
                    }
                }

                attempts--;
            }

        }

        // If no valid exit found, just pick any exit
        if (chosenExit == null)
        {
            chosenExit = activeExits[Random.Range(0, activeExits.Count)];
        }

        PlaceRoomAtExit(new GameObject[] { roomPrefab }, chosenExit);
    }

    private void RegisterExits(Room room)
    {
        foreach (GameObject exit in room.GetExits())
        {
            if (exit.activeSelf)
            {
                activeExits.Add(exit);
            }
        }
    }

    // Picks a prefab at random from the still available prefabs
    private GameObject GetRandomRoomPrefab()
    {
        List<GameObject[]> roomChoices = new List<GameObject[]>();

        if (corridorsLeft > 0 && corridors.Length > 0)
        { 
            roomChoices.Add((corridors)); 
        }

        if (cornersLeft > 0 && corners.Length > 0)
        { 
            roomChoices.Add((corners)); 
        }

        if (crossroadsLeft > 0 && crossroads.Length > 0)
        { 
            roomChoices.Add((crossroads)); 
        }

        if (roomChoices.Count == 0) return null;

        GameObject[] choice = roomChoices[Random.Range(0, roomChoices.Count)];
        GameObject roomPrefab = choice[Random.Range(0, choice.Length)];

        Room.RoomType type = roomPrefab.GetComponent<Room>().GetRoomType();

        switch (type)
        {
            case Room.RoomType.Corridor: corridorsLeft--; break;
            case Room.RoomType.Corner: cornersLeft--; break;
            case Room.RoomType.Crossroads: crossroadsLeft--; break;
        }

        return roomPrefab;
    }

    private void PlaceRoomAtExit(GameObject[] objectPool, GameObject exit)
    {
        if (objectPool == null || objectPool.Length == 0)
            return;

        GameObject prefab = objectPool[Random.Range(0, objectPool.Length)];
        GameObject roomInstance = Instantiate(prefab, Vector3.zero, Quaternion.identity, levelContainer);

        Room roomClass = roomInstance.GetComponent<Room>();
        GameObject roomEntrance = roomClass.GetEntrance();

        roomInstance.transform.rotation = exit.transform.rotation;

        Vector3 entranceWorldPos = roomEntrance.transform.position;
        Vector3 exitWorldPos = exit.transform.position;
        roomInstance.transform.position += exitWorldPos - entranceWorldPos;

        roomEntrance.SetActive(false);
        exit.SetActive(false);
        activeExits.Remove(exit);

        spawnedRooms.Add(roomClass);
        RegisterExits(roomClass);
    }
}