using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinManager : MonoBehaviour {
    [SerializeField]
    private float scaleIncrease;
    private float spawnPosOffset;
    [SerializeField]
    private float offset;
    public int wantedDir;
    [SerializeField]
    private float spawnTimer;
    [SerializeField]
    private float spawnDelay;
    public Vector3 scaleSet;

    public GameObject fireball;
    // Audio
    public AudioClip nom;
    public AudioClip coin;
    private AudioSource source;

    private void Start() {
        source = GetComponent<AudioSource>();
        spawnPosOffset = 1f;
        wantedDir = 1;
        spawnTimer = 0;
        scaleSet = transform.localScale;
    }

    //Setting the spawnpoint according to where the gameobject is facing
    private void Update() {
        if (Input.GetAxisRaw("Horizontal") < 0) {
            spawnPosOffset = -offset;
        } else if (Input.GetAxisRaw("Horizontal") > 0) {
            spawnPosOffset = offset;
        }
        if (Input.GetAxisRaw("Horizontal") == 1) {
            wantedDir = 1;
        } else if (Input.GetAxisRaw("Horizontal") == -1) {
            wantedDir = -1;
        }

        spawnTimer += 1 * Time.deltaTime;

        //Spawn a fireball, decreasing coinamount of player, and setting the scale of all the raycastpoints.
        if (Input.GetButtonDown("Fire1") && (spawnTimer > spawnDelay) && this.gameObject.GetComponent<PlayerController>().coinAmount > 0) {
            ObjectPooler.Instance.SpawnFromPool("Fireball", SpawnPos(), transform.rotation);
            //play coin sound
            source.PlayOneShot(coin, 1);
            source.pitch = Random.Range(0.9f, 1.1f);
            this.gameObject.GetComponent<PlayerController>().coinAmount -= 1;
            scaleSet -= new Vector3(scaleIncrease, scaleIncrease, scaleIncrease);
            this.gameObject.GetComponent<PlayerController>().ScaleToOffsetPoints();
            spawnTimer = 0f;
        }
    }

    private Vector3 SpawnPos() {
        return new Vector3(transform.position.x + spawnPosOffset / 2, transform.position.y + 0.2f, transform.position.z);
    }

    //Spawn a fireball, increasing coinamount of player, and setting the scale of all the raycastpoints, 
    //these work with a few gameobjects in OntriggerEnter2D
    private void OnTriggerEnter2D(Collider2D col) {
        if (col.tag == "Coin" || col.tag == "Fireball") {
            this.gameObject.GetComponent<PlayerController>().coinAmount += 1;
            scaleSet += new Vector3(scaleIncrease, scaleIncrease, scaleIncrease);
            Debug.Log("CoinCol");
            if (col.tag == "Coin") {
                ObjectPooler.Instance.AddToPool(fireball);
                Destroy(col.gameObject);
            }
            if (col.tag == "Fireball") {
                ObjectPooler.Instance.BackToPool("Fireball", col.gameObject.transform.parent.gameObject);
            }
            //nom sound
            source.PlayOneShot(nom, 1);
            source.pitch = Random.Range(0.9f, 1.1f);
            this.gameObject.GetComponent<PlayerController>().ScaleToOffsetPoints();
        }
        if (col.tag == "Mushroom") {
            for (int i = 0; i < 4; i++){
                gameObject.GetComponent<PlayerController>().coinAmount += 1;
                scaleSet += new Vector3(scaleIncrease, scaleIncrease, scaleIncrease);
                ObjectPooler.Instance.AddToPool(fireball);
            }
            this.gameObject.GetComponent<PlayerController>().ScaleToOffsetPoints();
            col.SendMessage("Hit");
        }
    }
}
