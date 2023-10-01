using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;

public class Enemy : MonoBehaviour
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] float navRate;
    [SerializeField] float navDistance;
    [SerializeField] ParticleSystem blood;

    public bool isDead = false;

    float navTimer;

    void Update()
    {
        if(Time.time > navTimer)
        {
            navTimer = Time.time + navRate;
            agent.SetDestination(new Vector3(transform.position.x + Random.insideUnitCircle.x * navDistance, transform.position.y, transform.position.z + Random.insideUnitCircle.y * navDistance));
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
            Player.killCount++;
            blood.Play();

            GetComponentInChildren<SpriteAnimation>().runAnimation(1);
        }
    }
}
