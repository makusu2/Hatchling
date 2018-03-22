using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MiningRock : MonoBehaviour {

    public int MaxStone = 10;
    public int MaxSteel = 2;
    public int RespawnTime = 20;
    
    private int currentStone;
    private int currentSteel;
    
    private System.Random rnd = new System.Random();
    
    private GameObject player;
    
	// Use this for initialization
	void Start () {
		currentStone = MaxStone;
        currentSteel = MaxSteel;
        player = GameObject.FindWithTag("MainPlayer");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    
    void Disappear() {
        this.GetComponent<Collider>().enabled = false;
        this.GetComponent<MeshRenderer>().enabled = false;
    }
    
    void Reappear() {
        this.GetComponent<Collider>().enabled = true;
        this.GetComponent<MeshRenderer>().enabled = true;
    }
    
    public void GetSwungAt(GameObject player) {
        if(player.GetComponent<PlayerBehavior>().CurrentItem.Equals("Pickaxe")) {
            player.GetComponent<PlayerBehavior>().inventory.AddItem(GetItemToGive());
            if(currentStone <= 0 && currentSteel <= 0) {
                Die();
            }
        }
    }
    
    string GetItemToGive() {
        int totalItems = currentStone + currentSteel;
        int randIndex = rnd.Next(0,totalItems);
        if(randIndex > currentStone) {
            currentSteel -= 1;
            return "Steel";
        }
        else {
            currentStone -= 1;
            return "Stone";
        }
    }
    
    
    public void Die() {
        Disappear();
        currentStone = MaxStone;
        currentSteel = MaxSteel;
        Invoke("Reappear",RespawnTime);
    }
}
