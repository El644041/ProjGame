using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading;
using UnityEngine.EventSystems;

public class PlayerCombat : MonoBehaviour
{
    #region public members
    public Animator animator;
    public Transform attackPoint;
    public GameOverScreen GameOverScreen;
    public GameObject hud;
    public LayerMask enemyLayers;
    public TimeSpan elapsed;
    public float attackRange;
    public bool controlEnabled;
    public bool heavyAttack;
    public int swordDamage;
    public int arrowDamage;
    public bool isDead;
    public float attackRate = 1f;
    private AudioSource[] playerAudioSource;
    public AudioClip playerHurtAudio;
    public AudioClip swordAudio;
    public AudioClip punchAudio;

    // public Inventory inventory;

    public Inventory inventory;
    // public InventoryItemBase currentItem;
    #endregion
    
    #region private members
    private DateTime startTime;
    private DateTime endTime;
    private FpsMovement fps;
    private PlayerStats playerStats;
    private FpsMovement movement;
    // private CharacterController cc;
    private float punchRate = 0.6f;
    private float shootRate = 1.0f;
    private float nextPunch;
    private float nextAttack;
    private float nextShot;
    private int attackDamage;
    private bool showingEnd;
    private bool canHook;
    private float attackChainCounter;
    private bool isShooting;
    
    [SerializeField] private Transform bow;
    [SerializeField] private Transform arrow;
    [SerializeField] private Transform arrow2;


    #endregion

    void Start()
    {
        playerAudioSource = GetComponents<AudioSource>();
        playerStats = GetComponent<PlayerStats>();
        movement = GetComponent<FpsMovement>();
        // cc = GetComponent<CharacterController>();
        fps = GetComponent<FpsMovement>();
        // currentItem = GetComponent<PlayerController>().mCurrentItem;
        // attackDamage = playerStats.attackDamage;
        startTime = DateTime.Now;
        endTime = DateTime.Now;
        controlEnabled = true;
        
    }
    
    void Update()
    {
        if (!isDead && fps.IsArmed)
        {
            if (fps.mCurrentItem.Name == "Bow")
            {
                if (!isShooting)
                {
                    if (inventory.hasArrows)
                        arrow.gameObject.SetActive(true);
                    else
                    {
                        arrow.gameObject.SetActive(false);
                    }
                }

                if (Input.GetKeyDown(KeyCode.Mouse0) && Time.time > nextShot)
                {
                    if (Cursor.lockState == CursorLockMode.Locked)
                    {
                        nextShot = Time.time + shootRate;
                        StartCoroutine(Shoot());
                    }
                }
            }
            else
            {
                attackRange = 0.75f;
                if (Input.GetKeyDown(KeyCode.Mouse0) && Time.time > nextAttack)
                {
                    if (Cursor.lockState == CursorLockMode.Locked)
                    {
                        nextAttack = Time.time + attackRate;
                        StartCoroutine(Attack());
                    }
                }
            }
        }
        else if (!isDead && !fps.IsArmed)
        {
            attackRange = 0.6f;
            if (!canHook && Input.GetKeyDown(KeyCode.Mouse0) && Time.time > nextPunch)
            {
                if (Cursor.lockState == CursorLockMode.Locked)
                {
                    animator.speed = 1.5f;
                    animator.SetTrigger("Punch");
                    nextPunch = Time.time + punchRate;
                    StartCoroutine(Punch());
                }
            }
            
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Left Punch"))
            {
                canHook = true;
                attackChainCounter = Time.time;
            }
            else if (Time.time - attackChainCounter > 0.5f)
            {
                canHook = false;
            }
            
            if (canHook && Input.GetKeyDown(KeyCode.Mouse0) && Time.time > nextPunch)
            {
                if (Cursor.lockState == CursorLockMode.Locked)
                {
                    animator.speed = 1.5f;
                    animator.SetTrigger("Right Hook");
                    nextPunch = Time.time + punchRate;
                    StartCoroutine(Punch());
                }
            }
        }
    }

    private IEnumerator Shoot()
    {
        isShooting = true;
        if (inventory.hasArrows)
        {
            arrow.gameObject.SetActive(false);
            foreach (InventorySlot slot in inventory.mSlots)
            {
                if (!slot.IsEmpty && slot.FirstItem.Name == "Arrows")
                {
                    // slot.FirstItem.OnUse();
                    inventory.RemoveItem(slot.FirstItem);
                }
            }
            
            // int damage = 20;
            float currentHealth = playerStats.currentHealth;
            float currentOvershield = playerStats.currentOvershield;
            //playerStats.attackDamage = 40;
            if (!isDead)
            {
                // arrow2.gameObject.SetActive(false);
                // int health = currentHealth;
                // animator.speed = 1.0f;
                animator.SetTrigger("Attack");
                yield return new WaitForSeconds(0.3f);
                arrow.gameObject.SetActive(false);
                arrow2.gameObject.SetActive(true);
                yield return new WaitForSeconds(0.5f);
                // transform.LookAt(player);
                Vector3 shootDir = fps.headCam.transform.forward;
                // arrow.gameObject.SetActive(false);
                if (currentHealth == playerStats.currentHealth && currentOvershield == playerStats.currentOvershield)
                {
                    Transform newArrow = Instantiate(arrow2, arrow2.position, arrow2.rotation);
                    newArrow.gameObject.SetActive(true);
                    arrow2.gameObject.SetActive(false);
                    // newArrow.localScale = new Vector3(12, 12, 12);
                    newArrow.GetComponent<Arrow>().Setup(shootDir);
                }

                yield return new WaitForSeconds(0.4f);
                arrow.gameObject.SetActive(true);
                isShooting = false;
            }
        }
    }

    private IEnumerator Punch()
    {
        // int damage = 20;
        int punchDamage = Mathf.RoundToInt((float)(playerStats.attackDamage * 20f));
        // attackRange = 0.6f;
        // Play attack animation
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Left Punch"))
        {
            punchRate = 0.8f;
            yield return new WaitForSeconds(0.3f);
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Right Hook"))
        {
            punchRate = 0.6f;
            yield return new WaitForSeconds(0.5f);
        }

        // Detect enemies in range of attack
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);

        // Damage them
        // foreach (Collider enemy in hitEnemies)
        // {
        int length = hitEnemies.Length;
        if (length > 0)
        {
            playerAudioSource[1].PlayOneShot(punchAudio, 0.7f);
            // Debug.Log(hitEnemies[length - 1].name + " hit!");
            if (hitEnemies[length - 1].CompareTag("Duck"))
            {
                Vector3 forceDir = Vector3.back;
                hitEnemies[length - 1].GetComponent<Rigidbody>().AddForce(5 * forceDir, ForceMode.Impulse);
                hitEnemies[length - 1].GetComponent<DuckController>().TakeDamage(punchDamage);
            }
            else if (hitEnemies[length - 1].CompareTag("Mage"))
            {
                StartCoroutine(hitEnemies[length - 1].GetComponent<MageController>().TakeDamage(punchDamage));
                if (hitEnemies[length - 1].GetComponent<MageController>().currentHealth <= 0)
                {
                    playerStats.enemiesKilled++;
                }
            }
            else if (hitEnemies[length - 1].CompareTag("Enemy"))
            {
                StartCoroutine(hitEnemies[length - 1].GetComponent<AIMovement>().TakeDamage(punchDamage));
                if (hitEnemies[length - 1].GetComponent<AIMovement>().currentHealth <= 0)
                {
                    playerStats.enemiesKilled++;
                }
            }
        }
    }

    private IEnumerator Attack()
    {
        // attackRange = 0.75f;
        int attackType;
        float currentHealth = playerStats.currentHealth;
        float currentOvershield = playerStats.currentOvershield;
        // Play attack animation
        animator.SetTrigger("Attack");
        if (Input.GetKey(KeyCode.LeftShift) && movement.isSprintingForward)
        {
            if (fps.CarriesItem("Sword Epic"))
            {
                animator.speed = 1f;
                attackType = 3;
            }
            else
            {
                animator.speed = 0.75f;
                attackType = 1;
            }
            animator.SetFloat("AttackMode", 0.5f);
        }
        else
        {
            if (fps.CarriesItem("Sword Epic"))
            {
                attackType = 2;
            }
            else
            {
                attackType = 0;
            }
            animator.SetFloat("AttackMode", 0f);
        }
        
        if (attackType == 0)
        {
            // Katana normal attack
            swordDamage = Mathf.RoundToInt((float)(playerStats.attackDamage * 40f));
            yield return new WaitForSeconds(0.1f);
            controlEnabled = true;
        }
        else if (attackType == 1)
        {
            // Katana special attack
            swordDamage = Mathf.RoundToInt((float)(playerStats.attackDamage * 80f));
            heavyAttack = true;
            movement.runSpeed = 4.0f;
            yield return new WaitForSeconds(0.9f);
            controlEnabled = false;
        }
        else if (attackType == 2)
        {
            // Great sword normal attack
            swordDamage = Mathf.RoundToInt((float)(playerStats.attackDamage * 60f));
            yield return new WaitForSeconds(0.7f);
            controlEnabled = true;
        }
        else if (attackType == 3)
        {
            // Great sword special attack
            swordDamage = Mathf.RoundToInt((float)(playerStats.attackDamage * 100f));
            heavyAttack = true;
            movement.runSpeed = 4.0f;
            yield return new WaitForSeconds(1.3f);
            controlEnabled = false;
        }

        if (currentHealth == playerStats.currentHealth && currentOvershield == playerStats.currentOvershield)
        {
            // Detect enemies in range of attack
            Collider[] hitEnemies =
                Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers); // Damage them
            
            foreach (Collider enemy in hitEnemies)
            {
                playerAudioSource[1].PlayOneShot(swordAudio, 0.7f);
                // Debug.Log(enemy.name + " hit!");
                if (enemy.CompareTag("Duck"))
                {
                    Vector3 forceDir = Vector3.back;
                    enemy.GetComponent<Rigidbody>().AddForce(5 * forceDir, ForceMode.Impulse);
                    enemy.GetComponent<DuckController>().TakeDamage(swordDamage);
                }
                else if (enemy.CompareTag("Mage"))
                {
                    StartCoroutine(enemy.GetComponent<MageController>().TakeDamage(swordDamage));
                    if (enemy.GetComponent<MageController>().currentHealth <= 0)
                    {
                        playerStats.enemiesKilled++;
                    }
                }
                else if (enemy.CompareTag("Enemy"))
                {
                    StartCoroutine(enemy.GetComponent<AIMovement>().TakeDamage(swordDamage));
                    // Debug.Log(enemy.name + " hit!");
                    if (enemy.GetComponent<AIMovement>().currentHealth <= 0)
                    {
                        playerStats.enemiesKilled++;
                    }  
                }
            }
        }

        // cc.enabled = false;
        yield return new WaitForSeconds(0.5f);
        heavyAttack = false;
        movement.runSpeed = 4f;
        // cc.enabled = true;
        controlEnabled = true;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;
        
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    public void TakePlayerDamage(int damage)
    {
        playerAudioSource[1].PlayOneShot(playerHurtAudio, 1.0f);
        int currentOvershield = playerStats.currentOvershield;
        int currentHealth = playerStats.currentHealth;
        int remainder;
        if (currentOvershield > 0)
        {
            playerStats.SubtractOvershield(damage);
            remainder = damage - currentOvershield;
            if (remainder > 0)
            {
                currentHealth = playerStats.SubtractHealth(remainder);
            }
        }
        else
        {
            playerStats.currentOvershield = 0;
            currentHealth = playerStats.SubtractHealth(damage);
        }
        animator.SetTrigger("Hurt");
        
        if (currentHealth <= 0)
        {
            PlayerDie();
        }
    }

    public void endGame(GameObject trigger, GameObject other)
    {
        /*
         * This method will end the game when the end of the
         * maze object is touched
         */

        Debug.Log(other.tag);
        if (other.tag == "Player")
        {
            PlayerDie();
        }

    }

    private void PlayerDie()
    {
        // Debug.Log("Player died!");
        // animator.SetBool("IsDead", true);
        // GetComponent<Collider>().enabled = false;
        // GetComponent<CharacterController>().enabled = false;
        // GetComponent<FpsMovement>().enabled = false;
        //TimeSpan time = GetComponent<GameController>().elapsed;
        isDead = true;
        endTime = DateTime.Now;
        elapsed = endTime - startTime;
        hud.SetActive(false);
        GameOverScreen.Setup();
    }
}
