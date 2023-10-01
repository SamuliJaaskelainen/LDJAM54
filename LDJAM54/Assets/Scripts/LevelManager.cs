using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [SerializeField] List<GameObject> enemyPrefabs = new List<GameObject>();
    [SerializeField] List<GameObject> roomPrefabs = new List<GameObject>();

    Room currentRoom = null;
    Room nextRoom = null;

    void Awake()
    {
        Instance = this;

        currentRoom = SpawnRoom(Vector3.zero, Vector3.zero);
        currentRoom.startDoorway.SetActive(true);
        nextRoom = SpawnRoom(currentRoom.endDoorway.transform.position, RoomDoorRotation(currentRoom));
    }

    Vector3 RoomDoorRotation(Room room)
    {
        return new Vector3(room.endDoorway.transform.eulerAngles.x + 90.0f, room.endDoorway.transform.eulerAngles.y, room.endDoorway.transform.eulerAngles.z);
    }

    public void SpawnNextRoom()
    {
        if (currentRoom)
        { 
            for(int i = 0; i < currentRoom.enemies.Count; i++)
            {
                if (currentRoom.enemies[i])
                {
                    Destroy(currentRoom.enemies[i].gameObject);
                }
            }

            Debug.Log("Destroy previous room.");
            Destroy(currentRoom.gameObject);
        }

        if(nextRoom)
        {
            Debug.Log("Set new current room.");
            currentRoom = nextRoom;
            currentRoom.startDoorway.SetActive(true);
        }

        Debug.Log("Spawn next room.");
        nextRoom = SpawnRoom(currentRoom.endDoorway.transform.position, RoomDoorRotation(currentRoom));
    }

    Room SpawnRoom(Vector3 position, Vector3 euler)
    {
        Room room = Instantiate(roomPrefabs[Random.Range(0, roomPrefabs.Count)], position, Quaternion.Euler(euler)).GetComponent<Room>();
        room.Init();

        for (int i = 0; i < room.spawns.Count; i++)
        {
            GameObject newEnemy = Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Count)], room.spawns[i].position, Quaternion.identity);
            room.enemies.Add(newEnemy.GetComponent<Enemy>());
        }

        return room;
    }

    public void Lose()
    {
        Debug.Log("LOSE");
        SceneManager.LoadScene(0);
    }
}
