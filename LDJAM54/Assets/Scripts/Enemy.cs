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
    [SerializeField] LayerMask fireLayers;
    [SerializeField] GameObject projectile;
    [SerializeField] float projectileYOffset;
    [SerializeField] float fireRate;
    [SerializeField] int attackAmountBeforeCooldown;
    [SerializeField] float attackCooldown;

    public bool isDead = false;

    float navTimer;
    float fireTimer;
    RaycastHit hit;
    int attacks;
    float attackTimer;
    bool justAttacked = false;

    void Update()
    {
        if(Time.time > navTimer)
        {
            navTimer = Time.time + navRate;
            agent.SetDestination(new Vector3(transform.position.x + Random.insideUnitCircle.x * navDistance, transform.position.y, transform.position.z + Random.insideUnitCircle.y * navDistance));
        }

        
        if (Time.time > fireTimer)
        {
            justAttacked = false;
            fireTimer = Time.time + fireRate;

            if (Time.time > attackTimer)
            {
                Vector3 bulletSpawnPoint = transform.position + Vector3.up * projectileYOffset;
                if (Vector3.Distance(bulletSpawnPoint, Player.Instance.transform.position) < 20.0f)
                {
                    if (Physics.Linecast(bulletSpawnPoint, Player.Instance.transform.position, out hit, fireLayers))
                    {
                        Debug.Log("Enemy sees: " + hit.transform.name);
                        if (hit.transform.tag == "Player")
                        {
                            justAttacked = true;
                            Projectile newProjectile = Instantiate(projectile, bulletSpawnPoint, Quaternion.identity).GetComponent<Projectile>();
                            newProjectile.direction = (Player.Instance.transform.position - bulletSpawnPoint).normalized;
                            attacks++;

                            if(attacks >= attackAmountBeforeCooldown)
                            {
                                attacks = 0;
                                attackTimer = Time.time + attackCooldown;
                            }
                        }
                    }
                }
            }
        }

        if(justAttacked)
        {
            if (!agent.isStopped)
            {
                agent.isStopped = true;
                GetComponentInChildren<SpriteAnimation>().runAnimation(2);
            }
        }
        else
        {
            if (agent.isStopped)
            {
                agent.isStopped = false;
                GetComponentInChildren<SpriteAnimation>().runAnimation(0);
            }
        }
    }

    public void Die()
    {
        Debug.Log("Kill enemy!");

        if (isDead == false)
        {
            isDead = true;
            enabled = false;
            Destroy(GetComponentInChildren<Collider>());
            agent.isStopped = true;
            Player.killCount++;
            blood.Play();

            GetComponentInChildren<SpriteAnimation>().runAnimation(1);
        }
    }
}
