using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class RaycastEngine : Entity {
    //Variables, serialized so they can be changed in inspector
    [Header("Raycasts")]
    [SerializeField]
    private LayerMask platformMask;
    //Inset so the rays dont touch
    [SerializeField]
    private float parallelInsetLen;
    [SerializeField]
    private float groundTestLen;
    //Inset so the rays dont touch eachother
    [SerializeField]
    private float perpendicularInsetLen;
    [SerializeField]
    private float width;
    [SerializeField]
    private float length;
    [Header("Physics")]
    public float originalGravity;
    public float gravity;
    [SerializeField]
    private float horizSpeedUpAccel;
    [SerializeField]
    private float horizSpeedDownAccel;
    //snapspeed in changing left to right or right to left
    [SerializeField]
    private float horizSnapSpeed;
    //when smooth is false, snapspeed will be immediate
    [SerializeField]
    private bool smooth = true;
    public float horizMaxSpeed;
    public float originalHorizMaxSpeed;

    //the player can break blocks but enemies cant
    [SerializeField]
    private bool blockBreak;

    public Vector2 velocity;
    //Classes with raycast functions
    public RaycastMoveDirection moveDown;
    public RaycastMoveDirection moveLeft;
    public RaycastMoveDirection moveRight;
    public RaycastMoveDirection moveUp;
    public RaycastCheckTouch groundDown;
    
    //floats that are important for the precision of the raycast points in relation to the game object scale
    protected float halfRayWidth;
    protected float halfRayLength;
    
    public Vector2 displacement;

    private void Start() {
        //scaling the distance between the rays at start
        ScaleToOffsetPoints();
        originalGravity = gravity; 
        originalHorizMaxSpeed = horizMaxSpeed;
    }
    //function for returning a whole number float to use for horizontal movement and other functions
    public int GetSign(float v) {
        if (Mathf.Approximately(v, 0)) {
            return 0;
        }
        if (v > 0) {
            return 1;
        } else {
            return -1;
        }
    }

    //This function translates 2 ints to movement of the object and it will switch between 4 directions in which 2 rays are raycasting
    public void Movement(int wantedDir, int velocityDir) {
        int wantedDirection = wantedDir;
        int velocityDirection = velocityDir;
        if (wantedDirection != 0) {
            if (wantedDirection != velocityDirection) {
                velocity.x = horizSnapSpeed * wantedDirection;
            } else  if (smooth == true){
                velocity.x = Mathf.MoveTowards(velocity.x, horizMaxSpeed * wantedDirection, horizSpeedUpAccel * Time.deltaTime);
            } else if (smooth == false) {
                velocity.x = horizMaxSpeed * wantedDirection;
            }
        } else if (smooth == true){
            velocity.x = Mathf.MoveTowards(velocity.x, 0, horizSpeedDownAccel * Time.deltaTime);
        } else if (smooth == false) {
            velocity.x = 0;
        }


        velocity.y -= gravity * Time.deltaTime;

        displacement = Vector2.zero;
        Vector2 wantedDispl = velocity * Time.deltaTime;
        
        if (velocity.x > 0) {
            displacement.x = moveRight.DoRaycast(transform.position, wantedDispl.x);    
        } else if (velocity.x < 0) {
            displacement.x = -moveLeft.DoRaycast(transform.position, -wantedDispl.x);
        }
        if (velocity.y > 0) {
            displacement.y = moveUp.DoRaycast(transform.position, wantedDispl.y);
        } else if (velocity.y < 0) {
            displacement.y = -moveDown.DoRaycast(transform.position, -wantedDispl.y);
        }

        transform.Translate(displacement);

        if (Mathf.Approximately(displacement.x, wantedDispl.x) == false) {
            velocity.x = 0;
        }
        if (Mathf.Approximately(displacement.y, wantedDispl.y) == false) {
            velocity.y = 0;
        }
    }

    //Function to hold the raycasts at its place in relation to the object scale
    public void ScaleToOffsetPoints() {
        bool rayReplace = true;
        if (rayReplace == true) {
            float oldHalfRayLength = halfRayLength;

            //this fixes a bug where the left and right rays will switch because the x scale is minus. this way it tackles it
            if (transform.localScale.x >= 0) {
                halfRayWidth = width / 2 * transform.localScale.x;
            } else if (transform.localScale.x < 0) {
                halfRayWidth = width / 2 * -transform.localScale.x;
            }
            
            halfRayLength = length / 2 * transform.localScale.y;
            moveDown = new RaycastMoveDirection(new Vector2(-halfRayWidth, -halfRayLength), new Vector2(halfRayWidth, -halfRayLength), Vector2.down,
                platformMask, Vector2.right * parallelInsetLen, Vector2.up * perpendicularInsetLen);
            moveLeft = new RaycastMoveDirection(new Vector2(-halfRayWidth, -halfRayLength), new Vector2(-halfRayWidth, halfRayLength), Vector2.left,
                platformMask, Vector2.up * parallelInsetLen, Vector2.right * perpendicularInsetLen);
            moveUp = new RaycastMoveDirection(new Vector2(-halfRayWidth, halfRayLength), new Vector2(halfRayWidth, halfRayLength), Vector2.up,
                platformMask, Vector2.right * parallelInsetLen, Vector2.down * perpendicularInsetLen);
            moveRight = new RaycastMoveDirection(new Vector2(halfRayWidth, -halfRayLength), new Vector2(halfRayWidth, halfRayLength), Vector2.right,
                platformMask, Vector2.up * parallelInsetLen, Vector2.left * perpendicularInsetLen);

            groundDown = new RaycastCheckTouch(new Vector2(-halfRayWidth, -halfRayLength), new Vector2(halfRayWidth, -halfRayLength), Vector2.down,
                platformMask, Vector2.right * parallelInsetLen, Vector2.up * perpendicularInsetLen, groundTestLen);
            transform.position = new Vector3(transform.position.x, transform.position.y + oldHalfRayLength/2, transform.position.z);
            rayReplace = false;
        }
    }
}



