using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InfoHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public string infoStr;
    
    private GameObject player;
    private HUD hud;
    
	// Use this for initialization
	void Awake () {
        player = GameObject.FindWithTag("MainPlayer");
        hud = player.GetComponent<HUD>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    
    public void OnPointerEnter(PointerEventData eventData){
        //If your mouse hovers over the GameObject with the script attached, output this message
        if (infoStr == default(string)) {
            Debug.LogError("Info string was never initialized");
        }
        else {
            hud.InfoStr = infoStr;
        }
    }

    public void OnPointerExit(PointerEventData eventData){
        hud.InfoStr = "";
    }
    
    public void OnDisable() {
        if (hud == null) {
            return; //Means that start was never called, so we don't have to worry
        }
        if (hud.InfoStr == infoStr) {
            hud.InfoStr = "";
        }
    }
}
