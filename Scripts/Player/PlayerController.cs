using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Controller2D))]
[RequireComponent(typeof(NavMeshAgent2D))]
[RequireComponent(typeof(Character))]
[RequireComponent(typeof(AttackAgentManager))]
public class PlayerController : MonoBehaviour {

    public float interactableDetectDistance = 3f;
    public LayerMask interactableMask;
    public LayerMask enemyMask;

    [HideInInspector]
    public Controller2D controller;
    [HideInInspector]
    public Character character;
    [HideInInspector]
    public Interactable focus;
    [HideInInspector]
    public Vector2 playerInput;

    AttackAgentManager attackAgentManager;
    AttackAgent attackAgent;

    const float mouseRaycastDistance = 50f;

    Character.Knockback currentKnockback;

	// Use this for initialization
	void Awake () {
        controller = GetComponent<Controller2D>();
        character = GetComponent<Character>();
        controller.SetSpeed(character.defenceStats.speed.GetValue());

        attackAgentManager = GetComponent<AttackAgentManager>();
        attackAgentManager.Instantiate();

        attackAgent = attackAgentManager.GetAttackAgent();
        if (attackAgent != null)
        {
            attackAgent.Init(enemyMask, controller.collisionMask);
        }

        character.onTakeDamage += OnTakeDamage;
        EquipmentManager.onEquipmentChanged += OnEquipmentChanged;
    }
	
	// Update is called once per frame
	void Update () {
        if(attackAgent == null)
        {
            attackAgent = attackAgentManager.GetAttackAgent();
            if (attackAgent != null)
            {
                attackAgent.Init(enemyMask, controller.collisionMask);
            }
        }

        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if(currentKnockback.effect > 0)
        {
            Vector2 moveAmount = currentKnockback.direction * Time.deltaTime * currentKnockback.speed;
            currentKnockback.effect -= moveAmount.magnitude;
            controller.Move(moveAmount, -1);
            
            return;
        }

        controller.SetSpeed(character.defenceStats.speed.GetValue());

        playerInput = Vector2.zero;

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 destination = CastRayToWorld();
            controller.MoveToPoint(destination);
            RemoveFocus();
        } else
        {
            playerInput = new Vector2(
                Input.GetAxisRaw("Horizontal"),
                Input.GetAxisRaw("Vertical")
            );

            if (playerInput != Vector2.zero)
            {
                controller.Move(playerInput * Time.deltaTime);
                RemoveFocus();
            }
        }

        if (Input.GetButtonDown("Action"))
        {

            Collider2D hit = null;
            if (Input.GetMouseButtonDown(1))
            {
                hit = CastRayToInteractable();

            }
            else
            {
                hit = FindClosestInteractable();
            }

            bool hasInteracted = false;

            if (hit)
            {
                Interactable interactable = hit.GetComponent<Interactable>();
                if (interactable != null)
                {
                    SetFocus(interactable);
                    hasInteracted = true;
                }
            }

            if(!hasInteracted)
            {
                PerformDefaultAction();
            }
            
        }
    }

    void PerformDefaultAction()
    {
        if (attackAgent != null)
        {
            attackAgent.Attack();
        }
    }

    void SetFocus(Interactable newFocus)
    {
        if (newFocus != focus)
        {
            if(focus != null)
            {
                focus.onDefocused();
            }

            focus = newFocus;
            controller.FollowTarget(newFocus);
        }

        focus.OnFocused(transform);
    }

    void RemoveFocus()
    {
        if(focus != null)
        {
            focus.onDefocused();
        }
        focus = null;
        controller.StopFollowingTarget();
    }

    void OnTakeDamage(float finalDamage, Character.Knockback knockback, AttackAgent attackAgent) 
    {
        currentKnockback = knockback;
    }

    void OnEquipmentChanged(Equipment newItem, Equipment oldItem)
    {
        if (newItem != null)
        {
            attackAgentManager.ChangePreset(newItem.attackAgent);
        } else
        {
            if(oldItem != null)
            {
                attackAgentManager.Reset();
            }
        }
    }

    Vector2 CastRayToWorld()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector2 point = ray.origin + (ray.direction * (Mathf.Abs(Camera.main.transform.position.z - transform.position.z)));
        return point;
    }

    Collider2D CastRayToInteractable()
    {
        Vector2 cameraToWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(cameraToWorld, Vector2.zero, 0f, interactableMask);

        return hit ? hit.collider : null;

    }

    Collider2D FindClosestInteractable()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, interactableDetectDistance, interactableMask);

        float minSqrtDistance = float.MaxValue;
        Collider2D target = null;

        for(int i = 0; i < colliders.Length; i++)
        {
            float distance = (transform.position - colliders[i].transform.position).magnitude;

            if(distance < minSqrtDistance)
            {
                minSqrtDistance = distance;
                target = colliders[i];
            }
        }

        return target;
    }
}
