using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterUI : MonoBehaviour {

    public Transform equipmentParent;
    public GameObject characterUI;

    EquipmentSlot[] equipmentSlots;

    // Use this for initialization
    void Start()
    {
        EquipmentManager.onEquipmentChanged += UpdateUI;

        int numSlots = System.Enum.GetNames(typeof(Equipment.Slot)).Length;
        equipmentSlots = new EquipmentSlot[numSlots];

        EquipmentSlot[] slots = equipmentParent.GetComponentsInChildren<EquipmentSlot>();
        Equipment[] currentEquipment = EquipmentManager.instance.GetCurrentEquipment();

        for (int i = 0; i < slots.Length; i++)
        {
            int slotIndex = (int)slots[i].slot;

            equipmentSlots[slotIndex] = slots[i];

            if(currentEquipment != null && currentEquipment[slotIndex] != null)
            {
                equipmentSlots[slotIndex].EquipItem(currentEquipment[slotIndex]);
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Character"))
        {
            Debug.Log("Toggle Character");
            characterUI.SetActive(!characterUI.activeSelf);
        }
    }

    void UpdateUI(Equipment newItem, Equipment oldItem) 
    {
        if(newItem)
        {
            equipmentSlots[(int)newItem.equipSlot].EquipItem(newItem);
        } else
        {
            if (oldItem)
            {
                equipmentSlots[(int)oldItem.equipSlot].UnequipItem();
            }
        }
    }
}
