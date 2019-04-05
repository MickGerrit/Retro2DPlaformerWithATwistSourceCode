using UnityEngine;
using System.Collections;

public class RaycastMoveDirection{
    protected Vector2 raycastDirection;
    public Vector2[] offsetPoints;
    protected LayerMask layerMask;
    protected float addLength;
    public GameObject lastHit;

    //Setting up certain raycasts
    public RaycastMoveDirection(Vector2 start, Vector2 end, Vector2 dir, LayerMask mask, 
        Vector2 parallelInset, Vector2 perpendicularInset) {
        this.raycastDirection = dir;
        Debug.Log("Setting raypoints");
        this.offsetPoints = new Vector2[] {
            start + parallelInset + perpendicularInset,
            end - parallelInset + perpendicularInset,
        };
        this.addLength = perpendicularInset.magnitude;
        this.layerMask = mask;
    }

    //Checking raycast hits
    public float DoRaycast(Vector2 origin, float distance) {
        float minDistance = distance;
        foreach(var offset in offsetPoints) {
            RaycastHit2D hit = Raycast(origin + offset, raycastDirection, distance + addLength, layerMask);
            if (hit.collider != null) {
                   lastHit = hit.collider.gameObject;
                   minDistance = Mathf.Min(minDistance, hit.distance - addLength);
            }
        }
        return minDistance;
    }

    //debugging
    public RaycastHit2D Raycast(Vector2 start, Vector2 dir, float len, LayerMask mask) { 
        //Debug.Log(string.Format("Raycast start {0} in {1} for {2}", start, dir, len));
        Debug.DrawLine(start, start + dir * len, Color.blue);
        return Physics2D.Raycast(start, dir, len, mask);
    }
}
