using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using UnityEditor;


public class PlayerBehavior : MonoBehaviour {

    public Inventory inventory;
    public HUD Hud;
    
    private int attackLevel;
    public int AttackLevel {
        get {
            return attackLevel;
        }
        set {
            attackLevel = value;
            Hud.SetAttackStat(value);
        }
    }
    //public int defenseLevel = 2;
    public int maxHealth = 100;
    
    private float currentHealth;
    public float Health {
        get{return currentHealth;}
        set {
            if(value<=0) {
                currentHealth = 0;
                Hud.SetHealthStat(0);
                Die();
            }
            else {
                Hud.SetHealthStat((int)value);
                currentHealth = value;
            }
        }
    }
    
    
    public GameObject HeldItemObject {
        get {
            foreach(Transform transChild in EquippedContainer.transform) {
                if (transChild.gameObject.activeInHierarchy) {
                    return transChild.gameObject;
                }
            }
            throw new NullReferenceException();
        }
        set {}
    }
            
    
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
                try {
                    AttackLevel = transChild.gameObject.GetComponent<HandItemBehavior>().AttackLevel;
                }
                catch(NullReferenceException) {
                    AttackLevel = 1;
                }
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
            AttackLevel = 1;
        }
    }
    
    
    public string CurrentItem {
        get {
            return inventory.CurrentlySelectedItem;
        }
        set {
            inventory.CurrentlySelectedItem = value;
        }
    }
    
    void Awake() {
        Arms = GameObject.FindWithTag("Arms");
        
    }
    
	// Use this for initialization
	void Start () {
        Hud = GetComponent<HUD>();
        inventory = gameObject.GetComponent<Inventory>();
        Health = maxHealth;
        EquippedContainer = GameObject.FindWithTag("EquipContainer").gameObject;
        SetEquippedItem("Hands");
	}
	
	// Update is called once per frame
	void Update () {
        if (inventory.PreparingBuild) {
            if (Input.GetButtonDown("Fire1")) {
                inventory.CompleteBuildItem();
            }
            else {
                RaycastHit hit = GetComponentInChildren<CameraBehavior>().GetRayHit();
                Vector3 newPosition = hit.point;
                newPosition.y = newPosition.y + inventory.PrepareBuildOffset;
                inventory.PrepareBuildObject.transform.position = newPosition;
                //Update location
            }
            return;
        }
        if (!Hud.UsingUI && Input.GetButtonDown("Fire1")) {
            RaycastHit hit = GetComponentInChildren<CameraBehavior>().GetRayHit();
            try {
                ClickOn(hit.transform.gameObject);
            }
            catch (NullReferenceException) {
                //Nothing was hit, don't need to do anything
            }
        }
        if (!Hud.UsingUI && Input.GetButtonDown("Fire2")) { 
            UseItem();
        }
        if (Input.GetKeyDown(KeyCode.Tab)) {
            Hud.InventoryMenuOpen = !Hud.InventoryMenuOpen;
        }
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Hud.PauseMenuOpen = !Hud.PauseMenuOpen;
        }
        for (int i=0;i<10;i++) {
            if (Input.GetKeyDown(i.ToString())) {
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
            HeldItemObject.SendMessage("ActivateItem",SendMessageOptions.RequireReceiver);
        }
    }
    
    void ClickOn(GameObject obj) {
        obj.SendMessage("GetClickedOn",this.gameObject,SendMessageOptions.DontRequireReceiver);
        
    }
    
    void OnGUI(){
        GUI.Box(new Rect(Screen.width/2,Screen.height/2, 10, 10), ""); //Drawing crosshair
    }
    
    public void GetDamaged(int attackLevel) {
        Health -= attackLevel;
    }
    
    void Die() {
        print("YOU HAVE DIED");
    }
}