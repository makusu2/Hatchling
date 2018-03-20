using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandItemBehavior : MonoBehaviour {


    public int AttackLevel = 1;
    
    private GameObject player;
	// Use this for initialization
	void Start () {
		player = GameObject.FindWithTag("MainPlayer");
	}
	
	// Update is called once per frame
	void Update () {
        
	}
    void OnTriggerEnter(Collider col) {
        
        if(!col.gameObject.CompareTag("Ground") && player.GetComponent<PlayerBehavior>().IsSwinging) {
            col.gameObject.SendMessage("GetSwungAt",player,SendMessageOptions.DontRequireReceiver);
            player.GetComponent<PlayerBehavior>().Arms.GetComponent<Animator>().SetTrigger("CancelSwing"); //NOT WORKING
        }
    }
    
    public void ActivateItem() {
        player.GetComponent<PlayerBehavior>().Arms.GetComponent<Animator>().SetTrigger("Swing");
    }
}
