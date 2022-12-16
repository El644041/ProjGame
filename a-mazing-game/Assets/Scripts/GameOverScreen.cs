using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine.Serialization;


[RequireComponent(typeof(MazeConstructor))]


public class GameOverScreen : MonoBehaviour
{
    public GameObject player;
    public GameObject controller;
    public Text pointsText;
    public Text timeText;
    public Text coinsCollected;
    public Text remainingCoins;
    public Text healthStat;
    public Text damageStat;
    public Text staminaStat;
    public BaseStats stats;
    private int maxHealth; //max health counter
    private float maxDamage; //max damage counter
    private float maxStamina; // max stamina counter
    private int numCoins;
    private PlayerStats playerStats;
    private Inventory2 inventory;
    private BaseStats baseStats;

    // TODO
    public NavMeshSurface surface;

    // Start is called before the first frame update

    private void Awake()
    {
        playerStats = player.GetComponent<PlayerStats>();
        baseStats = controller.GetComponent<BaseStats>();
        inventory = player.GetComponent<Inventory2>();

    }
    public void Setup()
    {
        gameObject.SetActive(true);
        Time.timeScale = 0;
        //pointsText.text = "Enemies Killed: " + playerStats.enemiesKilled.ToString();
        //timeText.text = "Time: ";
        maxHealth = playerStats.maxHealth;
        maxDamage = playerStats.maxDamage;
        maxStamina = playerStats.maxStamina;
        numCoins = inventory.numCoins + PlayerPrefs.GetInt("coins", 0);
        healthStat.text = maxHealth.ToString();
        remainingCoins.text = numCoins.ToString();
        damageStat.text = maxDamage.ToString();
        staminaStat.text = maxStamina.ToString();
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void TakeDown()
    {
        setStats();
        PlayerPrefs.SetInt("health", maxHealth);
        PlayerPrefs.SetFloat("damage", maxDamage);
        PlayerPrefs.SetFloat("stamina", maxStamina);
        PlayerPrefs.SetInt("coins", numCoins);
        gameObject.SetActive(false);

    }


    public void ContinueButton()
    {
        setStats();
        Time.timeScale = 1;
        SceneManager.LoadScene("Scene");
    }

    public void MenuButton()
    {
        setStats();
        Time.timeScale = 1;
        SceneManager.LoadScene("Menu");
    }

    public void increaseHealth()
    {
        if (numCoins >= 20)
        {
            maxHealth += 10;
            numCoins -= 20;
            remainingCoins.text = numCoins.ToString();
            healthStat.text = maxHealth.ToString();
        }

    }

    public void decreaseHealth()
    {
        if (maxHealth > 10)
        {
            maxHealth = maxHealth - 10;
            numCoins = numCoins + 20;
            remainingCoins.text = numCoins.ToString();
            healthStat.text = maxHealth.ToString();
        }

    }

    public void increaseDamage()
    {
        if (numCoins >= 20)
        {
            maxDamage += 0.05f;
            numCoins -= 20;
            remainingCoins.text = numCoins.ToString();
            damageStat.text = maxDamage.ToString();
        }

    }

    public void decreaseDamage()
    {
        if (maxDamage > 0.05f)
        {
            maxDamage -= 0.05f;
            numCoins = numCoins + 20;
            remainingCoins.text = numCoins.ToString();
            damageStat.text = maxDamage.ToString();
        }

    }

    public void increaseStamina()
    {
        if (numCoins >=  20)
        {
            maxStamina += 10;
            numCoins -= 20;
            remainingCoins.text = numCoins.ToString();
            staminaStat.text = maxStamina.ToString();
        }

    }
    public void decreaseStamina()
    {
        if (maxStamina > 10)
        {
            maxStamina = maxStamina - 10;
            numCoins = numCoins + 20;
            remainingCoins.text = numCoins.ToString();
            staminaStat.text = maxStamina.ToString();
        }

    }


    private void setStats()
    {
        PlayerPrefs.SetInt("health", maxHealth);
        PlayerPrefs.SetFloat("damage", maxDamage);
        PlayerPrefs.SetFloat("stamina", maxStamina);
        PlayerPrefs.SetInt("coins", numCoins);
    }


}
