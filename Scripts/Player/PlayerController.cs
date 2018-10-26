using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Controller2D))]
[RequireComponent(typeof(NavMeshAgent2D))]
[RequireComponent(typeof(Character))]
public class PlayerController : MonoBehaviour {

    public float interactableDetectDistance = 3f;
    public LayerMask intercatableMask;

    [HideInInspector]
    public Controller2D controller;
    [HideInInspector]
    public Character character;
    [HideInInspector]
    public Interactable focus;
    [HideInInspector]
    public Vector2 playerInput;

    const float mouseRaycastDistance = 50f;

	// Use this for initialization
	void Awake () {
        controller = GetComponent<Controller2D>();
        character = GetComponent<Character>();
        controller.SetSpeed(character.defenceStats.speed.GetValue());
    }
	
	// Update is called once per frame
	void Update () {

        if(EventSystem.current.IsPointerOverGameObject())
        {
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

            if (hit)
            {
                Interactable interactable = hit.GetComponent<Interactable>();
                if (interactable != null)
                {
                    SetFocus(interactable);
                }
            }
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

    Vector2 CastRayToWorld()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector2 point = ray.origin + (ray.direction * (Mathf.Abs(Camera.main.transform.position.z - transform.position.z)));
        return point;
    }

    Collider2D CastRayToInteractable()
    {
        Vector2 cameraToWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(cameraToWorld, Vector2.zero, 0f, intercatableMask);

        return hit ? hit.collider : null;

    }

    Collider2D FindClosestInteractable()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, interactableDetectDistance, intercatableMask);

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
