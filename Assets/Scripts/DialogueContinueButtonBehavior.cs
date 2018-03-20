using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueContinueButtonBehavior : MonoBehaviour {

    private HUD hud;
    
    void Awake() {
        hud = GameObject.FindWithTag("MainPlayer").GetComponent<HUD>();
    }
    
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void ContinueDialogue() {
        hud.DialogueContinue();
    }
}
