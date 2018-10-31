using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Melee Attack Agent", menuName = "Attack Agents/Melee", order = 1)]
public class MeleeAttackAgentPreset : AttackAgentPreset {

    public MeleeAttackAgent.AttackDirection attackDirection;

    public override void Configure(AttackAgent attackAgent)
    {
        base.Configure(attackAgent);
        (attackAgent as MeleeAttackAgent).attackDirection = attackDirection;
    }

    public override Type GetAttackAgentType()
    {
        return Type.Melee;
    }
}
