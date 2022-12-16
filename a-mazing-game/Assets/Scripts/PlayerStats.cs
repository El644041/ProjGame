using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    public float attackDamage;
    public float maxDamage;
    public int maxHealth;
    public int currentHealth;
    public int maxOvershield;
    private int intStamina;
    public int currentOvershield;
    public float maxStamina;
    public float currentStamina;
    public int enemiesKilled = 0;
    private int numCoins;

    public HealthBar healthBar;
    public StaminaBar staminaBar;
    public OvershieldBar overshieldBar;
    public GameObject controller;

    public Text healthLabel;
    public Text staminaLabel;
    public Text coinLabel;

    private WaitForSeconds regenTick = new WaitForSeconds(0.05f);
    private Coroutine regen;
    
    private void Awake()
    {

        controller = GameObject.Find("Controller");
        maxHealth = PlayerPrefs.GetInt("health", 100);
        maxDamage = PlayerPrefs.GetFloat("damage", 1.0f);
        maxStamina = PlayerPrefs.GetFloat("stamina", 100);
        
        currentHealth = maxHealth;
        attackDamage = maxDamage;
        healthBar.SetMaxHealth(maxHealth);
        healthBar.SetHealth(maxHealth);
        overshieldBar.SetMaxOvershield(maxOvershield);
        overshieldBar.SetOvershield(0);
        staminaBar.SetMaxStamina(maxStamina);
        staminaBar.SetStamina(maxStamina);
        currentStamina = maxStamina;
        healthLabel.text = maxHealth.ToString() + "/" + maxHealth.ToString();
        staminaLabel.text = maxStamina.ToString() + "/" + maxStamina.ToString();
        numCoins = GetComponent<Inventory2>().numCoins + PlayerPrefs.GetInt("coins", 0);
        coinLabel.text = "Coins: " + numCoins.ToString();
    }


    public void SetUp()
    {
        controller = GameObject.Find("Controller");
        maxHealth = PlayerPrefs.GetInt("health", 100);
        maxDamage = PlayerPrefs.GetFloat("damage", 1.0f);
        maxStamina = PlayerPrefs.GetFloat("stamina", 100);
        currentHealth = maxHealth;
        attackDamage = maxDamage;
        currentStamina = maxStamina;
        healthBar.SetHealth(maxHealth);
        overshieldBar.SetMaxOvershield(maxOvershield);
        overshieldBar.SetOvershield(0);
        healthBar.SetMaxHealth(maxHealth);
        healthBar.SetHealth(maxHealth);
        overshieldBar.SetMaxOvershield(maxOvershield);
        overshieldBar.SetOvershield(0);
        staminaBar.SetMaxStamina(maxStamina);
        staminaBar.SetStamina(maxStamina);
        intStamina = (int) currentStamina;
        staminaLabel.text = intStamina.ToString() + "/" + maxStamina.ToString();
        healthLabel.text = currentHealth.ToString() + "/" + maxHealth.ToString();
    }


    public void AddCoins()
    {
        numCoins += 1;
        coinLabel.text = "Coins: " + numCoins.ToString();
    }
    public int SubtractHealth(int damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);
        healthLabel.text = currentHealth.ToString() + "/" + maxHealth.ToString();
        return currentHealth;
    }

    public void AddHealth(int health)
    {
        currentHealth += health;
        if (currentHealth > maxHealth)
        {
            healthBar.SetHealth(maxHealth);
        }
        else
        {
            healthBar.SetHealth(currentHealth);
        }
        healthLabel.text = currentHealth.ToString() + "/" + maxHealth.ToString();
    }
    
    public void AddOvershield(int overshield)
    {
        currentOvershield += overshield;
        if (currentOvershield > maxOvershield)
        {
            overshieldBar.SetOvershield(maxHealth);
        }
        else
        {
            overshieldBar.SetOvershield(currentOvershield);
        }
    }
    
    public int SubtractOvershield(int damage)
    {
        currentOvershield -= damage;
        overshieldBar.SetOvershield(currentOvershield);
        return currentOvershield;
    }

    public void IncreaseMaxHealth(int health)
    {
        int newHealth = maxHealth + health;
        healthBar.SetMaxHealth(newHealth);
        maxHealth = newHealth;
    }

    public bool UseStamina(float amount)
    {
        if (currentStamina - amount >= 0)
        {
            currentStamina -= amount;
            staminaBar.SetStamina(currentStamina);
            intStamina = (int)currentStamina;
            staminaLabel.text = intStamina.ToString() + "/" + maxStamina.ToString();

            if (regen != null)
            {
                StopCoroutine(regen);
            }
            
            regen = StartCoroutine(RegenStamina());
            return true;
        }
        else
        {
            // Debug.Log("Not enough stamina");
            return false;
        }
    }

    public IEnumerator RegenStamina()
    {
        yield return new WaitForSeconds(2);

        while (currentStamina < maxStamina)
        {
            currentStamina += maxStamina / 100;
            staminaBar.SetStamina(currentStamina);
            intStamina = (int)currentStamina;
            staminaLabel.text = intStamina.ToString() + "/" + maxStamina.ToString();
            yield return regenTick;
        }

        regen = null;
    }
}
