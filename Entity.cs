using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class Entity : MonoBehaviour {

    public int health = 1;
    [SerializeField]
    private bool hasHealth;
    public float invincible;
    public Transform prefab;
    [SerializeField]
    private Transform[] instantTransform;
    
    //This is a health checker function. The entity needs to have health (hasHealth), otherwise it can not be destroyed. 
    //Some entities need to drop an item, so therefore the transform and prefab.
    public void DestroyGO() {
        if (health <= 0 && hasHealth == true) {
            Debug.Log("death");
            if (prefab != null) {
                for (int i = 0; i < instantTransform.Length; i++) {
                    Debug.Log("Instantiate");
                    Instantiate(prefab, new Vector3(instantTransform[i].position.x, instantTransform[i].position.y,
                        instantTransform[i].position.z), Quaternion.identity);
                    if (prefab.GetComponent<Rigidbody2D>() != null) {
                        if (prefab.GetComponent<Fireball>() != null) {
                            prefab.GetComponent<Fireball>().stopThis = true;
                        }
                    }
                }
            }

            Destroy(this.gameObject);
        }
    }

    //Use this function not in update
    public void Hit() {
        if (hasHealth == true) {
            health -= 1;
        }
    }

    //5 second invincible powerup
    public void Invincible() {
        Debug.Log(invincible);
        if(invincible > 0) {
            invincible -= Time.deltaTime;
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, .3f);
            hasHealth = false; 

        } else if (invincible <= 0){
            hasHealth = true;
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
        }
    }


}
