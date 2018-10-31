using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackAgentPreset : ScriptableObject {

    public float range;
    public Stat knockback;
    public Stat knockbackSpeed;

    public Stat attackRate;
    public float attackDuration;

    public abstract Type GetAttackAgentType();

    public virtual void Configure(AttackAgent attackAgent)
    {
        attackAgent.range = range;
        attackAgent.knockback = knockback;
        attackAgent.knockbackSpeed = knockbackSpeed;
        attackAgent.attackRate = attackRate;
        attackAgent.attackDuration = attackDuration;
    }

    public enum Type
    {
        None,
        Melee
    }
}
