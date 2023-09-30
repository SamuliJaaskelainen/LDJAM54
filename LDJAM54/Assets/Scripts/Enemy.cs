using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] float navRate;
    [SerializeField] float navDistance;
    float navTimer;

    void Update()
    {
        if(Time.time > navTimer)
        {
            navTimer = Time.time + navRate;
            agent.SetDestination(new Vector3(Random.insideUnitCircle.x * navDistance, transform.position.y, Random.insideUnitCircle.y * navDistance));
        }
    }

    public void Die()
    {
        enabled = false;
        GetComponent<BoxCollider>().enabled = false;

        // TODO: Add death animation and leave body on the ground instead of destroying the object
        Destroy(gameObject);
    }
}
