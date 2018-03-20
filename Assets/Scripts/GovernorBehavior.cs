using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GovernorBehavior : MonoBehaviour {

    private bool explainedSituation = false;
    private GameObject town;
    private GameObject player;
    private HUD hud;
    
    void Awake() {
        town = GameObject.FindWithTag("Town");
        player = GameObject.FindWithTag("MainPlayer");
    }
    
	// Use this for initialization
	void Start () {
		hud = player.GetComponent<HUD>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    
    void GetClickedOn(GameObject player) {
        if (explainedSituation) {
            hud.TownMenuOpen = true;
        }
        else {
            player.GetComponent<PlayerBehavior>().Hud.InitiateDialogue("Governor");
            explainedSituation = true;
        }
    }
}
