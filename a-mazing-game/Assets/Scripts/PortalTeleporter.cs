using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalTeleporter : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform player;
    public Transform receiver;
    private Vector3 AutzenLocation;
    public GameObject duck;
    public AudioSource shout;
    public int test;
    public GameObject healthBar;
    public MazeConstructor mz;
    private bool playerIsOverlapping = false;
    // Update is called once per frame

    void Awake()
    {
        mz = GameObject.Find("Controller").GetComponent<MazeConstructor>();
        healthBar = GameObject.Find("Duck Health").gameObject;
        duck = GameObject.Find("Duck").gameObject;
        shout = GameObject.Find("AutzenModel").GetComponent<AudioSource>();
        player = GameObject.Find("Player").transform;
        AutzenLocation = new Vector3(-341.5f, 0f, 32f);
        healthBar.SetActive(false);
        duck.SetActive(false);
    }
    void Update()
    {
        if (playerIsOverlapping)
        {
            int flag = PlayerPrefs.GetInt("tutorial", 0);
            if (flag == 1)
            {
                mz.GetComponent<tutorial>().endGame();
                Vector3 resetPlayer = new Vector3(0, 0, 0);
                player.position = resetPlayer;

            }
            else
            {
                duck.SetActive(true);
                healthBar.SetActive(true);
                player.position = AutzenLocation;
                player.Rotate(Vector3.up, -115f);
                shout.Play();
                playerIsOverlapping = false;
            }
        }

    }

    public void SetReceiver(Transform receive)
    {
        receiver = receive;
    }

    public void SetPlayer(Transform playerPos)
    {
        player = playerPos;
    }

    void OnTriggerEnter(Collider other)
    {
        print("touch");
        if (other.tag == "Player")
        {
            playerIsOverlapping = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            playerIsOverlapping = false;
        }
    }
}
