using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodBehavior : MonoBehaviour, ItemActivator {


    private GameObject player;
    private string item;
    
    public int deltaHunger;
    public int deltaThirst;
    
	// Use this for initialization
	void Start () {
		player = GameObject.FindWithTag("MainPlayer");
        item = gameObject.name;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    void GetEaten() {
        player.GetComponent<PlayerBehavior>().HungerLevel += deltaHunger;
        player.GetComponent<PlayerBehavior>().ThirstLevel += deltaThirst;
        player.GetComponent<PlayerBehavior>().Hud.Inventory.RemoveItem(item);
    }
    public void ActivateItem() {
        GetEaten();
    }
}
