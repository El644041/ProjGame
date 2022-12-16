using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EItemType
{
    Default,
    Consumable,
    Weapon,
}

public class InteractableItemBase : MonoBehaviour
{
    public string Name;

    public Sprite Image;

    public string InteractText = "Press F to pickup";

    public EItemType ItemType;
    public tutorial tutorialScript;

    
    public virtual void OnInteractAnimation(Animator animator)
    {
        animator.SetTrigger("tr_pickup");
    }

    public virtual void OnInteract()
    {
    }

    public virtual bool CanInteract(Collider other)
    {
        return true;   
    }
}

public class InventoryItemBase : InteractableItemBase
{
    public MazeConstructor mz;
    public bool pickedUp;
    public bool isDropped;

    public InventorySlot Slot
    {
        get; set;
    }

    public virtual void OnUse()
    {
        tutorialScript.GetComponent<tutorial>().onWeaponPickUp();
        transform.localPosition = PickPosition;
        transform.localEulerAngles = PickRotation;
    }

    public virtual void OnDrop()
    {
        isDropped = true;
        gameObject.SetActive(true);
        gameObject.transform.rotation = Quaternion.Euler(DropRotation);
    }

    public virtual void OnPickup()
    {
        isDropped = false;
        Destroy(gameObject.GetComponent<Rigidbody>());
        Destroy(gameObject.GetComponent<BoxCollider>());
        gameObject.SetActive(false);
    }

    public Vector3 PickPosition;

    public Vector3 PickRotation;

    public Vector3 DropRotation;

    public bool UseItemAfterPickup = false;


}
