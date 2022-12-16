using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    private Vector3 throwDir;
    
    public void Setup(Vector3 throwDir)
    {
        this.throwDir = throwDir;
        Transform player = GameObject.Find("Player").transform;
        transform.LookAt(player);
        // transform.Rotate(Vector3.up, 90);
        Destroy(gameObject, 5f);
    }

    // Update is called once per frame
    private void Update()
    {
        float throwSpeed = 10f;
        transform.position += throwDir * throwSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetType() == typeof(BoxCollider))
        {
            PlayerCombat target = other.GetComponent<PlayerCombat>();
            if (target != null)
            {
                Debug.Log("hit at " + Time.time);
                target.TakePlayerDamage(20);
                Destroy(gameObject);
            }
        }
        else if (other.GetType() == typeof(MeshCollider))
            Destroy(gameObject);
    }
}
