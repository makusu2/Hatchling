using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour {

    public float speed; //shows up in editor
    
    
    
    private Rigidbody rb;
    
    
	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    
    void FixedUpdate() {
        float move_horizontal = Input.GetAxis("Horizontal");
        float move_vertical = Input.GetAxis("Vertical");
        
        Vector3 movement = new Vector3(move_horizontal,0,move_vertical);
        rb.AddForce(movement*speed);
    }
    
    void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Apple")) {
            other.gameObject.SetActive(false);
        }
    }
}
