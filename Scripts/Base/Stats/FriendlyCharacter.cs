using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendlyCharacter : Character {

    public bool killable = false;

    protected override void Die()
    {
        base.Die();

        if(killable)
        {
            Destroy(gameObject);
        }
    }
}
