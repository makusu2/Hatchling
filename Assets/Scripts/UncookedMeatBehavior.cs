using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UncookedMeatBehavior : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    
    void OnCollisionEnter(Collision col) {
        try {
            if (col.gameObject.GetComponent<Heated>().HeatLevel >= 3) {
                GetCooked();
            }
        }
        catch(NullReferenceException) { }
    }
    
    void GetCooked() {
        GameObject cookedGO = Instantiate(Resources.Load("InWorld/CookedMeat") as GameObject);
        cookedGO.SetActive(true);
        cookedGO.transform.position = transform.position;
        Destroy(this);
    }
}
