using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

public class PlayerBehavior : MonoBehaviour {

    //public float speed; //shows up in editor
    
    private string[] collectibles = {"Apple","Wood","Coin"};
    
    private Rigidbody rb;
    private Camera cam;
    private CameraBehavior camBehavior;
    private Inventory inventory;
    
    
    
	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();
        cam = GetComponentInChildren(typeof(Camera)) as Camera;
        camBehavior = GetComponentInChildren<CameraBehavior>();
        inventory = transform.Find("Inventory").GetComponent<Inventory>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    
    void FixedUpdate() {
        if (Input.GetButtonDown("Fire1")) {
            RaycastHit hit = camBehavior.GetRayHit();
            //print(hit.collider.name);
            try {
                ClickOn(hit.transform.gameObject);
            }
            catch (NullReferenceException e) {
                
            }
        }
        
    }
    
    void ClickOn(GameObject obj) {
        //print(obj);
        obj.SendMessage("GetClickedOn",SendMessageOptions.DontRequireReceiver);
        if (collectibles.Contains(obj.tag)) {
            inventory.AddItem(obj.tag);
        }
    }
    
    void OnGUI(){
        GUI.Box(new Rect(Screen.width/2,Screen.height/2, 10, 10), "");
    }
    /*void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Apple")) {
            other.gameObject.SetActive(false);
        }
    }*/
}
