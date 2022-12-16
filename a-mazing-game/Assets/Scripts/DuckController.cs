using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;


public class DuckController : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public Transform attackPoint;
    public LayerMask playerLayers;
    public GameObject coins;
    public EnemyHealthBar healthBar;
    public AudioSource duckAudio;
    public AudioClip duckHurtAudio;
    public AudioClip duckDeadAudio;
    public StartBossFight startFight;
    
    public float lookRadius;
    public float wanderRadius;
    public float throwRadius;
        
    public float attackRange = 1.25f;
    public int attackDamage = 40;
    
    public int maxHealth = 500;
    public int currentHealth;

    public GameOverScreen victoryScreen;
    
    private Animator animator;
    [SerializeField] private Transform football;
    [SerializeField] private Transform cheercone;
    [SerializeField] private Transform hand;
    
    private float wanderSpeed = 1.25f;
    private float runSpeed = 2.25f;
    
    private float attackRate = 2f;
    private float throwRate = 2.5f;
    private float nextAttack;
    private float nextThrow;
    private bool isDead;
    public MazeConstructor mz;

    void Start()
    {
        duckAudio = GetComponent<AudioSource>();
        animator = GetComponentInChildren<Animator>();
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        healthBar.SetHealth(maxHealth);
        duckAudio.clip = duckHurtAudio;
        agent.isStopped = true;
    }

    void Update()
    {
        if (startFight.startFight)
        {
            animator.SetTrigger("Getup");
            while (animator.GetCurrentAnimatorStateInfo(0).IsName("Standup") 
                    || animator.GetCurrentAnimatorStateInfo(0).IsName("Pushups"))
            {
                return;
            }
        }
        else
        {
            return;
        }
        
        // Distance to the target
        float distance = Vector3.Distance(player.position, transform.position);

        // If not inside the lookRadius
        if (distance >= lookRadius)
        {
            // If inside the throw radius
            if (distance < throwRadius)
            {
                // football.gameObject.SetActive(true);
                cheercone.gameObject.SetActive(false);
                FaceTarget();
                // Idle();
                if (Time.time > nextThrow)
                {
                    nextThrow = Time.time + throwRate;
                    StartCoroutine(Throw());
                }
                
            }
            
            // If not inside the throw radius
            if (distance >= throwRadius)
            {
                football.gameObject.SetActive(false);
                cheercone.gameObject.SetActive(false);
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
        }

        // If inside the look radius
        if (distance < lookRadius)
        {
            football.gameObject.SetActive(false);
            cheercone.gameObject.SetActive(true);
            StopCoroutine(Throw());
            FaceTarget();
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
            
            Collider[] hitPlayers = Physics.OverlapSphere(attackPoint.position, attackRange, playerLayers);
            if (hitPlayers.Length > 0)
            {
                // Debug.Log("Player hit");
                hitPlayers[0].GetComponent<PlayerCombat>().TakePlayerDamage(attackDamage);
            }

            yield return new WaitForSeconds(0.7f);
            animator.speed = 1f;
            agent.isStopped = false;
        }
    }
    
    private IEnumerator Throw()
    {
        if (!isDead)
        {
            football.gameObject.SetActive(true);
            agent.isStopped = true;
            int health = currentHealth;
            animator.speed = 1.0f;
            animator.SetTrigger("Throw");
            yield return new WaitForSeconds(0.75f);
            transform.LookAt(player);
            Vector3 throwDir = transform.forward;
            football.gameObject.SetActive(false);
            if (health == currentHealth)
            {
                Transform newFootball = Instantiate(football, hand.position, Quaternion.identity);
                newFootball.gameObject.SetActive(true);
                newFootball.localScale = new Vector3(12, 12, 12);
                newFootball.GetComponent<Football>().Setup(throwDir);
            }

            yield return new WaitForSeconds(0.7f);
            football.gameObject.SetActive(true);
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

    public void TakeDamage(int damage)
    {
        if (!isDead)
        {
            currentHealth = SubtractEnemyHealth(damage);
            duckAudio.Play();
            if (currentHealth <= 0)
            {
                animator.SetTrigger("Hurt");
                isDead = true;
                StartCoroutine(Die());
            }
        }
    }
    
    private IEnumerator Die()
    {
        // Play death animation
        duckAudio.clip = duckDeadAudio;
        duckAudio.Play();
        animator.speed = 1f;
        animator.SetBool("IsDead", true);
        GetComponent<CapsuleCollider>().enabled = false;
        healthBar.gameObject.SetActive(false);

        // mz.RemoveEnemyNode(gameObject, 0);
        enabled = false;
        // mz.RemoveEnemyNode(gameObject);

        Instantiate(coins, agent.transform.position, Quaternion.identity);
        yield return new WaitForSeconds(3f);

        // TODO: Call game over screen
        victoryScreen.Setup();
    }
}
