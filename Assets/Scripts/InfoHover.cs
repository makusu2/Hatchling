using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InfoHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public string infoStr;
    
    private GameObject player;
    
	// Use this for initialization
	void Start () {
        player = GameObject.FindWithTag("MainPlayer");
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
            player.GetComponent<HUD>().InfoStr = infoStr;
        }
    }

    public void OnPointerExit(PointerEventData eventData){
        player.GetComponent<HUD>().InfoStr = "";
    }
}
