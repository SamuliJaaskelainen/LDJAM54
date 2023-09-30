using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] float navRate;
    [SerializeField] float navDistance;

    public bool isDead = false;

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
        Debug.Log("Kill enemy!");

        if (isDead == false)
        {
            isDead = true;
            enabled = false;
            GetComponent<BoxCollider>().enabled = false;
            agent.isStopped = true;
            LevelManager.Instance.CheckForWin();

            GetComponentInChildren<SpriteAnimation>().runAnimation(1);
        }
    }
}
