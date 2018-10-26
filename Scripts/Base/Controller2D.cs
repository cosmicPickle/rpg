using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FaceController))]
public class Controller2D : RaycastController {

    [HideInInspector]
    public CollisionInfo collisions;
    [HideInInspector]
    public FaceController faceController;

    NavMeshAgent2D agent;
    Transform target;
    Vector2 warpVelocity;
    Vector2 lastMoveAmount;

    public override void Awake() {
        base.Awake();

        if(agent == null)
            agent = GetComponent<NavMeshAgent2D>();

        faceController = GetComponent<FaceController>();
    }

    void Update()
    {
        faceController.UpdateFaceDirection((agent.velocity != Vector2.zero ? agent.velocity : lastMoveAmount));
        if (target != null)
        {
            agent.SetDestination(target.position);
        }
    }

    public void SetSpeed(float speed)
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent2D>();

        if (speed != 0)
        {
            agent.speed = speed;
            agent.acceleration = speed * 10;
        }
    }

    public void Move(Vector2 moveAmount)
    {
        UpdateRaycastOrigins();
        collisions.Reset();

        if (moveAmount == Vector2.zero)
            return;

        if (moveAmount.x != 0)
        {
            HandleHorizontalCollisions(ref moveAmount);
        }
        if (moveAmount.y != 0)
        {
            HandleVerticalCollisions(ref moveAmount);
        }

        agent.stoppingDistance = 0;
        warpVelocity = (Vector2)transform.position + moveAmount * agent.speed;
        lastMoveAmount = moveAmount;

        agent.Warp(warpVelocity);
    }

    public void MoveToPoint(Vector2 point)
    {
        agent.SetDestination(point);
    }

    public void FollowTarget(Interactable newTarget)
    {
        agent.stoppingDistance = newTarget.radius * 0.8f ;
        target = newTarget.transform;
    }

    public void StopFollowingTarget()
    {
        agent.stoppingDistance = 0;
        target = null;
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
            if(hit)
            {
                moveAmount.x = (hit.distance - skinWidth) * directionX;
                collisions.left = directionX == -1;
                collisions.right = directionX == 1;
                collisions.any = true;
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
                collisions.bottom = directionY == -1;
                collisions.top = directionY == 1;
                collisions.any = true;
            }
        }
    }

    public struct CollisionInfo
    {
        public bool top, bottom;
        public bool left, right;
        public bool any;

        public void Reset()
        {
            top = bottom = false;
            left = right = false;
            any = false;
        }
    }
}
