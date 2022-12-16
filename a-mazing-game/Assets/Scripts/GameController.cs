using System;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.AI;

[RequireComponent(typeof(MazeConstructor))]

public class GameController : MonoBehaviour

{
    //1
    [SerializeField] private FpsMovement player;
    [FormerlySerializedAs("timeLabel")] 
    //[SerializeField] private Text healthLabel;
    //[SerializeField] private Text staminaLabel;
    [SerializeField] private Text scoreLabel;
    public Text coinLabel;

    private MazeConstructor generator;
    
    public NavMeshSurface surface;
    public MazeConstructor mz;

    //2
    private DateTime startTime;
    private int timeLimit;
    private int reduceLimitBy;
    // public int health;
    // public int maxHealth;
    private DateTime endTime;
    public TimeSpan elapsed;

    private bool showingEnd;
    public GameOverScreen GameOverScreen;
    public PauseScreen PauseScreen;

    public GameObject hud;

    private int score;
    private bool goalReached;
    public int test = 1;

    public float x;

    public float y;

    public float z;
    //3
    void Start() {
        //maxHealth = 10;
        generator = GetComponent<MazeConstructor>();
        StartNewGame();
        surface.BuildNavMesh();
        hud.SetActive(true);
    }

    //4
    private void StartNewGame()
    {
        timeLimit = 10;
        reduceLimitBy = 5;
        startTime = DateTime.Now;
        //healthBar.SetMaxHealth(maxHealth);
        hud.SetActive(true);
        // gameObject.Find("EventSystem").SetActive(false);
        StartNewMaze();
    }

    public void movePlayer()
    {
        x = generator.startCol * generator.hallWidth;
        y = 1;
        z = generator.startRow * generator.hallWidth;
        player.transform.position = new Vector3(z, y, z);

    }

    //5
    private void StartNewMaze()
    {
        //attempts to get rows and cols, if not found sets to 25
        int numberRows = PlayerPrefs.GetInt("rows", 25);
        int numberCols = PlayerPrefs.GetInt("cols", 25);
        generator.GenerateNewMaze(numberRows,numberCols, OnStartTrigger, reduceHealth, endGame) ;
        x = generator.startCol * generator.hallWidth;
        y = 1;
        z = generator.startRow * generator.hallWidth;
        
        // Thread.Sleep(10000);
        // Debug.Log("start row " + x + " start col " + z);
        movePlayer();
        
        goalReached = false;
        showingEnd = false;
        player.enabled = true;

        // restart timer
        timeLimit -= reduceLimitBy;
        startTime = DateTime.Now;
    }

    public void StartFromLastPlay()
    {
        GameOverScreen.TakeDown();
        hud.SetActive(true);
        startTime = DateTime.Now;
        //healthBar.SetMaxHealth(maxHealth);
        player.GetComponent<PlayerStats>().SetUp();
        player.GetComponent<Inventory2>().SetUp();

        StartNewMaze();

    }
    
    public void DestroyPowerUp(GameObject trigger, GameObject other)
    {
        mz.RemoveEnemyNode(trigger, 1);
        Debug.Log(trigger.tag);
        Destroy(trigger);

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
            //healthLabel.text = "You have reached the end of the maze!";
            player.enabled = false;
            Destroy(trigger);
            endTime = DateTime.Now;
            elapsed = endTime - startTime;
            //score = player.GetComponent<PlayerStats>().enemiesKilled;
            hud.SetActive(false);
            GameOverScreen.Setup();
        }
        
    }

  
    void reduceHealth(GameObject trigger, GameObject other)
    {
        /*
         * Reduces the player's health
         */
        
        
        //health -= 5;
        //healthBar.SetHealth(health);
        // goalReached = true;  // todo remove
        //healthLabel.text = health.ToString();
        Debug.Log("took 5 damage");
        Destroy(trigger);
        // Invoke("StartNewGame", 1);
        
    }

    //6
    private void Update()
    {
        if (!player.enabled)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (!PauseScreen.isPaused)
            {

                PauseScreen.pauseGame();
            }
            else
            {
                PauseScreen.resumeGame();
            }
        }
        // healthLabel.fontSize = 75;
        // Invoke("StartNewGame", 4);
    }

    
    //7
    private void OnGoalTrigger(GameObject trigger, GameObject other)
    {
        Debug.Log("Goal!");
        goalReached = true;

        score += 1;
        scoreLabel.text = score.ToString();
        

        Destroy(trigger);
    }

    private void OnStartTrigger(GameObject trigger, GameObject other)
    {
        if (goalReached)
        {
            Debug.Log("Finish!");
            player.enabled = false;
    
            Invoke("StartNewMaze", 4);
        }
    }
}
