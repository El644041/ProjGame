using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class AIMovement : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public Transform attackPoint;
    public LayerMask playerLayers;
    public GameObject coins;
    public EnemyHealthBar healthBar;
    
    public AudioSource playerAudioSource;

    public AudioClip skeletonDeadAudio;
    
    public float lookRadius;
    public float wanderRadius;

    public float attackRange = 1f;
    public int attackDamage = 20;
    
    public int maxHealth = 100;
    public int currentHealth;
    
    private Animator animator;
    
    private float wanderSpeed = 1.25f;
    private float runSpeed = 2.25f;
    
    private float attackRate = 2f;
    private float nextAttack;
    private bool isDead;
    public MazeConstructor mz;
    public tutorial tutorialScript;

    void Start()
    {
         animator = GetComponentInChildren<Animator>();
         currentHealth = maxHealth;
         healthBar.SetMaxHealth(maxHealth);
         healthBar.SetHealth(maxHealth);
    }

    void Update()
    {
        // Distance to the target
        float distance = Vector3.Distance(player.position, transform.position);

        // If not inside the lookRadius
        if (distance >= lookRadius)
        {
            Wander();
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance
                                   && !agent.hasPath || agent.velocity.sqrMagnitude == 0f)
            {
                agent.ResetPath();
                NavMeshPath path = new NavMeshPath();
                Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
                agent.CalculatePath(newPos, path);
                if (agent.pathStatus != NavMeshPathStatus.PathPartial)
                {
                    agent.SetDestination(newPos);
                }
            }
        }

        if (distance < lookRadius)
        {
            FaceTarget();
            tutorialScript.GetComponent<tutorial>().combat();
            // If within attacking distance
            if (distance < agent.stoppingDistance)
            {
                Idle();
                if (Time.time > nextAttack)
                {
                    nextAttack = Time.time + attackRate;
                    StartCoroutine(Slash());
                }
            }
            else
            {
                // Move towards the target
                agent.SetDestination(player.position);
                Run();
            }
        }
    }

    public int SubtractEnemyHealth(int damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);
        return currentHealth;
    }
    
    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask) 
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;
        NavMeshHit navHit;
        NavMesh.SamplePosition (randDirection, out navHit, dist, layermask);
     
        return navHit.position;
    }
    
    private void Idle()
    {
        animator.SetFloat("Speed", 0f, 0.2f, Time.deltaTime);
    }

    private void Wander()
    {
        agent.speed = wanderSpeed;
        animator.SetFloat("Speed", 0.5f, 0.2f, Time.deltaTime);
    }
    
    private void Run()
    {
        agent.speed = runSpeed;
        animator.SetFloat("Speed", 1f, 0.2f, Time.deltaTime);
    }

    private IEnumerator Slash()
    {
        if (!isDead)
        {
            agent.isStopped = true;
            int health = currentHealth;
            animator.speed = 1.5f;
            animator.SetTrigger("Swing");
            yield return new WaitForSeconds(0.6f);
            if (health == currentHealth)
            {
                Collider[] hitPlayers = Physics.OverlapSphere(attackPoint.position, attackRange, playerLayers);
                
                if (hitPlayers.Length > 0)
                    hitPlayers[0].GetComponent<PlayerCombat>().TakePlayerDamage(attackDamage);
            }

            yield return new WaitForSeconds(0.7f);
            animator.speed = 1f;
            agent.isStopped = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;
        
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    // Rotate to face the target
    void FaceTarget()
    {
        Vector3 direction = (player.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    public IEnumerator TakeDamage(int damage)
    {
        if (!isDead)
        {
            //playerAudioSource.PlayOneShot(skeletonHitAudio, 0.7f);
            // animator.speed = 1.75f;
            agent.isStopped = true;
            // currentHealth -= damage;
            // Play hurt animation
            nextAttack = Time.time + 1f;
            animator.SetTrigger("Hurt");
            currentHealth = SubtractEnemyHealth(damage);
            if (currentHealth <= 0)
            {
                isDead = true;
                playerAudioSource.PlayOneShot(skeletonDeadAudio, 0.7f);
                StartCoroutine(Die());
            }

            yield return new WaitForSeconds(0.5f);
            // animator.speed = 1f;
            if (!isDead)
                agent.isStopped = false;
        }
    }
    
    

    private IEnumerator Die()
    {
        // Debug.Log("Enemy died!");
        mz.RemoveEnemyNode(gameObject, 0);
        // Play death animation
        animator.speed = 1f;
        animator.SetBool("IsDead", true);
        // agent.isStopped = true;
        GetComponent<CapsuleCollider>().enabled = false;
        // GetComponent<MeshCollider>().enabled = false;
        transform.GetChild(2).gameObject.SetActive(false);
        transform.GetChild(3).gameObject.SetActive(false);
        // GetComponent<NavMeshAgent>().enabled = false;
        
        enabled = false;
        // mz.RemoveEnemyNode(gameObject);
        // enabled = false;

        Instantiate(coins, agent.transform.position, Quaternion.identity);
        tutorialScript.GetComponent<tutorial>().onEnemyDeath();
        yield return new WaitForSeconds(6f);
        Destroy(gameObject);
    }
}
