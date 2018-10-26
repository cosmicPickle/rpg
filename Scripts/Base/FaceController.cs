using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceController : MonoBehaviour {

    public Vector2 defaultFaceDirection;
    Vector2 faceDirection;

    [Header("Debug Settings")]
    public bool showFaceDirection = true;


    void Awake()
    {
        faceDirection = defaultFaceDirection;
    }

    public Vector2 GetFaceDirection()
    {
        return faceDirection;
    }

	public void UpdateFaceDirection(Vector2 moveAmount)
    {
        if (moveAmount != Vector2.zero)
        {
            if (Mathf.Abs(moveAmount.y) > Mathf.Abs(moveAmount.x))
            {
                faceDirection = new Vector2(0, Mathf.Sign(moveAmount.y));
            }
            else
            {
                faceDirection = new Vector2(Mathf.Sign(moveAmount.x), 0);
            }
        }

        if (faceDirection == Vector2.zero)
        {
            faceDirection = defaultFaceDirection;
        }
    }

    public void UpdateFaceDirection(Transform target)
    {
        Vector2 heading = target.position - transform.position;
        UpdateFaceDirection(heading);
    }

    public void LookAt(Vector2 target)
    {
        Vector2 heading = target - (Vector2)transform.position;
        UpdateFaceDirection(heading);
    }

    void OnDrawGizmos()
    {
        if (showFaceDirection)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(new Ray(transform.position, faceDirection));
        }
    }
}
