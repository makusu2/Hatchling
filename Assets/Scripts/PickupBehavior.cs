using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupBehavior : MonoBehaviour {
    
    public int RespawnTime = 5;
    
    private Vector3 respawnLocation;
    
	// Use this for initialization
	void Start () {
		respawnLocation = transform.position;
	}
    
    public string PickupName;
    
    void Disappear() {
        this.GetComponent<Collider>().enabled = false;
        this.GetComponent<MeshRenderer>().enabled = false;
        try {
            this.GetComponent<Rigidbody>().isKinematic = true;
        }
        catch(MissingComponentException) {}
        foreach (Transform childTrans in transform) {
            childTrans.GetComponent<MeshRenderer>().enabled = false;
        }
    }
    
    void Reappear() {
        this.GetComponent<Collider>().enabled = true;
        this.GetComponent<MeshRenderer>().enabled = true;
        try {
            this.GetComponent<Rigidbody>().isKinematic = false;
        }
        catch(MissingComponentException){}
        foreach (Transform childTrans in transform) {
            childTrans.GetComponent<MeshRenderer>().enabled = true;
        }
        transform.position = respawnLocation;
    }
    
    void GetClickedOn(GameObject player) {
        //print("Hi, I was clicked on");
        player.GetComponent<PlayerBehavior>().inventory.AddItem(PickupName);
        Disappear();
        if(RespawnTime >= 0) {
            Invoke("Respawn",RespawnTime);
        }
    }
    
    void Respawn() {
        Reappear();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
