using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private const int SLOTS = 9;

    public IList<InventorySlot> mSlots = new List<InventorySlot>();

    public event EventHandler<InventoryEventArgs> ItemAdded;
    public event EventHandler<InventoryEventArgs> ItemRemoved;
    public event EventHandler<InventoryEventArgs> ItemUsed;

    public bool hasArrows;

    public Inventory()
    {
        for (int i = 0; i < SLOTS; i++)
        {
            mSlots.Add(new InventorySlot(i));
        }
    }

    private InventorySlot FindStackableSlot(InventoryItemBase item)
    {
        foreach (InventorySlot slot in mSlots)
        {
            if (slot.IsStackable(item))
                return slot;
        }
        return null;
    }

    private InventorySlot FindNextEmptySlot()
    {
        foreach (InventorySlot slot in mSlots)
        {
            if (slot.IsEmpty)
                return slot;
        }
        return null;
    }

    public void AddItem(InventoryItemBase item)
    {
        if (item.Name == "Arrows")
            hasArrows = true;
        InventorySlot freeSlot = FindStackableSlot(item);
        if (freeSlot == null)
        {
            freeSlot = FindNextEmptySlot();
        }
        if (freeSlot != null)
        {
            int addAmount = 1;
            
            if (item.Name == "Arrows")
            {
                // GameObject goItem = (item as MonoBehaviour).gameObject;
                addAmount = item.GetComponent<Arrows>().pickupAmount;
                hasArrows = true;
            }
            
            for (var i = 0; i < addAmount; i++)
                freeSlot.AddItem(item);
            
            if (ItemAdded != null)
            {
                ItemAdded(this, new InventoryEventArgs(item));
            }
        }
    }

    internal void UseItem(InventoryItemBase item)
    {
        // if (item.Name == "Bow")
        // {
        //     GameObject goItem = (item as MonoBehaviour).gameObject;
        //     goItem.GetComponent<BowAndArrow>().arrows.pickupAmount--;
        //     RemoveItem(goItem.GetComponent<BowAndArrow>().arrows);
        // }
        if (ItemUsed != null)
        {
            ItemUsed(this, new InventoryEventArgs(item));
        }

        item.OnUse();
    }

    public void RemoveItem(InventoryItemBase item)
    {
        foreach (InventorySlot slot in mSlots)
        {
            if (item.Name == "Arrows")
            {
                if (item.isDropped)
                {
                    var slotInfo = slot.RemoveAll(item);
                    if (slotInfo.Item1)
                    {
                        // GameObject goItem = (item as MonoBehaviour).gameObject;
                        item.GetComponent<Arrows>().pickupAmount = slotInfo.Item2;
                        hasArrows = false;
                        if (ItemRemoved != null)
                        {
                            ItemRemoved(this, new InventoryEventArgs(item));
                        }
                        break;
                    }
                }
                else
                {
                    if (slot.Remove(item))
                    {
                        if (slot.IsEmpty)
                            hasArrows = false;
                        if (ItemRemoved != null)
                        {
                            ItemRemoved(this, new InventoryEventArgs(item));
                        }
                        break;
                    }
                }
            }
            else
            {
                if (slot.Remove(item))
                {
                    if (ItemRemoved != null)
                    {
                        ItemRemoved(this, new InventoryEventArgs(item));
                    }
                    break;
                }
            }
        }
    }
}
