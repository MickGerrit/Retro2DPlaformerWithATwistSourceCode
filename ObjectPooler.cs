using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour {
    //Sort of singleton to use for the objectpool
    [System.Serializable]
    public class Pool {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public static ObjectPooler Instance;

    private void Awake() {
        Instance = this;
    }
    
    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    Queue<GameObject> objectPool;

    // Setting up the objectpool
    private void Start () {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools) {
            objectPool = new Queue<GameObject>();

            for(int i = 0; i < pool.size; i++) {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
	}
	
    //return a gameobject from the pool with a certain tag.
    public GameObject SpawnFromPool (string tag, Vector3 position, Quaternion rotation) {
        if (!poolDictionary.ContainsKey(tag)) {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
            return null;
        }
        
        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        IPooledObject pooledObj = objectToSpawn.GetComponent<IPooledObject>();

        if (pooledObj != null) {
            pooledObj.OnObjectSpawn();
        }

        //poolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }

    //add a gameobject back in the pool
    public void BackToPool(string tag, GameObject objectBackToPool) {
        Debug.Log("BackToPool");
        poolDictionary[tag].Enqueue(objectBackToPool);
        objectBackToPool.SetActive(false);
    }
    public void AddToPool(GameObject newObjectToPool) {
        GameObject obj = Instantiate(newObjectToPool);
        obj.SetActive(false);
        objectPool.Enqueue(obj);
    }


}
