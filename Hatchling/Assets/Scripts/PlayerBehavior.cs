using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerBehavior : MonoBehaviour {

    //public float speed; //shows up in editor
    
    
    
    private Rigidbody rb;
    private Camera cam;
    private CameraBehavior camBehavior;
    
    
    
	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();
        cam = GetComponentInChildren(typeof(Camera)) as Camera;
        camBehavior = GetComponentInChildren<CameraBehavior>();
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
