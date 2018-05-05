using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TraderSelection : MonoBehaviour {

    private GameObject player;
    private PlayerBehavior pb;
    //private HUD hud;
    private TraderBehavior traderBehavior;
    
    private string itemName;
    private int coinValue;
    private bool playerIsBuying;
    
    
    public string ButtonText {
        set {
            transform.Find("Text").GetComponent<Text>().text = value;
        }
    }
    
    
    void Awake() {
        player = GameObject.FindWithTag("MainPlayer");
        pb = player.GetComponent<PlayerBehavior>();
        //hud = player.GetComponent<HUD>();
    }
    
    public void Setup(string itemName, int coinValue, bool playerIsBuying, TraderBehavior traderBehavior) {
        this.itemName = itemName;
        this.coinValue = coinValue;
        this.playerIsBuying = playerIsBuying;
        this.traderBehavior = traderBehavior;
        
        if(playerIsBuying) {
            ButtonText = "Spend "+coinValue.ToString()+" coins for "+itemName;
        }
        else {
            ButtonText = "Sell "+itemName+" for "+coinValue.ToString()+" coins";
        }
    }
    
    
    public bool PlayerCanTrade() {
        return !((playerIsBuying && player.GetComponent<PlayerBehavior>().Money < coinValue) || (!playerIsBuying && player.GetComponent<PlayerBehavior>().inventory.CountOf(itemName) < 1));
    }
    
    public void ClickedOn() {
        if (!PlayerCanTrade()) {
            Debug.LogError("Player was able to click on trade button but doesn't have the stuff necessary");
        }
        if(playerIsBuying) {
            pb.Money -= coinValue;
            pb.inventory.AddItem(itemName);
        }
        else {
            pb.inventory.RemoveItem(itemName);
            pb.Money += coinValue;
        }
        pb.PlayFXAudio(pb.TradingSound);
        traderBehavior.UpdateStock();
    }
}
