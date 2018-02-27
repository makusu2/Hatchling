using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;


public class PlayerBehavior : MonoBehaviour {

    //public float speed; //shows up in editor
    
    
    private string[] collectibles = {"Apple","Wood","Coin","Stone"};
    
    private Rigidbody rb;
    private Camera cam;
    private CameraBehavior camBehavior;
    private GameObject infoTextBox;
    public Inventory inventory;
    
    
    public int attackLevel = 2;
    public int defenseLevel = 2;
    public int maxHealth = 10;
    private float currentHealth;
    
    
	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();
        cam = GetComponentInChildren(typeof(Camera)) as Camera;
        camBehavior = GetComponentInChildren<CameraBehavior>();
        inventory = transform.Find("Inventory").GetComponent<Inventory>();
        currentHealth = maxHealth;
        infoTextBox = GameObject.Find("InfoPanel").gameObject;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("Fire1")) {
            RaycastHit hit = camBehavior.GetRayHit();
            //print(hit.collider.name);
            try {
                ClickOn(hit.transform.gameObject);
            }
            catch (NullReferenceException e) {
                
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
    
    void SetCursorFree(bool free) {
        
    }
    
    public void SetInfoText(string s) {
        infoTextBox.GetComponent<Text>().text = s;
    }
    
    void FixedUpdate() {
        
    }
    
    void ClickOn(GameObject obj) {
        obj.SendMessage("GetClickedOn",new ClickArg(this,"Hands"),SendMessageOptions.DontRequireReceiver);
        /*if (collectibles.Contains(obj.tag)) {
            inventory.AddItem(obj.tag);
        }*/
        
    }
    
    void OnGUI(){
        GUI.Box(new Rect(Screen.width/2,Screen.height/2, 10, 10), ""); //Drawing crosshair
    }
    /*void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Apple")) {
            other.gameObject.SetActive(false);
        }
    }*/
}