using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipmentSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public Equipment.Slot slot;
    public Image icon;
    public Button unequipButton;

    Equipment item;

    public void EquipItem(Equipment newItem)
    {
        if(newItem.equipSlot != slot)
        {
            return;
        }

        item = newItem;

        icon.sprite = item.icon;
        icon.enabled = true;
        unequipButton.interactable = true;
    }


    public void UnequipItem()
    {
        item = null;

        if (icon)
        {
            icon.sprite = null;
            icon.enabled = false;
            unequipButton.interactable = false;
        }
    }

    public void OnUnequipButton()
    {
        EquipmentManager.instance.Uneqip((int)item.equipSlot);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item != null)
        {
            TooltipUI.instance.ShowTooltip(transform.position, item);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipUI.instance.HideTooltip();
    }
}
