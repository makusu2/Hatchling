using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    public GameObject SpawnedGO;
    public int MaxSpawned;
    public int RespawnWaitPeriod;
    
    private GameObject[] children;
    
	// Use this for initialization
	void Start () {
        children = new GameObject[MaxSpawned];
		for (int i=0;i<MaxSpawned;i++) {
            Vector3 spawnPos = new Vector3(this.transform.position.x+Random.Range(-1.0f,1.0f),
                                           0, //change to y coordinate of ground
                                           this.transform.position.z+Random.Range(-1.0f,1.0f));
            children[i] = Instantiate(SpawnedGO,spawnPos,Quaternion.identity);
            children[i].transform.SetParent(transform);
        }
        Invoke("CheckForRespawn",RespawnWaitPeriod);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    
    public void CheckForRespawn() {
        //If multiple are dead, only respawn one
        foreach (GameObject child in children) {
            if(!child.activeInHierarchy) {
                //not active hierarchy means they're dead
                child.SetActive(true);
                child.gameObject.SendMessage("Respawn",SendMessageOptions.DontRequireReceiver);
                
            }
        }
        Invoke("CheckForRespawn",RespawnWaitPeriod);
    }
}
