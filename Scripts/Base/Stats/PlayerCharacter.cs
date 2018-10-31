using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : Character {

    protected override void Awake()
    {
        base.Awake();
        EquipmentManager.onEquipmentChanged += OnEquipmentChanged;
    }

    void OnEquipmentChanged(Equipment newItem, Equipment oldItem)
    {
        if (oldItem != null)
        {
            attackStats.RemoveStatBulk(oldItem.attackStats);
            defenceStats.RemoveStatBulk(oldItem.defenceStats);
        }

        if (newItem != null)
        {
            attackStats.AddStatBulk(newItem.attackStats);
            defenceStats.AddStatBulk(newItem.defenceStats);
        }
    }
}
