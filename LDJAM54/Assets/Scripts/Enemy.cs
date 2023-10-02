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
    [SerializeField] float attackRange;
    [SerializeField] private float enemyType;

    public bool isDead = false;

    float navTimer;
    float fireTimer;
    RaycastHit hit;
    int attacks;
    float attackTimer;
    bool justAttacked = false;

    void Update()
    {
        if (Time.time > navTimer)
        {
            navTimer = Time.time + navRate;
            if (agent.isOnNavMesh)
            {
                agent.SetDestination(new Vector3(transform.position.x + Random.insideUnitCircle.x * navDistance, transform.position.y, transform.position.z + Random.insideUnitCircle.y * navDistance));
            }
        }

        
        if (Time.time > fireTimer)
        {
            justAttacked = false;
            fireTimer = Time.time + fireRate;

            if (Time.time > attackTimer)
            {
                Vector3 bulletSpawnPoint = transform.position + Vector3.up * projectileYOffset;
                if (Vector3.Distance(bulletSpawnPoint, Player.Instance.transform.position) < attackRange)
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
                            PlayShootSound();
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

        if (agent.isOnNavMesh)
        {
            if (justAttacked)
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
    }

    public void Die()
    {
        Debug.Log("Kill enemy!");

        if (isDead == false)
        {
            isDead = true;
            enabled = false;
            Destroy(GetComponentInChildren<Collider>());
            Destroy(agent);
            Player.killCount++;
            blood.Play();
            PlayDeathSound();

            GetComponentInChildren<SpriteAnimation>().runAnimation(1);
        }
    }
    
    private void PlayDeathSound()
    {
        switch (enemyType) {
            case 0:
                AudioManager.Instance.PlaySound(Random.Range(37, 44), transform.position, 1f, 1f, false);
                break;
            case 1:
                AudioManager.Instance.PlaySound(Random.Range(44, 51), transform.position, 1f, 1f, false);
                break;
            case 2:
                AudioManager.Instance.PlaySound(Random.Range(51, 59), transform.position, 1f, 1f, false);
                break;
                
        }
    }
    
    private void PlayShootSound()
    {
        switch (enemyType) {
            case 0:
                AudioManager.Instance.PlaySound(Random.Range(33, 37), transform.position, 0.35f, 1f, false);
                break;
            case 1:
                AudioManager.Instance.PlaySound(Random.Range(29, 33), transform.position, 0.35f, 1f, false);
                break;
            case 2:
                break;
                
        }
    }
}
