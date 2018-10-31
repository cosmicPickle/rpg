using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    protected float currentHealth;
    public AttackStatSheet attackStats;
    public DefenceStatSheet defenceStats;

    public delegate void OnTakeDamage(float finalDamage, Knockback knockbackEffect, AttackAgent attackAgent);
    public delegate void OnDeath(AttackAgent attackAgent);

    public OnTakeDamage onTakeDamage;
    public OnDeath onDeath;

    public const float attackerMemoryDuration = 2f;
    [HideInInspector]
    public AttackAgent lastAttacker;
    float attackerMemoryDurationLeft = 0f;

    protected float invulnerableDurationLeft = 0;

    protected virtual void Awake()
    {
        currentHealth = defenceStats.maxHealth.GetValue();
        defenceStats.InitDefenceMap();
    }

    void Update()
    {
        if(invulnerableDurationLeft > 0)
        {
            invulnerableDurationLeft -= Time.deltaTime;
        }

        if(attackerMemoryDurationLeft > 0)
        {
            attackerMemoryDurationLeft -= Time.deltaTime;
        } else
        {
            attackerMemoryDurationLeft = 0;
            lastAttacker = null;
        }
    }

    public void TakeDamage(List<AttackStatSheet.Attack> attacks, AttackAgent attackAgent)
    {
        if (invulnerableDurationLeft > 0)
            return;

        lastAttacker = attackAgent;
        attackerMemoryDurationLeft = attackerMemoryDuration;

        float finalDamage = defenceStats.CalculateDamage(attacks);
        currentHealth -= finalDamage;
        invulnerableDurationLeft = defenceStats.invulnerableDuration.GetValue();

        Knockback knockback = new Knockback
        {
            direction = (transform.position - attackAgent.transform.position).normalized,
            effect = attackAgent.knockback.GetValue() - defenceStats.knockbackReduction.GetValue(),
            speed = attackAgent.knockbackSpeed.GetValue()
        };
        
        if (onTakeDamage != null)
        {
            onTakeDamage(finalDamage, knockback, attackAgent);
        }

        if(currentHealth <= 0)
        {
            currentHealth = 0;
            Die();

            if (onDeath != null)
            {
                onDeath(attackAgent);
            }
        }

        Debug.Log(gameObject.tag + " taking damage: " + finalDamage + " Current Health: " + currentHealth);
    }

    protected virtual void Die()
    {

    }

    public struct Knockback
    {
        public Vector2 direction;
        public float effect;
        public float speed;
    }
}
