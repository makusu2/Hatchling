using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;


public class PlayerBehavior : MonoBehaviour {

    public Inventory inventory;
    
    
    public int attackLevel = 2;
    public int defenseLevel = 2;
    public int maxHealth = 10;
    private float currentHealth;
    
    public HUD Hud {get;set;}
    
    public string CurrentItem {
        get {
            return inventory.CurrentlySelectedItem;
        }
        set {
            inventory.CurrentlySelectedItem = value;
        }
    }
    
    
    
	// Use this for initialization
	void Start () {
        inventory = transform.Find("Inventory").GetComponent<Inventory>();
        currentHealth = maxHealth;
        Hud = GetComponent<HUD>();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("Fire1")) {
            RaycastHit hit = GetComponentInChildren<CameraBehavior>().GetRayHit();
            try {
                ClickOn(hit.transform.gameObject);
            }
            catch (NullReferenceException) {
                //Nothing was hit, don't need to do anything
            }
        }
        if (Input.GetKeyDown(KeyCode.Tab)) {
            inventory.ToggleInventoryMenu();
        }
        for (int i=0;i<10;i++) {
            if (Input.GetKeyDown(i.ToString())) {
                inventory.CurrentlySelectedSlot = i-1; //minus 1 so that 1 is the first element rather than 0
            }
        }
		
	}
    
    void FixedUpdate() {
        
    }
    
    void ClickOn(GameObject obj) {
        obj.SendMessage("GetClickedOn",this.gameObject,SendMessageOptions.DontRequireReceiver);
        
    }
    
    void OnGUI(){
        GUI.Box(new Rect(Screen.width/2,Screen.height/2, 10, 10), ""); //Drawing crosshair
    }
}