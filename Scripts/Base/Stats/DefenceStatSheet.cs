using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DefenceStatSheet: IOnTooltipShow
{

    public Stat maxHealth;
    public Stat defence;
    public Stat speed;
    public Stat knockbackReduction;

    public Stat fireDefence;
    public Stat iceDefence;
    public Stat shockDefence;
    public Stat poisonDefence;

    protected Dictionary<AttackStatSheet.AtackStatNames, Stat> attackToDefenceMap;

    public void InitDefenceMap()
    {
        attackToDefenceMap = new Dictionary<AttackStatSheet.AtackStatNames, Stat>
        {
            { AttackStatSheet.AtackStatNames.Attack, defence },
            { AttackStatSheet.AtackStatNames.FireAttack, fireDefence },
            { AttackStatSheet.AtackStatNames.IceAttack, iceDefence },
            { AttackStatSheet.AtackStatNames.PoisonAttack, poisonDefence },
            { AttackStatSheet.AtackStatNames.ShockAttack, shockDefence }
        };
    }

    public void AddStatBulk(DefenceStatSheet sheet)
    {
        if(sheet.maxHealth.GetValue() != 0)
            maxHealth.AddModifier(sheet.maxHealth.GetValue());
        if (sheet.defence.GetValue() != 0)
            defence.AddModifier(sheet.defence.GetValue());
        if (sheet.fireDefence.GetValue() != 0)
            fireDefence.AddModifier(sheet.fireDefence.GetValue());
        if (sheet.iceDefence.GetValue() != 0)
            iceDefence.AddModifier(sheet.iceDefence.GetValue());
        if (sheet.shockDefence.GetValue() != 0)
            shockDefence.AddModifier(sheet.shockDefence.GetValue());
        if (sheet.poisonDefence.GetValue() != 0)
            poisonDefence.AddModifier(sheet.poisonDefence.GetValue());
        if (sheet.speed.GetValue() != 0)
            speed.AddModifier(sheet.speed.GetValue());
    }

    public void RemoveStatBulk(DefenceStatSheet sheet)
    {
        if (sheet.maxHealth.GetValue() != 0)
            maxHealth.RemoveModifier(sheet.maxHealth.GetValue());
        if (sheet.defence.GetValue() != 0)
            defence.RemoveModifier(sheet.defence.GetValue());
        if (sheet.fireDefence.GetValue() != 0)
            fireDefence.RemoveModifier(sheet.fireDefence.GetValue());
        if (sheet.iceDefence.GetValue() != 0)
            iceDefence.RemoveModifier(sheet.iceDefence.GetValue());
        if (sheet.shockDefence.GetValue() != 0)
            shockDefence.RemoveModifier(sheet.shockDefence.GetValue());
        if (sheet.poisonDefence.GetValue() != 0)
            poisonDefence.RemoveModifier(sheet.poisonDefence.GetValue());
        if (sheet.speed.GetValue() != 0)
            speed.RemoveModifier(sheet.speed.GetValue());
    }

    public float CalculateDamage(List<AttackStatSheet.Attack> attacks)
    {
        float finalDamage = 0;

        attacks.ForEach(attack =>
        {
            finalDamage += (attack.value - attackToDefenceMap[attack.name].GetValue());
        });

        return finalDamage;
    }

    public string OnTooltipShow()
    {
        string final = "<size=\"10\">";

        if (maxHealth.GetValue() > 0)
            final += "max health: " + maxHealth.GetValue().ToString() + "\n";
        if (defence.GetValue() > 0)
            final += "defence: " + defence.GetValue().ToString() + "\n";
        if (speed.GetValue() > 0)
            final += "speed: " + speed.GetValue().ToString() + "\n";
        if (fireDefence.GetValue() > 0)
            final += "fire defence: " + fireDefence.GetValue().ToString() + "\n";
        if (iceDefence.GetValue() > 0)
            final += "ice defence: " + iceDefence.GetValue().ToString() + "\n";
        if (shockDefence.GetValue() > 0)
            final += "poison defence: " + shockDefence.GetValue().ToString() + "\n";
        if (poisonDefence.GetValue() > 0)
            final += "shock defence: " + poisonDefence.GetValue().ToString() + "\n";
        
        final += "</size>";
        return final;
    }

    public enum DefenceStatNames
    {
        MaxHealth,
        Defence,
        FireDefence,
        IceDefence,
        ShockDefence,
        PoisonDefence
    }

}
