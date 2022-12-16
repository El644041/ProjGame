using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torch : MonoBehaviour
{
    public float range = 0.001f;
    public GameObject torch;
    public Transform fakeParent;
    private Vector3 offset;
    public MazeConstructor mz;

    void Start()
    {
        //int layerMask = 3 << 8;
        //layerMask = ~layerMask;
        offset = new Vector3 (0.0f, 1.5f, 0.0f);
    }
    void LateUpdate()
    {
        //RaycastHit hit;
        //var directionOfRay = transform.TransformDirection(Vector3.forward);
        //Debug.DrawRay(transform.position, directionOfRay * range, Color.red, 5f);
        transform.rotation = fakeParent.rotation;
        transform.position = fakeParent.position + offset;
        //transform.position.y = fakeParent.position.y + 2;
        if (Input.GetKeyDown(KeyCode.T))
        { 
            PlaceTorch();
        }
    }


    // Update is called once per frame
    public void PlaceTorch ()
    {
        Debug.Log("Pressed T");
        RaycastHit hit;
        //Debug.DrawRay(transform.position, transform.forward * range, Color.green, 5f);
        if (Physics.Raycast(transform.position, transform.forward, out hit, range))
        {

            // mz = GetComponent<MazeConstructor>();
            Debug.Log(hit.transform.name);
            //Vector3 pos = hit.transform.position;
            Vector3 placePoint = hit.point;
            placePoint.z = hit.point.z - 0.12f * transform.forward.z;
            placePoint.x = hit.point.x - 0.12f * transform.forward.x;
            
            //pos.z = pos.z  -1;
            
            GameObject t = Instantiate(torch, placePoint, transform.rotation);
            mz.torchList.AddLast(t);
            //torch.transform.LookAt(hit.point + hit.normal);

        }
        

    }
}
