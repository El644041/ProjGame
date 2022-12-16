/*
 * written by Joseph Hocking 2017
 * released under MIT license
 * text of license https://opensource.org/licenses/MIT
 *
 * Modified VERY heavily by Jacob
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEditor;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;
using Random = System.Random;

public class MazeConstructor : MonoBehaviour
{
    public bool showDebug;
    private int[] col;
    public int[] row;
    private int[] enemies; // the spawn location of enemies
    public int length;

    

    [SerializeField] private Material mazeMat1;
    [SerializeField] private Material mazeMat2;
    [SerializeField] private Material startMat;
    [SerializeField] private Material treasureMat;
    [SerializeField] private Material endGoal;
    [SerializeField] private Material testSpawn;
    public GameObject start;
    public GameObject enemy;
    public GameObject skeleton;  // Skeleton prefab
    public GameObject healthPotion;  // health prefab
    public GameObject strengthPotion;  // strength prefab
    public GameObject shieldPotion;  // shield prefab
    public GameObject gate;
    public AIMovement ai;
    public NavMeshAgent agent;
    private MeshRenderer mr;  // mesh renderer
    private int[] deadEndCol;  // stores dead column indices
    private int[] deadEndRow;  // stores dead end row indicies
    public int desiredEnemies;  // number of enemies to initially spawn in the maze
    private LinkedList<GameObject> enemyList;  // holds all enemies 
    private LinkedList<GameObject> powerUps;  // holds all spawned powerups
    public LinkedList<GameObject> arrowList;  // holds all spawned arrows
    public LinkedList<GameObject> torchList;  // holds all placed torches
    public GameObject player;  // player gameobject
    public GameObject portal;  // portal game object to get location 
    public InventoryItemBase bottles;
    public int loadTutorial;  // flag to load tutorial level or not
    public GameObject Arrows;
    private int spawnDistance;
    public bool enemyFirstEncounter; // for tutorial. has player encountered an enemy
    public bool powerUpFirstEncounter;  // for tutorial. has player encounter a powerup
    public GameObject mage;

    private int[,] tutorialMaze;
    
    
    public int[,] data
    {
        get; private set;
    }

    public float hallWidth
    {
        get; private set;
    }
    public float hallHeight
    {
        get; private set;
    }

    public int startRow
    {
        get; private set;
    }
    public int startCol
    {
        get; private set;
    }

    public int goalRow
    {
        get; private set;
    }
    public int goalCol
    {
        get; private set;
    }

    private MazeDataGenerator dataGenerator;
    private MazeMeshGenerator meshGenerator;
    private FpsMovement fpsMovement;
    public tutorial tutorialScript;

    void Awake()
    {
        dataGenerator = new MazeDataGenerator();
        meshGenerator = new MazeMeshGenerator();
        fpsMovement = GetComponent<FpsMovement>();
        length = 0;
        agent = GetComponent<NavMeshAgent>();
        int mazeType = PlayerPrefs.GetInt("size", 0);
        loadTutorial = PlayerPrefs.GetInt("tutorial", 0);
        spawnDistance = 10;
        enemyFirstEncounter = false;
        powerUpFirstEncounter = false;
        tutorialScript = GetComponent<tutorial>();
        if (mazeType == 0  || mazeType == 4) // small or tutorial 
        {
            desiredEnemies = 5;
        }
        else if (mazeType == 1)  // medium
        {
            desiredEnemies = 24;
        }
        else  // large
        {
            desiredEnemies = 37;
        }
        enemyList = new LinkedList<GameObject>();
        powerUps = new LinkedList<GameObject>();
        arrowList = new LinkedList<GameObject>();
        torchList = new LinkedList<GameObject>();
        player = GameObject.FindGameObjectWithTag("Player");
        ai = GetComponent<AIMovement>();
        

        // default to walls surrounding a single empty cell
        data = new int[,]
        {
            {1, 1, 1},
            {1, 0, 1},
            {1, 1, 1}
        };
        
        /*
         * This is the tutorial maze so it is not randomly generated each time
         */
        tutorialMaze = new int[,]  // use 11 for column and row
        {
            // {1,1,1,1,1,1,1,1,1,1,1,1,1},
            // {1,0,1,0,1,0,0,0,1,0,0,0,1},
            // {1,0,1,0,1,0,1,1,1,0,1,0,1},
            // {1,0,0,0,1,0,1,0,0,0,1,0,1},
            // {1,0,1,0,1,0,1,0,1,1,1,0,1},
            // {1,0,1,0,0,0,0,0,1,0,0,0,1},
            // {1,0,0,0,1,0,1,1,1,0,1,0,1},
            // {1,0,0,0,1,0,1,0,0,0,1,0,1},
            // {1,1,1,0,1,1,1,1,1,1,1,0,1},
            // {1,0,1,0,0,0,0,0,1,0,1,0,1},
            // {1,0,1,0,1,0,1,1,1,1,1,0,1},
            // {1,0,0,0,1,0,0,0,0,0,0,0,1},
            // {1,1,1,1,1,1,1,1,1,1,1,1,1}
            
            {1,1,1,1,1,1,1,1,1,1,1,1,1},
            {1,0,1,1,1,0,1,0,1,1,1,0,1},
            {1,0,1,1,1,0,1,0,1,1,1,0,1},
            {1,0,1,0,1,0,0,0,0,0,0,0,1},
            {1,0,1,0,1,1,1,0,1,1,1,1,1},
            {1,0,0,0,0,0,0,0,0,0,0,0,1},
            {1,1,1,0,1,0,0,0,0,0,1,1,1},
            {1,0,1,0,1,0,0,0,0,0,0,0,1},
            {1,0,1,0,1,1,1,1,1,0,0,0,1},
            {1,0,0,0,1,0,0,0,0,0,0,0,1},
            {1,0,1,1,0,0,1,0,1,0,1,0,1},
            {1,0,0,0,0,0,1,0,1,0,1,0,1},
            {1,1,1,1,1,1,1,1,1,1,1,1,1}
            
            

        };
    }

    public void GenerateNewMaze(int sizeRows, int sizeCols,
        TriggerEventHandler startCallback=null, TriggerEventHandler goalCallback=null, TriggerEventHandler endGame=null)
    {
        Debug.Log("Row and cols: " + sizeRows + " " + sizeCols);

        if (sizeRows % 2 == 0 && sizeCols % 2 == 0)
        {
            Debug.LogError("Odd numbers work better for dungeon size.");
        }

        DisposeOldMaze();

        if(loadTutorial == 0)
        {
            data = dataGenerator.FromDimensions(sizeRows, sizeCols);
        }
        else
        {
            data = tutorialMaze;
        }
        

        FindStartPosition();
        FindGoalPosition();
        FindDeadEnd();
        Debug.Log("For tut: col: " + col[0] + " row: " + row[0]);
        // printMaze();

        // store values used to generate this mesh
        hallWidth = meshGenerator.width;
        hallHeight = meshGenerator.height;

        DisplayMaze();
        // SpawnEnemy(desiredEnemies);
        // Thread.Sleep(1000);
        GameObject endLocation = PlaceEndTrigger(col[0], row[0], endGame);
        portal = GameObject.FindGameObjectWithTag("Portal");
        SpawnPowerUp(endLocation);
        
        StartCoroutine(UpdateGameObjects());
        if (loadTutorial == 1)
        {
            tutorialPowerUpSpawn();
            tutorialScript.GetComponent<tutorial>().tutorialStartMessage(1);
        }
        else
        {
            StartCoroutine(SpawnCoRoutine());
        }
    }

    public void test()
    {
        Debug.Log("in test");
        StartCoroutine(SpawnCoRoutine());
    }
    
    public void RemoveEnemyNode(GameObject go, int type)
    {
        // 0 = enemy LL
        // 1 = powerUp LL

        LinkedListNode<GameObject> node;
        
        if(type == 0)
        {
            node = enemyList.First;
            while (true)
            {
                if (node.Value == go)
                {
                    enemyList.Remove(node);
                    return;
                }
        
                node = node.Next;
            }
        }

        if (type == 2)
        {
            Debug.Log("removing arrow");
            node = arrowList.First;
            while (true)
            {
                if (node.Value == go)
                {
                    arrowList.Remove(node);
                    return;
                }
        
                node = node.Next;
            }
        }
        else
        {
            node = powerUps.First;
            while (true)
            {
                if (node.Value == go)
                {
                    powerUps.Remove(node);
                    return;
                }
        
                node = node.Next;
            }
        }
    }
    
    private IEnumerator UpdateGameObjects()
    {
        /*
         * Enables / disables spawned game objects based upon their location
         */
        
        while (true)
        {
            LinkedListNode<GameObject> enemyNode = enemyList.First;
            LinkedListNode<GameObject> powerUpNode = powerUps.First;
            LinkedListNode<GameObject> arrowNode = arrowList.First;
            LinkedListNode<GameObject> torchNode = torchList.First;
            while (enemyNode != null)  // enemies
            {
                float distance = Vector3.Distance(player.transform.position, enemyNode.Value.transform.position);
                if (distance > 30)
                {
                    enemyNode.Value.SetActive(false);
                }
                else
                {
                    enemyNode.Value.SetActive(true);
                }
                enemyNode = enemyNode.Next;
            }

            while (powerUpNode != null)  // power ups
            {
                float distance = Vector3.Distance(player.transform.position, powerUpNode.Value.transform.position);
                if (!powerUpNode.Value.GetComponent<InventoryItemBase>().pickedUp)
                {
                    if (distance > 30)
                    {
                        powerUpNode.Value.SetActive(false);
                    }
                    else
                    {
                        powerUpNode.Value.SetActive(true);
                    }
                }
                powerUpNode = powerUpNode.Next;
            }
            
            while (arrowNode != null)  // enemies
            {
                float distance = Vector3.Distance(player.transform.position, arrowNode.Value.transform.position);
                if (distance > 30)
                {
                    arrowNode.Value.SetActive(false);
                }
                else
                {
                    arrowNode.Value.SetActive(true);
                }
                arrowNode = arrowNode.Next;
            }
            
            while (torchNode != null)  // enemies
            {
                float distance = Vector3.Distance(player.transform.position, torchNode.Value.transform.position);
                if (distance > 30)
                {
                    torchNode.Value.SetActive(false);
                }
                else
                {
                    torchNode.Value.SetActive(true);
                }
                torchNode = torchNode.Next;
            }
            yield return new WaitForSeconds(2);
        }
    }

    public IEnumerator SpawnCoRoutine()
    {
        /*
         * Continually spawn enemies within the maze
         */
        
        while (true)
        {
            // Debug.Log("Player pos: " + player.transform.position);
            GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
            int numEnemies = allEnemies.Length;
            int enemiesToSpawn = desiredEnemies - numEnemies;
            
            // Debug.Log("NumEnemies: " + numEnemies);
            // Debug.Log("Enemies to spawn: " + enemiesToSpawn);
            // Debug.Log("Enemies alive: " + aliveEnemies);

            if (loadTutorial == 0)
            {
                SpawnEnemy(enemiesToSpawn + 2);
            }
            else
            {
                tutorialSpawnEnemy(enemiesToSpawn + 2);
                StopCoroutine(SpawnCoRoutine());
            }

            if (spawnDistance != 0)
            {
                spawnDistance -= 2;
            }
                yield return new WaitForSeconds(30);
        }
        

        yield return null;
    }

    void tutorialSpawnEnemy(int numEnemies, TriggerEventHandler goalCallback = null)
    {
        PlaceEnemy(col[63], row[63], goalCallback);
        PlaceEnemy(col[60], row[60], goalCallback);
        PlaceEnemy(col[22], row[22], goalCallback);
        PlaceEnemy(col[56], row[56], goalCallback);
        PlaceEnemy(col[53], row[53], goalCallback);
        PlaceEnemy(col[16], row[16], goalCallback);
        PlaceEnemy(col[50], row[50], goalCallback);
        // enemies = new int[length];
        // for (int i = 0; i < numEnemies; i++)
        // {
        //     System.Random random = new System.Random();
        //     int temp = random.Next(0, length - 1);
        //     while (enemies.Contains(temp))
        //         temp = random.Next(0, length - 1);
        //     PlaceEnemy(col[temp], row[temp], goalCallback);
        //     Debug.Log("From tutorialSpawn: " + temp);
        //     enemies[i] = temp;
        // }
    }
    void SpawnEnemy(int numEnemies, TriggerEventHandler goalCallback=null)
    {
        /*
         * Simple method that spawns enemies in the maze
         */
        
        enemies = new int[length];
        for (int i = 0; i < numEnemies; i++)
        {
            System.Random random = new System.Random();
            int temp = random.Next(0, length - 1);
            while(enemies.Contains(temp))
                temp = random.Next(0, length - 1);
            // if(!(enemies.Contains(temp)))
            PlaceEnemy(col[temp], row[temp], goalCallback);
            enemies[i] = temp;
        }
    }

    void tutorialPowerUpSpawn()
    {
        SpawnHealth(1, 3);
        SpawnShield(1, 4);
    }

    void SpawnPowerUp(GameObject endLocation)
    {
        /*
         * Spawn power ups within the maze
         */
        
        int l = deadEndCol.Length;
        System.Random random = new System.Random();
        // Debug.Log("L length is: " + l);
        
        // guarantee spawning 2 piles of arrows
        for (int i = 0; i < 3; i++)
        {
            if (i > l)
            {
                break;
            }
            SpawnArrow(deadEndCol[i], deadEndRow[i], endLocation);
        }
        
        for (int i = 3; i < l; i++)
        {
            // Debug.Log("Calling shit"); 
            
            int temp = random.Next(1, 4);  // todo change from 0, 4
            Debug.Log(temp);
            if(temp == 0)  // stamina
            {
                // Debug.Log("Random is 0");
                // SpawnStamina(deadEndCol[i], deadEndRow[i], endLocation);
            }   
            
            else if (temp == 1)  // health
            {
                // Debug.Log("Random is 1");
                SpawnHealth(deadEndCol[i], deadEndRow[i], endLocation);
            }
            else if (temp == 2)  // shield
            {
                // Debug.Log("Random is 2");
                SpawnShield(deadEndCol[i], deadEndRow[i], endLocation);
            }
            else if (temp == 3) // arrow pile
            {
                SpawnArrow(deadEndCol[i], deadEndRow[i], endLocation);
            }
        }
    }

    private void DisplayMaze()
    {
        GameObject go = new GameObject();
        go.transform.position = Vector3.zero;
        go.name = "Procedural Maze";
        go.tag = "Generated";
        
        MeshFilter mf = go.AddComponent<MeshFilter>();
        mf.mesh = meshGenerator.FromData(data);

        MeshCollider mc = go.AddComponent<MeshCollider>();
        mc.sharedMesh = mf.mesh;

        mr = go.AddComponent<MeshRenderer>();
        mr.materials = new Material[2] {mazeMat1, mazeMat2};
        // go.AddComponent<NavMeshMo>()
    }

    void debugSpawn(int length)
    {
        for (int i = 0; i < length; i++)
        {
            Debug.Log("DeadEndCol: " + deadEndCol[i] + " eadEndRow: " + deadEndRow[i]);
        }
    }
    
    void printMaze()
    {
        int rMax = data.GetUpperBound(0);  // 1
        int cMax = data.GetUpperBound(1);  // 2
        int[,] maze = data;

        

        // loop top to bottom, left to right
        for (int i = rMax; i >= 0; i--)
        {
            string msg = "";
            for (int j = 0; j <= cMax; j++)
            {
                if (maze[i, j] == 0)
                {
                    msg += "0,";
                }
                else
                {
                    msg += "1,";
                }
            }
            Debug.Log(msg);
        }
    }

    public void DisposeOldMaze()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Generated");
        foreach (GameObject go in objects) {
            Destroy(go);
        }
    }

    private void FindStartPosition()
    {
        int[,] maze = data;
        int rMax = maze.GetUpperBound(0);
        int cMax = maze.GetUpperBound(1);

        for (int i = 0; i <= rMax; i++)
        {
            for (int j = 0; j <= cMax; j++)
            {
                if (maze[i, j] == 0)
                {
                    startRow = i;
                    startCol = j;
                    // Debug.Log("Starting POS. Row: " + i + " " + "Col " + j);
                    return;
                }
            }
        }
    }
    
    private void FindDeadEnd()
    {
        int[,] maze = data;
        int rMax = maze.GetUpperBound(0);
        int cMax = maze.GetUpperBound(1);
        int length = 0;
        // Debug.Log("rmax " + rMax + " cmax " + cMax);

        for (int row = 1; row < rMax; row++)
        {
            for (int col = 1; col < cMax; col++)
            {
                if (maze[row, col] == 0)
                {
                    int left = maze[row, col - 1];
                    int right = maze[row, col + 1];
                    int front = maze[row + 1, col];
                    int back = maze[row - 1, col];
                    
                    if((left == 1 && right == 1) && (front == 1 && back == 0))
                    {
                        // Debug.Log("Check case 1");
                        length++;
                        // break;
                    }

                    if ((left == 0 && right == 1) && (front == 1 && back == 1))
                    {
                        // Debug.Log("Check case 2");
                        length++;
                        // break;
                    }

                    if ((left == 1 && right == 0) && (front == 1 && back == 1))
                    {
                        // Debug.Log("Check case 3");
                        length++;
                        // break;
                    }

                    if ((left == 1 && right == 1) && (front == 0 && back == 1))
                    {
                        // Debug.Log("Check case 4");
                        length++;
                        // break;
                    }
                }
            }
        }

        deadEndCol = new int[length];
        deadEndRow = new int[length];
        int itter = 0;
        
        
        
        for (int i = 1; i < rMax; i++)
        {
            for (int j = 1; j < cMax; j++)
            {
                if (maze[i, j] == 0)
                {
                    int left = maze[i, j - 1];
                    int right = maze[i, j + 1];
                    int front = maze[i + 1, j];
                    int back = maze[i - 1, j];

                    if ((left == 1 && right == 1) && (front == 1 && back == 0))
                    {
                        deadEndCol[itter] = j;
                        deadEndRow[itter] = i;
                        // Debug.Log("Case 1");
                        itter++;
                        // break;
                    }

                    if ((left == 0 && right == 1) && (front == 1 && back == 1))
                    {
                        deadEndCol[itter] = j;
                        deadEndRow[itter] = i;
                        // Debug.Log("Case 2");
                        itter++;
                        // break;
                    }

                    if ((left == 1 && right == 0) && (front == 1 && back == 1))
                    {
                        deadEndCol[itter] = j;
                        deadEndRow[itter] = i;
                        // Debug.Log("Case 3");
                        itter++;
                        // break;
                    }
                    
                    if ((left == 1 && right == 1) && (front == 0 && back == 1))
                    {
                        deadEndCol[itter] = j;
                        deadEndRow[itter] = i;
                        // Debug.Log("Case 4");
                        itter++;
                        // break;
                    }
                }
            }
        }
    }

    private void FindGoalPosition()
    {
        int[,] maze = data;
        int rMax = maze.GetUpperBound(0);
        int cMax = maze.GetUpperBound(1);
        

        // loop top to bottom, right to left
        for (int i = rMax; i >= 0; i--)
        {
            for (int j = cMax; j >= 0; j--)
            {
                if (maze[i, j] == 0)
                {
                    length++;
                    // goalRow = i;
                    // goalCol = j;
                    // return;
                }
            }
        }
        // Debug.Log("rmax " + rMax + " cmax " + cMax + " length " + length);

        col = new int[length];
        row = new int [length];
        int itter = 0;
        
        for (int i = rMax; i >= 0; i--)
        {
            for (int j = cMax; j >= 0; j--)
            {
                if (maze[i, j] == 0)
                {
                    row[itter] = i;
                    col[itter] = j;
                    itter++;
                    
                    // return;
                }
            }
        }
    }
    
    private void SpawnStrength(int column, int newRow, GameObject end, TriggerEventHandler callback=null)
    {
        // Debug.Log("Made it to spawnStamina");
        GameObject strength = Instantiate(strengthPotion);
        strength.transform.position = new Vector3(column * hallWidth, -.5f, newRow * hallWidth);
        float distance = Vector3.Distance(player.transform.position, strength.transform.position);
        // health.AddComponent<SphereCollider>();
        strength.SetActive(false);
        strength.name = "Stamina Potion";
        strength.tag = "Stamina Potion";
        
        strength.GetComponent<SphereCollider>().isTrigger = true;
        TriggerEventRouter tc = strength.AddComponent<TriggerEventRouter>();
        tc.callback = callback;
        powerUps.AddLast(strength);

    }

    private void SpawnHealth(int column, int newRow, GameObject end=null, TriggerEventHandler callback=null)
    {
        // Debug.Log("Made it to SpawnHealth");
        GameObject health = Instantiate(healthPotion);
        health.transform.position = new Vector3(column * hallWidth, -.5f, newRow * hallWidth);
        float playerDistance = Vector3.Distance(player.transform.position, health.transform.position);
        float portalDistance = Vector3.Distance(portal.transform.position, health.transform.position);
        Debug.Log("health dist: " + playerDistance);
        if (playerDistance < 3  || portalDistance < 3)
        {
            Debug.Log("No spawning health, too close to player");
            Destroy(health);
            return;
        }
        // health.AddComponent<SphereCollider>();
        health.SetActive(false);
        health.name = "Health Potion";
        health.tag = "Health Potion";
        
        health.GetComponent<SphereCollider>().isTrigger = true;
        TriggerEventRouter tc = health.AddComponent<TriggerEventRouter>();
        tc.callback = callback;
        powerUps.AddLast(health);
    }
    
    private void SpawnShield(int column, int newRow, GameObject end=null, TriggerEventHandler callback=null)
    {
        // Debug.Log("Made it to SpawnShield");
        GameObject shield = Instantiate(shieldPotion);
        shield.transform.position = new Vector3(column * hallWidth, -.5f, newRow * hallWidth);
        float playerDistance = Vector3.Distance(player.transform.position, shield.transform.position);
        float portalDistance = Vector3.Distance(portal.transform.position, shield.transform.position);
        Debug.Log("shield dist: " + playerDistance);
        if (playerDistance < 3  || portalDistance < 3)
        {
            Debug.Log("No spawning shield, too close to player");
            Destroy(shield);
            return;
        }
        shield.SetActive(false);
        shield.name = "Overshield Potion";
        shield.tag = "Overshield Potion";
        
        shield.GetComponent<SphereCollider>().isTrigger = true;

        TriggerEventRouter tc = shield.AddComponent<TriggerEventRouter>();
        tc.callback = callback;
        powerUps.AddLast(shield);
    }
    
    private void SpawnArrow(int column, int newRow, GameObject end=null, TriggerEventHandler callback=null)
    {
        // Debug.Log("Made it to SpawnShield");
        GameObject arrow = Instantiate(Arrows);
        arrow.transform.position = new Vector3(column * hallWidth, -0.0f, newRow * hallWidth);
        float playerDistance = Vector3.Distance(player.transform.position, arrow.transform.position);
        float portalDistance = Vector3.Distance(portal.transform.position, arrow.transform.position);
        Debug.Log("arrow dist: " + playerDistance);
        if (playerDistance < 3  || portalDistance < 3)
        {
            Debug.Log("No spawning arrow, too close to player");
            Destroy(arrow);
            return;
        }
        arrow.SetActive(false);
        arrow.name = "Arrow";
        arrow.tag = "Arrow";

        TriggerEventRouter tc = arrow.AddComponent<TriggerEventRouter>();
        tc.callback = callback;
        arrowList.AddLast(arrow);

    }

    private void PlaceStartTrigger(TriggerEventHandler callback)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.position = new Vector3(startCol * hallWidth, .5f, startRow * hallWidth);
        go.name = "Start Trigger";
        go.tag = "Generated";

        go.GetComponent<BoxCollider>().isTrigger = true;
        go.GetComponent<MeshRenderer>().sharedMaterial = startMat;

        TriggerEventRouter tc = go.AddComponent<TriggerEventRouter>();
        tc.callback = callback;
    }


    public IEnumerator spawnEnemyAutzen()
    {

        while (true)
        {
            for (int i = 0; i < 6; i++)
            {
                GameObject sk;
                System.Random random = new System.Random();
                int temp = random.Next(1, 3);
                int x = random.Next(-370, -340);
                int y = random.Next(30, 40);
                if (temp == 1)
                {
                    sk = Instantiate(mage);
                    sk.name = "Mage";
                    sk.tag = "Mage";
                }
                else
                {
                    sk = Instantiate(skeleton);
                    sk.name = "Skeleton";
                    sk.tag = "Enemy";
                }
        
                // Debug.Log("X: " + x + " y: " + y);
                sk.transform.position = new Vector3(x, .1f, y);
                float distance = Vector3.Distance(player.transform.position, sk.transform.position);
                // Debug.Log("autzen distance " + distance);
            
                sk.SetActive(false);
                enemyList.AddLast(sk);
            }
            
            yield return new WaitForSeconds(20);
        }
    }
    private void PlaceEnemy(int column, int newRow, TriggerEventHandler callback)
    {
        // GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);

        System.Random random = new System.Random();
        int temp = random.Next(1, 8);
        GameObject sk;
        Debug.Log("temp " + temp);
        if (temp >= 5f && loadTutorial == 0) 
        {
            sk = Instantiate(mage) as GameObject;
            sk.name = "Mage";
            sk.tag = "Mage";
        }
        else
        {
            sk = Instantiate(skeleton) as GameObject;
            sk.name = "Skeleton";
            sk.tag = "Enemy";
        }
        Debug.Log("Random spawn is " + temp);
        // sk.AddComponent<NavMeshAgent>();
        sk.transform.position = new Vector3(column * hallWidth, .1f, newRow * hallWidth);
        float distance = Math.Abs(sk.transform.position.x - player.transform.position.x);
        Debug.Log("distance: " + distance);
        if (distance < spawnDistance)
        {
            Debug.Log("Enemy too close, not spawning at location " + sk.transform.position);
            Destroy(sk);
            return;
        }
        
        sk.AddComponent<MeshCollider>();
        // sk.enabled = true;
        sk.SetActive(true);
        
        MeshCollider t = sk.GetComponent<MeshCollider>();
        // t.material = mr.materials[0];
        
        // Instantiate(skeleton);

        enemyList.AddLast(sk);
        
        // sk.GetComponent<BoxCollider>().isTrigger = true;
        // skeleton.GetComponent<MeshRenderer>().sharedMaterial = treasureMat;

        // TriggerEventRouter tc = skeleton.AddComponent<TriggerEventRouter>();
        // tc.callback = callback;

    }
    
    private GameObject PlaceEndTrigger(int column, int newRow, TriggerEventHandler callback)
    {
        // Debug.Log("End trigger: " + column + " " + newRow);
        Vector3 gatePos = new Vector3(column * hallWidth + hallWidth / 2, .5f, newRow * hallWidth);
        // Vector3 gatePos = new Vector3(hallWidth + hallWidth / 2, .5f, hallWidth);

        Instantiate(gate, gatePos, Quaternion.Euler(-90, 135, 0));
        gate.tag = "Portal";
        gate.name = "MazePortal";

        //go.GetComponent<BoxCollider>().isTrigger = true;
        //go.GetComponent<MeshRenderer>().sharedMaterial = endGoal;

        //TriggerEventRouter tc = go.AddComponent<TriggerEventRouter>();
        //tc.callback = callback;

        return gate;
    }

    // top-down debug display
    void OnGUI()
    {
        if (!showDebug)
        {
            return;
        }

        int[,] maze = data;
        int rMax = maze.GetUpperBound(0);
        int cMax = maze.GetUpperBound(1);

        string msg = "";

        // loop top to bottom, left to right
        for (int i = rMax; i >= 0; i--)
        {
            for (int j = 0; j <= cMax; j++)
            {
                if (maze[i, j] == 0)
                {
                    msg += "0";
                }
                else
                {
                    msg += "+";
                }

                msg += " ";
            }
            msg += "\n";
        }

        GUI.Label(new Rect(20, 20, 500, 500), msg);
    }
}
