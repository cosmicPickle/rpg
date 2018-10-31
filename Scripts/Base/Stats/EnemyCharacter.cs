using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCharacter : Character {
    protected override void Die()
    {
        base.Die();
        Destroy(gameObject);
    }
}
