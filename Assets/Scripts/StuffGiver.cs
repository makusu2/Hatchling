using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StuffGiver : MonoBehaviour {
    
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    
    void GetClickedOn(GameObject player) {
        //print("Hi, I was clicked on");
        string[] items = {"Wood","Stone","Apple","Branch","Steel"};
        for(int i=0;i<10;i++) {
            foreach (string item in items) {
                player.GetComponent<PlayerBehavior>().inventory.AddItem(item);
            }
        }
    }
}
