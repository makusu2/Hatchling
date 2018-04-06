using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Elm : MonoBehaviour {

    public int MaxWood = 30;
    public int MaxBranches = 4;
    public int RespawnTime = 20;
    
    private int currentWood;
    private int currentBranches;
    
    private static GameObject player;
    
    public int Wood {
        get {return currentWood;}
        set {
            currentWood = Math.Max(value,0);
            if (currentWood == 0) {
                Die();
            }
        }
    }
    public int Branches {
        get { return currentBranches;}
        set {
            currentBranches = value;
        }
    }
    
	// Use this for initialization
	void Start () {
        if(player == null) {
            player = GameObject.FindWithTag("MainPlayer");
        }
		currentWood = MaxWood;
        currentBranches = MaxBranches;
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
    
    public void GetSwungAt() {
        if(player.GetComponent<PlayerBehavior>().CurrentItem.Equals("Hatchet")) {
            player.GetComponent<PlayerBehavior>().inventory.AddItem("Wood");
            
            Wood -= 1;
        }
    }
    
    void GetClickedOn(GameObject player) {
        if(Branches > 0) {
            player.GetComponent<PlayerBehavior>().inventory.AddItem("Branch");
            Branches -= 1;
        }
    }
    
    public void Die() {
        Disappear();
        Wood = MaxWood;
        Branches = MaxBranches;
        Invoke("Reappear",RespawnTime);
    }
}
