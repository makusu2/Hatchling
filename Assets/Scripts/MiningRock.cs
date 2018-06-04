using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MiningRock : MonoBehaviour {

    public static int MaxStone = 10;
    public static int MaxSteel = 2;
    public static int RespawnTime = 20;
    
    private int currentStone = MaxStone;
    private int currentSteel = MaxSteel;
    
    private static GameObject player;
    
	// Use this for initialization
	void Start () {
        player = player??GameObject.FindWithTag("MainPlayer");
	}
    
    void Disappear() {
        this.GetComponent<Collider>().enabled = false;
        this.GetComponent<MeshRenderer>().enabled = false;
    }
    
    void Reappear() {
        this.GetComponent<Collider>().enabled = true;
        this.GetComponent<MeshRenderer>().enabled = true;
    }
    
    public void GetSwungAt() {
        if(player.GetComponent<PlayerBehavior>().CurrentItem.Equals("Pickaxe")) {
            player.GetComponent<PlayerBehavior>().inventory.AddItem(GetItemToGive());
            if(currentStone <= 0 && currentSteel <= 0) {
                Die();
            }
        }
    }
    
    string GetItemToGive() {
        int totalItems = currentStone + currentSteel;
        int randIndex = MakuUtil.rnd.Next(0,totalItems);
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
