using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolkitBehavior : MonoBehaviour {

    private GameObject player;
    
	// Use this for initialization
	void Start () {
		player = GameObject.FindWithTag("MainPlayer");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    
    public void ActivateItem() {
        player.GetComponent<PlayerBehavior>().Hud.BuildingMenuOpen = !player.GetComponent<PlayerBehavior>().Hud.BuildingMenuOpen;
    }
}
