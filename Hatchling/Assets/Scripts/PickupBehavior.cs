﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupBehavior : MonoBehaviour {
    
    public int RespawnTime = 5;
    
	// Use this for initialization
	void Start () {
		
	}
    
    void Disappear() {
        this.GetComponent<Collider>().enabled = false;
        this.GetComponent<MeshRenderer>().enabled = false;
    }
    
    void Reappear() {
        this.GetComponent<Collider>().enabled = true;
        this.GetComponent<MeshRenderer>().enabled = true;
    }
    
    /*void OnCollisionEnter() {
        Disappear();
        Invoke("Respawn",5); //Respawn in five seconds
    }*/
    
    void GetClickedOn(ClickArg args) {
        //print("Hi, I was clicked on");
        args.player.inventory.AddItem(this.tag);
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
