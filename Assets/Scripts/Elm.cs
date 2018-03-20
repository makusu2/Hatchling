using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elm : MonoBehaviour {

    public int MaxWood = 30;
    public int MaxBranches = 4;
    public int RespawnTime = 20;
    
    private int currentWood;
    private int currentBranches;
    
	// Use this for initialization
	void Start () {
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
    
    public void GetSwungAt(GameObject player) {
        if(player.GetComponent<PlayerBehavior>().CurrentItem.Equals("Hatchet")) {
            player.GetComponent<PlayerBehavior>().inventory.AddItem("Wood");
            currentWood -= 1;
            if(currentWood <= 0) {
                Die();
            }
        }
    }
    
    void GetClickedOn(GameObject player) {
        if(currentBranches > 0) {
            player.GetComponent<PlayerBehavior>().inventory.AddItem("Branch");
            currentBranches -= 1;
        }
    }
    
    public void Die() {
        Disappear();
        currentWood = MaxWood;
        currentBranches = MaxBranches;
        Invoke("Reappear",RespawnTime);
    }
}
