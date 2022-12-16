using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedPotion : InventoryItemBase
{
    public GameObject player;
    private PlayerStats playerStats;
    public Inventory inventory;
    private int potionHealth = 20;
    public AudioSource playerAudioSource;
    public AudioClip potionAudio;
    
    
    private void Start()
    {
        playerStats = player.GetComponent<PlayerStats>();
    }

    public override void OnUse()
    {
        playerAudioSource.PlayOneShot(potionAudio, 0.7f);
        gameObject.SetActive(true);
        inventory.RemoveItem(this);
        mz.RemoveEnemyNode(gameObject, 1);
        Destroy(gameObject);
        int currentHealth = playerStats.currentHealth;
        int maxHealth = playerStats.maxHealth;
        if (currentHealth < maxHealth)
        {
            if (currentHealth + potionHealth >= maxHealth)
            {
                playerStats.AddHealth(maxHealth - currentHealth);
            }
            else
            {
                playerStats.AddHealth(potionHealth);
            }
        }
    }
    
    public override void OnPickup()
    {
        // mz.RemoveEnemyNode(gameObject, 1);
        // Destroy(gameObject);
        tutorialScript.GetComponent<tutorial>().onPowerUpPickUp();
        pickedUp = true;
        gameObject.SetActive(false);
    }
}
