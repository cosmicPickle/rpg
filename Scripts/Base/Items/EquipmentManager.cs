using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour {

    #region Singleton
    public static EquipmentManager instance;
    void Awake()
    {
        if (instance != null)
        {
            if (instance != this)
                Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }
    #endregion

    Equipment[] currentEquipment;

    public delegate void OnEquipmentChanged(Equipment newEquipment, Equipment oldEquipment);
    public static OnEquipmentChanged onEquipmentChanged;

    void Start()
    {
        int numSlots = System.Enum.GetNames(typeof(Equipment.Slot)).Length;
        currentEquipment = new Equipment[numSlots];
    }

    public Equipment[] GetCurrentEquipment()
    {
        return currentEquipment;
    }

    public void Equip(Equipment newEquipment)
    {
        Equipment oldEquipment = null;

        int slotIndex = (int)newEquipment.equipSlot;

        if(currentEquipment[slotIndex] != null)
        {
            oldEquipment = currentEquipment[slotIndex];
            currentEquipment[slotIndex] = newEquipment;

            Inventory.instance.Add(oldEquipment);
        }

        currentEquipment[slotIndex] = newEquipment;
        Inventory.instance.Remove(newEquipment);

        Notifications.instance.AddNotification("Equipped " + newEquipment.name);

        if (onEquipmentChanged != null)
        {
            onEquipmentChanged(newEquipment, oldEquipment);
        }
    }

    public void Uneqip(int slotIndex)
    {
        if(currentEquipment[slotIndex] == null)
        {
            return;
        }

        Equipment oldEquipment = currentEquipment[slotIndex];

        if (Inventory.instance.Add(oldEquipment))
        {
            currentEquipment[slotIndex] = null;
        }

        Notifications.instance.AddNotification("Unequipped " + oldEquipment.name);
        if (onEquipmentChanged != null)
        {
            onEquipmentChanged(null, oldEquipment);
        }
    }
}
