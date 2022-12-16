using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    public Transform player;
    
    void LateUpdate()
    {
        Vector3 newPosition = player.position;
        Vector3 playerRotation = player.eulerAngles;
        transform.eulerAngles = new Vector3(90f,0f,-playerRotation.y);
        newPosition.y = transform.position.y;
        transform.position = newPosition;
    }
}
