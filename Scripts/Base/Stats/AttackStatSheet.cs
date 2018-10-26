using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AttackStatSheet: IOnTooltipShow
{

    public Stat attack;
    public Stat attackSpeed;

    public Stat fireAttack;
    public Stat iceAttack;
    public Stat shockAttack;
    public Stat poisonAttack;

    public enum AtackStatNames
    {
        Attack,
        AttackSpeed,
        FireAttack,
        IceAttack,
        ShockAttack,
        PoisonAttack
    }

    public void AddStatBulk(AttackStatSheet sheet)
    {
        if (sheet.attack.GetValue() != 0)
            attack.AddModifier(sheet.attack.GetValue());
        if (sheet.fireAttack.GetValue() != 0)
            fireAttack.AddModifier(sheet.fireAttack.GetValue());
        if (sheet.iceAttack.GetValue() != 0)
            iceAttack.AddModifier(sheet.iceAttack.GetValue());
        if (sheet.poisonAttack.GetValue() != 0)
            poisonAttack.AddModifier(sheet.poisonAttack.GetValue());
        if (sheet.shockAttack.GetValue() != 0)
            shockAttack.AddModifier(sheet.shockAttack.GetValue());
    }

    public void RemoveStatBulk(AttackStatSheet sheet)
    {
        if (sheet.attack.GetValue() != 0)
            attack.RemoveModifier(sheet.attack.GetValue());
        if (sheet.fireAttack.GetValue() != 0)
            fireAttack.RemoveModifier(sheet.fireAttack.GetValue());
        if (sheet.iceAttack.GetValue() != 0)
            iceAttack.RemoveModifier(sheet.iceAttack.GetValue());
        if (sheet.poisonAttack.GetValue() != 0)
            poisonAttack.RemoveModifier(sheet.poisonAttack.GetValue());
        if (sheet.shockAttack.GetValue() != 0)
            shockAttack.RemoveModifier(sheet.shockAttack.GetValue());
    }

    public List<Attack> GetAttacks()
    {
        List<Attack> attacks = new List<Attack>();

        if (attack.GetValue() > 0)
            attacks.Add(new Attack
            {
                name = AtackStatNames.Attack,
                value = attack.GetValue()
            });

        if (fireAttack.GetValue() > 0)
            attacks.Add(new Attack
            {
                name = AtackStatNames.FireAttack,
                value = fireAttack.GetValue()
            });

        if (iceAttack.GetValue() > 0)
            attacks.Add(new Attack
            {
                name = AtackStatNames.IceAttack,
                value = iceAttack.GetValue()
            });

        if (shockAttack.GetValue() > 0)
            attacks.Add(new Attack
            {
                name = AtackStatNames.ShockAttack,
                value = shockAttack.GetValue()
            });
        if (poisonAttack.GetValue() > 0)
            attacks.Add(new Attack
            {
                name = AtackStatNames.PoisonAttack,
                value = poisonAttack.GetValue()
            });

        return attacks;
    }

    public string OnTooltipShow()
    {
        string final = "<size=\"10\">";

        if (attack.GetValue() > 0)
            final += "attack: " + attack.GetValue().ToString() + "\n";
        if (fireAttack.GetValue() > 0)
            final += "fire attack: " + fireAttack.GetValue().ToString() + "\n";
        if (iceAttack.GetValue() > 0)
            final += "ice attack: " + iceAttack.GetValue().ToString() + "\n";
        if (poisonAttack.GetValue() > 0)
            final += "poison attack: " + poisonAttack.GetValue().ToString() + "\n";
        if (shockAttack.GetValue() > 0)
            final += "shock attack: " + shockAttack.GetValue().ToString() + "\n";

        final += "</size>";
        return final;
    }

    public struct Attack
    {
        public AtackStatNames name;
        public float value;
    }
}
