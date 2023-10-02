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

    float modifiedAttackCooldown;
    float modifiedAttackAmountBeforeCooldown;

    void Update()
    {
        if (LevelManager.Instance.gameTime > 500.0f)
        {
            modifiedAttackCooldown = attackCooldown * 0.2f;
            modifiedAttackAmountBeforeCooldown = Mathf.CeilToInt((float)attackAmountBeforeCooldown * 1.8f);
        }
        else if (LevelManager.Instance.gameTime > 450.0f)
        {
            modifiedAttackCooldown = attackCooldown * 0.3f;
            modifiedAttackAmountBeforeCooldown = Mathf.CeilToInt((float)attackAmountBeforeCooldown * 1.7f);
        }
        else if (LevelManager.Instance.gameTime > 400.0f)
        {
            modifiedAttackCooldown = attackCooldown * 0.4f;
            modifiedAttackAmountBeforeCooldown = Mathf.CeilToInt((float)attackAmountBeforeCooldown * 1.6f);
        }
        else if (LevelManager.Instance.gameTime > 350.0f)
        {
            modifiedAttackCooldown = attackCooldown * 0.5f;
            modifiedAttackAmountBeforeCooldown = Mathf.CeilToInt((float)attackAmountBeforeCooldown * 1.5f);
        }
        else if (LevelManager.Instance.gameTime > 300.0f)
        {
            modifiedAttackCooldown = attackCooldown * 0.6f;
            modifiedAttackAmountBeforeCooldown = Mathf.CeilToInt((float)attackAmountBeforeCooldown * 1.4f);
        }
        else if (LevelManager.Instance.gameTime > 250.0f)
        {
            modifiedAttackCooldown = attackCooldown * 0.7f;
            modifiedAttackAmountBeforeCooldown = Mathf.CeilToInt((float)attackAmountBeforeCooldown * 1.3f);
        }
        else if (LevelManager.Instance.gameTime > 200.0f)
        {
            modifiedAttackCooldown = attackCooldown * 0.8f;
            modifiedAttackAmountBeforeCooldown = Mathf.CeilToInt((float)attackAmountBeforeCooldown * 1.2f);
        }
        else if (LevelManager.Instance.gameTime > 150.0f)
        {
            modifiedAttackCooldown = attackCooldown * 0.9f;
            modifiedAttackAmountBeforeCooldown = Mathf.CeilToInt((float)attackAmountBeforeCooldown * 1.1f);
        }
        else
        {
            modifiedAttackCooldown = attackCooldown;
            modifiedAttackAmountBeforeCooldown = attackAmountBeforeCooldown;
        }

        if (Time.time > navTimer)
        {
            navTimer = Time.time + navRate;
            if (agent.isOnNavMesh)
            {
                if (enemyType == 2)
                {
                    Vector3 eyePositon = transform.position + Vector3.up * projectileYOffset;
                    if (Vector3.Distance(eyePositon, Player.Instance.transform.position) < attackRange)
                    {
                        // TODO: Add screaming enemy audio
                        Vector3 directionToPlayer = (transform.position - Player.Instance.transform.position).normalized;
                        Vector3 newDestination = new Vector3(transform.position.x + Random.insideUnitCircle.x * navDistance, transform.position.y, transform.position.z + Random.insideUnitCircle.y * navDistance);
                        newDestination += directionToPlayer * navDistance;
                        agent.SetDestination(newDestination);
                    }
                }
                else
                {
                    agent.SetDestination(new Vector3(transform.position.x + Random.insideUnitCircle.x * navDistance, transform.position.y, transform.position.z + Random.insideUnitCircle.y * navDistance));
                }
            }
        }
        
        if (Time.time > fireTimer && enemyType != 2)
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
                            if(attacks >= modifiedAttackAmountBeforeCooldown)
                            {
                                attacks = 0;
                                attackTimer = Time.time + modifiedAttackCooldown;
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
