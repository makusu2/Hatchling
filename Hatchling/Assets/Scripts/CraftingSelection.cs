using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingSelection : MonoBehaviour {

    private Inventory inventory;
    public string itemName;
    
	// Use this for initialization
	void Start () {
        inventory = GameObject.Find("Inventory").GetComponent<Inventory>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    
    public void Craft() {
        inventory.CraftItem(itemName);
    }
}
