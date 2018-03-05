﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using UnityEditor;


public class PlayerBehavior : MonoBehaviour {

    public Inventory inventory;
    
    
    public int attackLevel = 2;
    public int defenseLevel = 2;
    public int maxHealth = 10;
    private float currentHealth;
    
    public GameObject EquippedContainer;
    
    public GameObject Arms;
    
    public bool UsingHands = true;
    
    public bool IsSwinging {
        get {
            return Arms.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Punch") || Arms.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Swing");
        }
        set{}
    }
    
    void OnCollisionEnter(Collision col) {
        print("Collided with "+col.gameObject.name);
    }
    
    public void SetEquippedItem(string item) {
        bool wasFound = false;
        
        foreach(Transform transChild in EquippedContainer.transform) {
            if (transChild.gameObject.name == item) {
                wasFound = true;
                transChild.gameObject.SetActive(true);
            }
            else {
                transChild.gameObject.SetActive(false);
            }
        }
        if (wasFound) {
            UsingHands = false;
            Arms.GetComponent<Animator>().SetBool("WeaponIsOn",true);
        }
        else {
            UsingHands = true;
            Arms.GetComponent<Animator>().SetBool("WeaponIsOn",false);
        }
    }
    
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
        Arms = transform.Find("Arms05").gameObject;
        inventory = transform.Find("Inventory").GetComponent<Inventory>();
        currentHealth = maxHealth;
        Hud = GetComponent<HUD>();
        EquippedContainer = GameObject.FindWithTag("EquipContainer").gameObject;
        SetEquippedItem("Hands");
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
        if (Input.GetButtonDown("Fire2")) { 
            UseItem();
        }
        if (Input.GetKeyDown(KeyCode.Tab)) {
            inventory.ToggleInventoryMenu();
        }
        for (int i=0;i<10;i++) {
            if (Input.GetKeyDown(i.ToString())) {
                //print("Selecting slot: "+(i-1).ToString());
                inventory.CurrentlySelectedSlot = i-1; //minus 1 so that 1 is the first element rather than 0
            }
        }
		
	}
    
    void FixedUpdate() {
        
    }
    
    void UseItem() {
        if (UsingHands) {
            //swing with fists
            Arms.GetComponent<Animator>().SetTrigger("Swing");
        }
        else {
            //swing item
            Arms.GetComponent<Animator>().SetTrigger("Swing");
        }
    }
    
    void ClickOn(GameObject obj) {
        obj.SendMessage("GetClickedOn",this.gameObject,SendMessageOptions.DontRequireReceiver);
        
    }
    
    void OnGUI(){
        GUI.Box(new Rect(Screen.width/2,Screen.height/2, 10, 10), ""); //Drawing crosshair
    }
    
}