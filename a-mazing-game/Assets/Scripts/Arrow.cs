using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Arrow : MonoBehaviour
{
    private Vector3 shootDir;
    public AudioSource playerAudioSource;
    [SerializeField] private PlayerStats playerStats;
    public AudioClip arrowAudio;
    public GameObject player;
    bool isPlaying = false;

    public void Setup(Vector3 shootDir)
    {
        this.shootDir = shootDir;
        Destroy(gameObject, 5f);
    }

    // Update is called once per frame
    private void Update()
    {
        float shootSpeed = 50f;
        transform.position += shootDir * shootSpeed * Time.deltaTime;
    }

    private IEnumerator OnTriggerEnter(Collider other)
    {
        if (!isPlaying)
        {
            playerAudioSource.PlayOneShot(arrowAudio, 0.7f);
            isPlaying = true;
        }
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<NavMeshAgent>().isStopped = true;
            int damage = Mathf.RoundToInt((float)(playerStats.attackDamage * 40f));
            StartCoroutine(other.GetComponent<AIMovement>().TakeDamage(damage));
            yield return new WaitForSeconds(0.5f);
            other.GetComponent<NavMeshAgent>().isStopped = false;
            Destroy(gameObject);
            if (other.GetComponent<AIMovement>().currentHealth <= 0)
            {
                playerStats.enemiesKilled++;
            }
        }
        else if (other.CompareTag("Mage"))
        {
            other.GetComponent<NavMeshAgent>().isStopped = true;
            int damage = Mathf.RoundToInt((float)(playerStats.attackDamage * 40f));
            StartCoroutine(other.GetComponent<MageController>().TakeDamage(damage));
            yield return new WaitForSeconds(0.5f);
            other.GetComponent<NavMeshAgent>().isStopped = false;
            Destroy(gameObject);
            if (other.GetComponent<MageController>().currentHealth <= 0)
            {
                playerStats.enemiesKilled++;
            }
        }
        else if (other.CompareTag("Duck"))
        {
            int damage = Mathf.RoundToInt((float)(playerStats.attackDamage * 40f));
            other.GetComponent<DuckController>().TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}

