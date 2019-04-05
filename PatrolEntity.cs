using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolEntity : RaycastEngine {

    [SerializeField]
    private Transform Pos1;
    [SerializeField]
    private Transform Pos2;
    private bool walkR;
	
    //Simple patrolling enemy with the raycastengine. They move from one point to the other
	private void FixedUpdate () {
        if (GetSign(velocity.x) == 0) {
            walkR = !walkR;
        }
        if (transform.position.x >= Pos2.position.x && Pos2 != null) {
            walkR = false;
            gameObject.GetComponent<SpriteRenderer>().flipX = false;
        }  if (transform.position.x <= Pos1.position.x && Pos1 != null) {
            walkR = true;
            gameObject.GetComponent<SpriteRenderer>().flipX = true;
        }
        if (walkR) {
            Movement(1, 1);
        }
        if (!walkR) {
            Movement(-1, -1);
        }
        DestroyGO();
    }
}
