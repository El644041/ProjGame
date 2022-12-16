using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class MageController : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask playerLayers;
    public GameObject coins;
    public EnemyHealthBar healthBar;
    public Rigidbody rb;
    public GameObject playerGO;
    
    private AudioSource[] playerAudioSource;
    public AudioClip mageHitAudio;
    public AudioClip mageDeadAudio;
    
    public float lookRadius;
    public float wanderRadius;

    public int attackDamage = 20;
    
    public int maxHealth = 80;
    public int currentHealth;
    
    private Animator animator;
    
    [SerializeField] private Transform fireball;
    [SerializeField] private Transform hand;
    
    private float speed = 1.5f;
    
    private float attackRate = 3f;
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
         fireball.gameObject.SetActive(true);
         playerAudioSource = playerGO.GetComponents<AudioSource>();
    }

    void Update()
    {
        // Distance to the target
        float distance = Vector3.Distance(player.position, transform.position);

        // If not inside the lookRadius
        if (distance >= lookRadius)
        {
            agent.isStopped = false;
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
            Idle();
            agent.isStopped = true;
            if (Time.time > nextAttack)
            {
                nextAttack = Time.time + attackRate;
                StartCoroutine(Cast());
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
        agent.speed = speed;
        animator.SetFloat("Speed", 1f, 0.2f, Time.deltaTime);
    }

    private IEnumerator Cast()
    {
        if (!isDead)
        {
            // fireball.gameObject.SetActive(true);
            agent.isStopped = true;
            int health = currentHealth;
            // animator.speed = 1.0f;
            animator.SetTrigger("Cast");
            yield return new WaitForSeconds(0.95f);
            transform.LookAt(player);
            Vector3 throwDir = transform.forward;
            fireball.gameObject.SetActive(false);
            if (health == currentHealth)
            {
                Transform newFireball = Instantiate(fireball, hand.position, Quaternion.identity);
                newFireball.gameObject.SetActive(true);
                newFireball.gameObject.GetComponent<SphereCollider>().enabled = true;
                // newFireball = new Vector3(12, 12, 12);
                newFireball.GetComponent<Fireball>().Setup(throwDir);
            }

            yield return new WaitForSeconds(0.7f);
            fireball.gameObject.SetActive(true);
            animator.speed = 1f;
            agent.isStopped = false;
        }
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
            agent.isStopped = true;
            rb.AddForce(-transform.forward * 2, ForceMode.Impulse);
            playerAudioSource[1].PlayOneShot(mageHitAudio, 0.4f);
            // animator.speed = 1.75f;
            // currentHealth -= damage;
            // Play hurt animation
            nextAttack = Time.time + 2f;
            animator.SetTrigger("Hurt");
            currentHealth = SubtractEnemyHealth(damage);
            if (currentHealth <= 0)
            {
                isDead = true;
                playerAudioSource[1].PlayOneShot(mageDeadAudio, 2.0f);
                StartCoroutine(Die());
            }

            yield return new WaitForSeconds(1f);
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
        // animator.speed = 1f;
        animator.SetBool("IsDead", true);
        agent.isStopped = true;
        rb.constraints = RigidbodyConstraints.FreezePosition;
        GetComponent<CapsuleCollider>().enabled = false;
        // GetComponent<MeshCollider>().enabled = false;
        transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(2).gameObject.SetActive(false);
        // GetComponent<NavMeshAgent>().enabled = false;

        // mz.RemoveEnemyNode(gameObject, 0);
        enabled = false;
        // mz.RemoveEnemyNode(gameObject);
        // enabled = false;

        Instantiate(coins, agent.transform.position, Quaternion.identity);
        tutorialScript.GetComponent<tutorial>().onEnemyDeath();
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }
}
