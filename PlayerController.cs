using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : RaycastEngine {
    //little enum for a state system
    private enum JumpState {
        None = 0, Holding,
    }
    //Variables enzo. 
    [Header("Jump")]
    [SerializeField]
    private float JumpInputLeewayPeriod;
    [SerializeField]
    private float jumpStartSpeed;
    [SerializeField]
    private float jumpMaxHoldPeriod;
    [SerializeField]
    private float jumpMinSpeed;
    [SerializeField]
    private float weightMultiplier;
    public int coinAmount;

    private float jumpStartTimer;
    private float jumpHoldTimer;
    private bool jumpInputDown;
    private JumpState jumpState;
    private bool setScale = false;
    private int testAmount = 0;
    [SerializeField]
    private float invincibleTime;
    private int idleDir = 1;

    //audio
    public AudioClip jump;
    public AudioClip brick;
    public AudioClip goomba;
    private AudioSource source;

    [SerializeField]
    private GameObject canvas;
    private MySceneManager sceneloader;
    private Text coinDisplay;

    private void Awake() {
        source = GetComponent<AudioSource>();
        sceneloader = canvas.GetComponent<MySceneManager>();
        coinDisplay = canvas.transform.GetChild(0).GetComponent<Text>();
        DisplayCoins();
    }

    private void Update() {
        jumpStartTimer -= Time.deltaTime;
        //check for jumpinput
        bool jumpBtn = Input.GetButton("Jump");
        if (jumpBtn && jumpInputDown == false) {
            jumpStartTimer = JumpInputLeewayPeriod;

        }
        //jump sound
        if (Input.GetButtonDown("Jump")) {
            source.PlayOneShot(jump, 1);
            source.pitch = Random.Range(0.9f, 1.1f);
        }

        //modify gravity in relation to amount of coins
        gravity = originalGravity + coinAmount * weightMultiplier;
        if (Input.GetKeyDown(KeyCode.C)) {
            testAmount += 1;
        }
        jumpInputDown = jumpBtn;
        //this game object can be destroyed at one point
        DestroyGO();

        //the speed in relation to the cointamount
        horizMaxSpeed = originalHorizMaxSpeed - coinAmount * weightMultiplier * 0.1f ;
        if (Input.GetKeyDown(KeyCode.C)) {
            Debug.Log(coinAmount);
        }

        //Setting the x scale right and flip the dollar sprite
        if (GetSign(displacement.x) < 0) {
            idleDir = GetSign(displacement.x);
            transform.GetChild(1).GetComponent<SpriteRenderer>().flipX = true;
        }
        if (GetSign(displacement.x) > 0) {
            idleDir = GetSign(displacement.x);
            transform.GetChild(1).GetComponent<SpriteRenderer>().flipX = false;
        }

        transform.localScale = new Vector3(gameObject.GetComponent<CoinManager>().scaleSet.x * idleDir,
        gameObject.GetComponent<CoinManager>().scaleSet.y, gameObject.GetComponent<CoinManager>().scaleSet.z);
        
        //check for invincibility power up
        Invincible();
        
        DisplayCoins();
    }

    private void FixedUpdate() {
        //jump state system to give the player control over the jump
        switch (jumpState) {
            case JumpState.None:
            if (groundDown.DoRaycast(transform.position) && jumpStartTimer > 0) {
                jumpStartTimer = 0;
                jumpState = JumpState.Holding;
                jumpHoldTimer = 0;
                velocity.y = jumpStartSpeed;
            }
            break;
            case JumpState.Holding:
            jumpHoldTimer += Time.deltaTime;
            if (jumpInputDown == false || jumpHoldTimer > jumpMaxHoldPeriod) {
                jumpState = JumpState.None;
                //This gets rid of a stutter in mid air. Because of the LeewayPeriod it stuttered
                if (jumpHoldTimer < JumpInputLeewayPeriod) {
                    velocity.y = Mathf.Lerp(jumpMinSpeed, jumpStartSpeed, jumpHoldTimer / jumpMaxHoldPeriod);
                }
            }
            break;
        }
        if (jumpState == JumpState.None) {
            velocity.y -= gravity * Time.deltaTime;
        }

        float horizInput = Input.GetAxisRaw("Horizontal");
        int wantedDir = GetSign(horizInput);
        int velocityDir = GetSign(velocity.x);

        
        Movement(wantedDir, velocityDir);
        //Raycast detections. For each of the directions the gameobject are stored gameobject variables.
        if (moveUp.lastHit != null) {
            if (moveUp.lastHit.tag == "Block") {
                moveUp.lastHit.SendMessage("Hit");
                source.PlayOneShot(brick, 1);
                source.pitch = Random.Range(0.9f, 1.1f);
                moveUp.lastHit = null;
            }
        }
        if (moveUp.lastHit != null) {
            if (moveUp.lastHit.tag == "CoinBlock") {
                moveUp.lastHit.SendMessage("Hit");
                source.PlayOneShot(brick, 1);
                source.pitch = Random.Range(0.9f, 1.1f);
                coinAmount += 1;
                moveUp.lastHit = null;
            }
        }

        if (moveDown.lastHit != null) {
            if (moveDown.lastHit.tag == "Goomba") {
                moveDown.lastHit.SendMessage("Hit");
                source.PlayOneShot(goomba, 1);
                source.pitch = Random.Range(0.9f, 1.1f);
                moveDown.lastHit = null;
            } 
        }
        

        if (moveRight.lastHit != null) {
            if (moveRight.lastHit.tag == "Goomba") {
                if (invincible > 0) {
                    moveRight.lastHit.SendMessage("Hit");
                    source.PlayOneShot(goomba, 1);
                    source.pitch = Random.Range(0.9f, 1.1f);
                } else {
                    sceneloader.SceneLoader(2);
                }
                moveRight.lastHit = null;
            } 
        }
        if (moveLeft.lastHit != null) {
            if (moveLeft.lastHit.tag == "Goomba") {
                if (invincible > 0) {
                    moveLeft.lastHit.SendMessage("Hit");
                    source.PlayOneShot(goomba, 1);
                    source.pitch = Random.Range(0.9f, 1.1f);
                } else {
                    sceneloader.SceneLoader(2);
                }
                moveDown.lastHit = null;
            }
        }
    }

    //Little coin function
    private void DisplayCoins() {
        coinDisplay.text = "Coins collected: " + coinAmount.ToString();
    }

    private void OnTriggerEnter2D(Collider2D col) {
        if (col.tag == "Star") {
            invincible = invincibleTime;
            col.SendMessage("Hit");
        }
        //the van is the end of the level
        if (col.tag == "Van") {
            sceneloader.SceneLoader(3);
        }
    }
}

