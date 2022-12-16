using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class tutorial : MonoBehaviour
{

    public MazeConstructor mz;

    public bool weaponPickup;
    public bool powerUpPickup;
    public bool encounteredCombat;
    public bool enemyDeath;
    public GameObject tutorialScreen;
    public Text tutorialText;
    private int startTut;
    float sliderSensitivity;
    bool finishedTutorial;
    public bool miniMapTut;
    public bool portalTut;
    public Camera playerCam;

    public Button MenuButton;

    public Button ContinueButton;
    AsyncOperation asyncLoadLevel;

    void Start()
    {
        //pauseAudio(true);
        weaponPickup = false;
        powerUpPickup = false;
        encounteredCombat = false;
        enemyDeath = false;
        portalTut = false;
        miniMapTut = false;
        startTut = mz.GetComponent<MazeConstructor>().loadTutorial;
        if(startTut == 1)
            Cursor.lockState = CursorLockMode.None;
        Debug.Log("start tut: " + startTut);
        finishedTutorial = false;
        MenuButton.gameObject.SetActive(false);
    }

    public void pauseAudio(bool state)
    {
        //playerCam.GetComponent<AudioListener>().pause = state;
        AudioListener.pause = state;
    }
    public void tutorialStartMessage(int load=0)
    {
        /*
         * Introduce the game and the main goal (to make it to the boss portal in the opposite corner).
         * Prompt player to look down and pick up their weapon of choice.
         */

        Debug.Log("why: " + startTut);
        if(load == 0)
            return;
        
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0;
        tutorialText.text =
            "Welcome to the A-Maze-ing Game Tutorial Level!\n \nYour MAIN GOAL will be to make it to the " +
            "BOSS PORTAL in the OPPOSITE CORNER of the MAZE (the maze is a square). When you reach this portal in the " +
            "tutorial level, the tutorial will conclude. \n \nYou can PAUSE the game at any time by pressing P.\n \n" +
            "When you spawn in, LOOK DOWN and press F to PICK UP your weapon of choice.";
        tutorialScreen.SetActive(true);

        Debug.Log("tutorialStartMessage");
    }

    public void continueTutorial()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        tutorialScreen.SetActive(false);
        //pauseAudio(false);

    }

    public void onWeaponPickUp()
    {
        /*
         * Once a weapon is picked up: Walk through the basic controls (WASD movement, running, placing torches,
         * and pausing the game)
         */

        if(startTut == 0)
            return;

        //pauseAudio(true);

        if (!weaponPickup)
        {
            Debug.Log("onWeaponPickUp");
            weaponPickup = true;
            
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0;
            tutorialText.text =
                "You have picked up your first weapon!\n \nNow, go pick up those red and blue potions in front of you.\n" +
                "You can MOVE with the WASD keys and SPRINT by pressing SHIFT while walking.\nNOTE: sprinting uses your " +
                "available stamina (the yellow stat bar)!";
            tutorialScreen.SetActive(true);
            //pauseAudio(true); 
        }
       
    }

    public void onPowerUpPickUp()  // make sure to tell player to pick up blue potion
    {
        /*
         * Direct player to potions down main hallway (once picked up):
         * Talk about each function and how to use them (1-9 keys)
         */

        if(startTut == 0)
            return;

        while (!powerUpPickup)
        {
            Debug.Log("onPowerUpPickUp");
            powerUpPickup = true;
            
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0;
            tutorialText.text =
                "There are two main potions spawned within the maze:\nRED POTIONS give you HEALTH and " +
                "BLUE POTIONS give you OVERSHIELD.\nYour overshield will be consumed before player health." +
                "\n \nOnce you pick up a spawned item, that item is placed in your inventory.\nYou can use " +
                "your NUMBER KEYS (1-9) on the keyboard to USE that specific ITEM or to SWAP between WEAPONS. " +
                "\n \nPotions & arrows are spawned in dead end hallways within the maze. " +
                "\nArrows DO NOT glow very bright, so pay close attention when you're on the hunt for ammo!";

            tutorialScreen.SetActive(true);
            //pauseAudio(true);
        }
        if (powerUpPickup)
        {
            StartCoroutine(miniMap());
        }
    }

    public IEnumerator miniMap()
    {
        /*
         * Either immediately after, or when the player walks a bit further:
         * Introduce the minimap and what to look for. Point out a red dot, and prompt the player to walk over to it.
         */


        yield return new WaitForSeconds(2);
        Debug.Log("miniMap");

        if (!miniMapTut)
        {
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0;
            tutorialText.text = "In the TOP LEFT of the SCREEN is your MINIMAP.\nYOU are the GREEN DOT in the center " +
                                "of the map.\nENEMIES appear on the minimap as RED DOTS and POWER-UPS appear as SMALL " +
                                "DOTS on the map with their associated colors.\n \nThere are enemies on your minimap now, " +
                                "go and try to fight them!";
                
            tutorialScreen.SetActive(true);
            //pauseAudio(true);
            StartCoroutine(mz.GetComponent<MazeConstructor>().SpawnCoRoutine());
            miniMapTut = true;
        }
        
        StopCoroutine(miniMap());
    }

    public void combat()
    {
        /*
         * Once enemy is in fighting range: Pause and introduce combat controls (left click to attack, right click
         * to roll, run and left click to heavy attack)
         */

        if(startTut == 0)
            return;

        if (!encounteredCombat)
        {
            Debug.Log("combat");
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0;
            tutorialText.text = "An enemy has SPOTTED you!\n \nMelee weapons have two attack modes: QUICK and " +
                                "HEAVY attacks.\nYou can press the LEFT MOUSE BUTTON to perform a QUICK ATTACK.\n" +
                                "Alternatively, you can ATTACK WHILE SPRINTING to perform a HEAVY ATTACK.\n \nYou can also " +
                                "DODGE enemies by pressing the RIGHT MOUSE BUTTON.\n \nThe BOW is a ranged weapon that needs " +
                                "ARROWS to use (it also DOES NOT have a heavy attack). I bet you'll find some if you look around!";
                
            tutorialScreen.SetActive(true);
            //pauseAudio(true);
            encounteredCombat = true;
        }
    }

    public void onEnemyDeath()
    {
        /*
         * Once player kills first skeleton: Point to the coin counter after the player picked up the coins
         */

        if(startTut == 0)
            return;

        if (!enemyDeath)
        {
            Debug.Log("onEnemyDeath");
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0;
            tutorialText.text = "Nice job!\n \nEnemies will drop COINS when you KILL them!\nCoins can be used" +
                                " to purchase PLAYER UPGRADES like increasing your max health.\n \n" +
                                "ENEMIES are CONTINUOUSLY spawned within the maze, and in GREATER NUMBERS.\n" +
                                "So, while it's a good idea to EXPLORE the maze and STOCK UP on potions & arrows, just " +
                                "BE AWARE that if you stay in the maze too long you could be OVERRUN!\n \nPRO TIP: You can " +
                                "PLACE TORCHES on the WALLS of the maze by pressing T. This will help to keep track of " +
                                "where you have already been.";
                
            tutorialScreen.SetActive(true);
            //pauseAudio(true);
            enemyDeath = true;
        }

        StartCoroutine(portal());
    }

    public IEnumerator portal()
    {
        /*
         * Immediately after: Prompt the player to make it to the portal to finish the tutorial
         */
        
        yield return new WaitForSeconds(10);
        Debug.Log("portal");

        if (!portalTut)
        {
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0;
            tutorialText.text = "When you're ready to leave the maze, you can walk through the portal.\n \nIn a normal game mode, " +
                                "once you ENTER the portal, you will be TELEPORTED to the BOSS FIGHT. But, for this tutorial " +
                                "the session will end.\n \nHave fun!";
            portalTut = true;
                
            tutorialScreen.SetActive(true);
            //pauseAudio(true);
        }
        
        StopCoroutine(portal());
    }

    public void endGame()
    {
        /*
         * Once player reaches portal: End the tutorial, tell player to try easy or medium maze for practice,
         * and hard for a good challenge.
         */
        if(startTut == 0)
            return;

        
        Cursor.lockState = CursorLockMode.None;
        ContinueButton.gameObject.SetActive(false);
        MenuButton.gameObject.SetActive(true);
        pauseAudio(true);
        Time.timeScale = 0;
        tutorialText.text = "Congratulations! You've reached the end of the tutorial!\n \nPlay the easy or medium maze for some practice," +
            " or the hard mode maze for a challenge! :)";
        tutorialScreen.SetActive(true);
    }

    public void backToMenu()
    {
        StartCoroutine(loadMenu());
        sliderSensitivity = PlayerPrefs.GetFloat("sensitivity", 4f);
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetFloat("sensitivity", sliderSensitivity);
        Time.timeScale = 1;
        tutorialScreen.SetActive(false);

    }

    IEnumerator loadMenu()
    {
        yield return SceneManager.LoadSceneAsync("Menu");

    }

}
