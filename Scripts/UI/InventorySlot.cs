using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public Image icon;
    public Button removeButton;

    Item item;


    public void AddItem(Item newItem)
    {
        item = newItem;

        icon.sprite = item.icon;
        icon.enabled = true;
        removeButton.interactable = true;
    }


    public void ClearSlot()
    {
        item = null;

        if (icon)
        {
            icon.sprite = null;
            icon.enabled = false;
            removeButton.interactable = false;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(item != null)
        {
            TooltipUI.instance.ShowTooltip(transform.position, item);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipUI.instance.HideTooltip();
    }

    public void OnRemoveButton()
    {
        Inventory.instance.Remove(item);
    }

    public void UseItem()
    {
        if(item != null)
        {
            item.Use();
        }
    }
}
