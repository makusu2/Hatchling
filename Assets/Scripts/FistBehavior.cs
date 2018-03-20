using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FistBehavior : MonoBehaviour {

    private GameObject player;
    
    
	// Use this for initialization
	void Start () {
        player = GameObject.FindWithTag("MainPlayer");
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    void OnTriggerEnter(Collider col) {
        if(player.GetComponent<PlayerBehavior>().UsingHands && !col.gameObject.CompareTag("Ground") && player.GetComponent<PlayerBehavior>().IsSwinging) {
            col.gameObject.SendMessage("GetSwungAt",player,SendMessageOptions.DontRequireReceiver);
            player.GetComponent<PlayerBehavior>().Arms.GetComponent<Animator>().SetTrigger("CancelSwing"); //NOT WORKING
        }
    }
}
