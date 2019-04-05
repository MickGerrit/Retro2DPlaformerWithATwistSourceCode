using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//the objectpoolscript recognizes the game objects with a tag and the inteface IPooledObject
public class Fireball : RaycastEngine, IPooledObject {
    
    private int neededDir = 1;
    private GameObject player;
    [SerializeField]
    private Rigidbody2D rb;
    public bool stopThis;
    [SerializeField]
    private bool noHorizMovement;
    private float enemyHitDelay;
    private GameObject goomba;

    //This function is a must. This is the startfunction at objectspawn.
    //At spawn the fireball needs to get resetted to get working with the raycastengine again
    public void OnObjectSpawn() {
        if (player == null) {
            
            player = GameObject.FindWithTag("Player");
        }
        neededDir = player.GetComponent<CoinManager>().wantedDir;
        rb.isKinematic = true;
        velocity.x = 0;
        velocity.y = 0;
        stopThis = false;
        if (rb == null) {
            rb = this.GetComponent<Rigidbody2D>();
        }
        rb.velocity = Vector3.zero;
        enemyHitDelay = 0;
        transform.GetChild(0).gameObject.SetActive(false);
    }

    private void FixedUpdate() {
        if (stopThis == false) {

            Movement(neededDir, neededDir);

            transform.rotation = Quaternion.Euler(0, 0, 0);
        } else {
            rb.isKinematic = true;
        }
        if (groundDown.DoRaycast(transform.position) && GetSign(velocity.x) != 0) {
            velocity.y += 1;
        }
        //fireballs are going to use rigidbodyphysics when they hit the wall
        if (GetSign(velocity.x) == 0) {
            rb.gravityScale = 1;
            neededDir = 0;
            velocity.x = 0;
            velocity.y -= 1f * Time.deltaTime;
            rb.isKinematic = false;
            stopThis = true;
            transform.GetChild(0).gameObject.SetActive(true);
        } else {
            rb.isKinematic = true;
            Debug.Log("Delaying");
        }


    }
    
    private void Update() {
        if (stopThis == true) {
            enemyHitDelay += Time.deltaTime;
        } else {
            enemyHitDelay = 0;
        }
    }

    private void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.tag == "Goomba" && enemyHitDelay <0.1f) {
            col.gameObject.SendMessage("Hit");
        }
    }
}
