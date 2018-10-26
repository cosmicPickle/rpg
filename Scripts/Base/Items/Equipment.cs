using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "Inventory/Equipment", order = 1)]
public class Equipment : Item {

    public Slot equipSlot;
    public AttackStatSheet attackStats = new AttackStatSheet();
    public DefenceStatSheet defenceStats = new DefenceStatSheet();

    public enum Slot
    {
        Weapon,
        Head,
        Body,
        Hands,
        Feet
    }

    public override void Use()
    {
        base.Use();
        EquipmentManager.instance.Equip(this);
    }

    public override string OnTooltipShow()
    {
        return base.OnTooltipShow() + "\n\n" + attackStats.OnTooltipShow() + defenceStats.OnTooltipShow();
    }
}
