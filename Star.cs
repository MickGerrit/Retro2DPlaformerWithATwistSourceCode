using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : Entity {
    
    private Rigidbody2D rb;
    [SerializeField]
    private float force;
    public AudioClip hit;
    private AudioSource source;

    //the gameobject of this class needs to have an rb)
    private void Start() {
        source = GetComponent<AudioSource>();
        rb = this.GetComponent<Rigidbody2D>();
        rb.AddForce(new Vector2(force, 0f));
    }

    private void Update() {
        DestroyGO();

    }

    private void OnCollisionEnter2D(Collision2D collision) {
        source.PlayOneShot(hit, 1);
    }
}
