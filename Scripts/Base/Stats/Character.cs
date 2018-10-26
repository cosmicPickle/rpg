using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    protected float currentHealth;
    public AttackStatSheet attackStats = new AttackStatSheet();
    public DefenceStatSheet defenceStats = new DefenceStatSheet();

    public delegate void OnTakeDamage(float finalDamage, AttackAgent attackAgent);
    public delegate void OnDeath(AttackAgent attackAgent);

    public OnTakeDamage onTakeDamage;
    public OnDeath onDeath;

    void Awake()
    {
        currentHealth = defenceStats.maxHealth.GetValue();
        defenceStats.InitDefenceMap();
        EquipmentManager.onEquipmentChanged += OnEquipmentChanged;
    }


    void OnEquipmentChanged(Equipment newItem, Equipment oldItem)
    {
        if (oldItem != null)
        {
            attackStats.RemoveStatBulk(oldItem.attackStats);
            defenceStats.RemoveStatBulk(oldItem.defenceStats);
        }

        if (newItem != null)
        {
            attackStats.AddStatBulk(newItem.attackStats);
            defenceStats.AddStatBulk(newItem.defenceStats);
        }
    }

    public void TakeDamage(List<AttackStatSheet.Attack> attacks, AttackAgent attackAgent)
    {
        float finalDamage = defenceStats.CalculateDamage(attacks);
        currentHealth -= finalDamage;

        //TODO: ADD KNOCKBACK
        Vector2 knockbackDirection = (transform.position - attackAgent.transform.position).normalized;
        float knockbackEffect = attackAgent.knockback.GetValue() - defenceStats.knockbackReduction.GetValue();

        
        if (onTakeDamage != null)
        {
            onTakeDamage(finalDamage, attackAgent);
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

        Debug.Log("Taking damage: " + finalDamage + " Current Health: " + currentHealth);
    }

    protected virtual void Die()
    {

    }
}
