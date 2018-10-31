using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAgentManager : MonoBehaviour {

    public AttackAgentPreset defaultPreset;

    AttackAgentPreset preset;
    AttackAgent currentAttackAgent = null;
    Type attackAgentType = null;
    bool presetChanged = false;

    Dictionary<AttackAgentPreset.Type, Type> typeMap = new Dictionary<AttackAgentPreset.Type, Type>
    {
        { AttackAgentPreset.Type.Melee, typeof(MeleeAttackAgent) }
    };

    public AttackAgent GetAttackAgent()
    {
        return currentAttackAgent;
    }

    void LateUpdate()
    {
        if(presetChanged && attackAgentType != null)
        {
            currentAttackAgent = gameObject.AddComponent(attackAgentType) as AttackAgent;
            preset.Configure(currentAttackAgent);
            presetChanged = false;
            attackAgentType = null;
        } 
    }

    public void Instantiate()
    {
        if(preset == null)
        {
            preset = defaultPreset;
        }

        typeMap.TryGetValue(preset.GetAttackAgentType(), out attackAgentType);

        if(attackAgentType != null)
        {
            currentAttackAgent = null;
            Destroy(GetComponent<AttackAgent>());
            presetChanged = true;
        }
    }

    public void ChangePreset(AttackAgentPreset p)
    {
        preset = p;
        Instantiate();
    }

    public void Reset()
    {
        preset = defaultPreset;
        Instantiate();
    }

}
