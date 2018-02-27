using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupBehavior : MonoBehaviour {
    
    public int RespawnTime = 5;
    
	// Use this for initialization
	void Start () {
		
	}
    
    public string PickupName;
    
    void Disappear() {
        this.GetComponent<Collider>().enabled = false;
        this.GetComponent<MeshRenderer>().enabled = false;
    }
    
    void Reappear() {
        this.GetComponent<Collider>().enabled = true;
        this.GetComponent<MeshRenderer>().enabled = true;
    }
    
    void GetClickedOn(GameObject player) {
        //print("Hi, I was clicked on");
        player.GetComponent<PlayerBehavior>().inventory.AddItem(PickupName);
        Disappear();
        Invoke("Respawn",RespawnTime);
    }
    
    void Respawn() {
        Reappear();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
