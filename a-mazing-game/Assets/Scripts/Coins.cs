using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coins : MonoBehaviour
{
    private AudioSource[] playerAudioSource;
    public AudioClip coinAudio;
    // Start is called before the first frame update

    private void Start()
    {
        playerAudioSource = GameObject.FindGameObjectWithTag("Player").GetComponents<AudioSource>();

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            playerAudioSource[1].PlayOneShot(coinAudio, 0.7f);
            other.GetComponent<Inventory2>().numCoins++;
            other.GetComponent<PlayerStats>().AddCoins();
            Destroy(gameObject);
        }
    }
}
