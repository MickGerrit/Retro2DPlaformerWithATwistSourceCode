using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastCheckTouch{
    private Vector2 raycastDirection;
    public Vector2[] offsetPoints;
    private LayerMask layerMask;
    private float addLength;
    private float raycastLen;

    //Setting up certain raycasts
    public RaycastCheckTouch(Vector2 start, Vector2 end, Vector2 dir, LayerMask mask, Vector2 parallelInset, Vector2 perpendicularInset, float checkLength) {
        this.raycastDirection = dir;
        this.offsetPoints = new Vector2[] {
            start + parallelInset + perpendicularInset,
            end - parallelInset + perpendicularInset,
        };
        this.raycastLen = perpendicularInset.magnitude + checkLength;
        this.layerMask = mask;
    }

    //Checking raycast hits
    public bool DoRaycast(Vector2 origin) {
        foreach (var offset in offsetPoints) {
            RaycastHit2D hit = Raycast(origin + offset, raycastDirection, raycastLen, layerMask);
            if (hit.collider != null && hit.collider.tag != "Coin") {
                return true;
            }
        }
        return false;
    }

    //debugging
    public RaycastHit2D Raycast(Vector2 start, Vector2 dir, float len, LayerMask mask) {
        //Debug.Log(string.Format("Raycast start {0} in {1} for {2}", start, dir, len));
        Debug.DrawLine(start, start + dir * len, Color.red);
        return Physics2D.Raycast(start, dir, len, mask);
    }
}
