using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent2D))]
[RequireComponent(typeof(Character))]
public class BotNavAgent : RaycastController, IWalker
{
    Character character;
    NavMeshAgent2D agent;

    Vector3 pausedDestination;
    bool paused;

    Character.Knockback currentKnockback;

    public override void Awake()
    {
        base.Awake();
        agent = GetComponent<NavMeshAgent2D>();
        character = GetComponent<Character>();

        character.onTakeDamage += OnTakeDamage;
    }

    private void Update()
    {
        UpdateRaycastOrigins();
        if (currentKnockback.effect > 0)
        {
            HandleKnockback();
        }
    }

    void HandleKnockback()
    {
        if(!paused)
            Pause();

        Vector2 moveAmount = currentKnockback.direction * Time.deltaTime * currentKnockback.speed;
        
        //We need to handle collisions
        if (moveAmount.x != 0)
            HandleHorizontalCollisions(ref moveAmount);
        if (moveAmount.y != 0)
            HandleVerticalCollisions(ref moveAmount);

        agent.Warp((Vector2)transform.position + moveAmount * agent.speed);

        currentKnockback.effect -= moveAmount.magnitude;

        if (currentKnockback.effect <= 0)
        {
            Resume();
        }
    }

    void HandleHorizontalCollisions(ref Vector2 moveAmount)
    {
        float rayLength = Mathf.Abs(moveAmount.x) + skinWidth;
        float directionX = Mathf.Sign(moveAmount.x);

        if (Mathf.Abs(moveAmount.x) <= skinWidth)
        {
            rayLength = 2 * skinWidth;
        }

        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 origin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            origin += Vector2.up * (horizontalRaySpacing * i);

            RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.right * directionX, rayLength, collisionMask);

            Debug.DrawRay(origin, Vector2.right * directionX * rayLength, Color.red);
            if (hit)
            {
                moveAmount.x = (hit.distance - skinWidth) * directionX;
            }
        }
    }

    void HandleVerticalCollisions(ref Vector2 moveAmount)
    {
        float rayLength = Mathf.Abs(moveAmount.y) + skinWidth;
        float directionY = Mathf.Sign(moveAmount.y);

        if (Mathf.Abs(moveAmount.y) < skinWidth)
        {
            rayLength = 2 * skinWidth;
        }

        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 origin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            origin += Vector2.right * (verticalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.up * directionY, rayLength, collisionMask);

            Debug.DrawRay(origin, Vector2.up * directionY * rayLength, Color.red);
            if (hit)
            {
                moveAmount.y = (hit.distance - skinWidth) * directionY;
            }
        }
    }

    void OnTakeDamage(float finalDamage, Character.Knockback knockback, AttackAgent attackAgent)
    {
        currentKnockback = knockback;
    }

    public Vector3 GetVelocity()
    {
        return agent.velocity;
    }

    public void Pause()
    {
        pausedDestination = agent.destination;
        paused = true;

        Stop();
    }

    public void Resume()
    {
        paused = false;
        agent.SetDestination(pausedDestination);
    }

    public void Stop()
    {
        if (agent == null)
            return;

        agent.SetDestination(transform.position);
    }

    public void SetDestination(Vector3 destination, float stoppingDistance = 0f)
    {
        if (currentKnockback.effect > 0)
        {
            return;
        }

        agent.stoppingDistance = stoppingDistance;

        agent.speed = character.defenceStats.speed.GetValue();

        if (paused)
        {
            pausedDestination = destination;
        } else
        {
            agent.SetDestination(destination);
        }
    }

    public LayerMask GetObstacleMask()
    {
        return collisionMask;
    }
}
