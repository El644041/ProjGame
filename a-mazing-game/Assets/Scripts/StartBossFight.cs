using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartBossFight : MonoBehaviour
{
    public bool startFight;
    public MazeConstructor mz;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            mz = GameObject.Find("Controller").GetComponent<MazeConstructor>();
            StartCoroutine(mz.GetComponent<MazeConstructor>().spawnEnemyAutzen());
            startFight = true;
            gameObject.SetActive(false);
        }
    }
}
