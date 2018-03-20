using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSelection : MonoBehaviour {

    private GameObject player;
    private Inventory inventory;
    public string itemName;
    
	// Use this for initialization
	void Start () {
        player = GameObject.FindWithTag("MainPlayer");
        inventory = player.GetComponent<Inventory>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    
    public void Build() {
        inventory.PrepareBuildItem(itemName);
    }
}
