using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingSelection : MonoBehaviour {

    private GameObject player;
    private Inventory inventory;
    public string itemName;
    
    void Awake() {
        player = GameObject.FindWithTag("MainPlayer");
    }
    
	void Start () {
        player = GameObject.FindWithTag("MainPlayer");
        inventory = player.GetComponent<Inventory>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    
    public void Craft() {
        inventory.CraftItem(itemName);
    }
}
