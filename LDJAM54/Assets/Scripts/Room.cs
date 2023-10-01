using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Room : MonoBehaviour
{
    [HideInInspector] public List<Transform> spawns = new List<Transform>();
    [HideInInspector] public GameObject startDoorway;
    [HideInInspector] public GameObject endDoorway;
    [HideInInspector] public List<Enemy> enemies = new List<Enemy>();

    public void Init()
    {
        GameObject spawnsParent = transform.Find("Spawns").gameObject;
        for (int i = 0; i < spawnsParent.transform.childCount; ++i)
        {
            spawns.Add(spawnsParent.transform.GetChild(i).transform);
        }

        startDoorway = transform.Find("DoorwayStart").gameObject;
        endDoorway = transform.Find("DoorwayEnd").gameObject;

        startDoorway.SetActive(false);
        endDoorway.transform.Find("Door").gameObject.SetActive(false);
    }
}
