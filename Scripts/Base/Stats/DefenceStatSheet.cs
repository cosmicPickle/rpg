using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DefenceStatSheet: IOnTooltipShow
{
    public const float maxDamageReduction = 0.8f;

    public Stat maxHealth;
    public Stat defence;
    public Stat speed;
    public Stat knockbackReduction;
    public Stat invulnerableDuration;

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
        if (sheet.knockbackReduction.GetValue() != 0)
            knockbackReduction.AddModifier(sheet.knockbackReduction.GetValue());
        if (sheet.invulnerableDuration.GetValue() != 0)
            invulnerableDuration.AddModifier(sheet.invulnerableDuration.GetValue());
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
        if (sheet.knockbackReduction.GetValue() != 0)
            knockbackReduction.RemoveModifier(sheet.knockbackReduction.GetValue());
        if (sheet.invulnerableDuration.GetValue() != 0)
            invulnerableDuration.RemoveModifier(sheet.invulnerableDuration.GetValue());
    }

    public float CalculateDamage(List<AttackStatSheet.Attack> attacks)
    {
        float finalDamage = 0;

        attacks.ForEach(attack =>
        {
            float defenceValue = attackToDefenceMap[attack.name].GetValue();
            finalDamage += (attack.value - Mathf.Clamp(defenceValue/100, 0, maxDamageReduction) * attack.value);
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
        if (shockDefence.GetValue() > 0)
            final += "shock defence: " + poisonDefence.GetValue().ToString() + "\n";
        if (knockbackReduction.GetValue() > 0)
            final += "knockback reduction: " + knockbackReduction.GetValue().ToString() + "\n";
        if (invulnerableDuration.GetValue() > 0)
            final += "invulnerable duration: " + invulnerableDuration.GetValue().ToString() + "\n";

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
