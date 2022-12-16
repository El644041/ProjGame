using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemClickHandler : MonoBehaviour
{
    public Inventory _Inventory;

    public KeyCode _Key;

    private Button _button;


    public FpsMovement fps;

    void Awake()
    {
        _button = GetComponent<Button>();
    }

    void Update()
    {
        if(Input.GetKeyDown(_Key))
        {
            FadeToColor(_button.colors.pressedColor);
            // Click the button
            _button.onClick.Invoke();
            OnItemClicked();
        }
        else if(Input.GetKeyUp(_Key))
        {
            FadeToColor(_button.colors.normalColor);
        }
    }

    void FadeToColor(Color color)
    {
        Graphic graphic = GetComponent<Graphic>();
        graphic.CrossFadeColor(color, _button.colors.fadeDuration, true, true);
    }

    private InventoryItemBase AttachedItem
    {
        get
        {
            ItemDragHandler dragHandler =
            gameObject.transform.Find("ItemImage").GetComponent<ItemDragHandler>();

            return dragHandler.Item;
        }
    }

    public void OnItemClicked()
    {
        InventoryItemBase item = AttachedItem;

        if (item != null)
        {
            fps.arrow.SetActive(false);
            Debug.Log("using " + item.Name);
            _Inventory.UseItem(item);
            
        }
        else
        {
            if (fps.mCurrentItem != null)
            {
                fps.arrow.SetActive(false);
                fps.SetItemActive(fps.mCurrentItem, false);
                fps.mCurrentItem = null;
            }
        }
    }

}
