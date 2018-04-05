using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class TraderBehavior : MonoBehaviour {

    private static int numItemsToSell = 5;
    private static int numItemsToBuy = 5;
    
    public Dictionary<string,int> SellValues;
    public Dictionary<string,int> BuyValues;
    private GameObject[] traderSelectionButtons;
    
    private HUD hud;
    
    private static System.Random rnd = new System.Random();
    
    void Awake() {
        hud = GameObject.FindWithTag("MainPlayer").GetComponent<HUD>();
    }
	// Use this for initialization
	void Start () {
		PopulateSellBuyValues();
        //CreateButtons();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    
    void PopulateSellBuyValues() {
        SellValues = new Dictionary<string,int>();
        BuyValues = new Dictionary<string,int>();
        float transRatio = 2.0f; //If it costs 10 and the ratio is 2, buying will be 20 and selling will be 5
        if(numItemsToSell+numItemsToBuy > CoinValue.Count) {
            Debug.LogError("Not enough possible transactions to fill all sell and buy values");
        }
        List<string> usedItems = new List<string>();
        
        while (SellValues.Count < numItemsToSell) {
            string randItem = randomItem();
            if(!usedItems.Contains(randItem)) {
                usedItems.Add(randItem);
                SellValues[randItem] = (int)((float)CoinValue[randItem] / transRatio);
            }
        }
        while(BuyValues.Count < numItemsToBuy) {
            string randItem = randomItem();
            if(!usedItems.Contains(randItem)) {
                usedItems.Add(randItem);
                BuyValues[randItem] = (int)((float)CoinValue[randItem] * transRatio);
            }
        }
    }
    
    public void CreateButtons() {
        DestroyButtons(); //just in case
        traderSelectionButtons = new GameObject[numItemsToSell+numItemsToBuy];
        int currentTraderSelectionButtonsIndex = 0;
        foreach(string item in SellValues.Keys) {
            traderSelectionButtons[currentTraderSelectionButtonsIndex] = GetTraderSelectionButton(item,SellValues[item],false);
            currentTraderSelectionButtonsIndex += 1;
        }
        foreach(string item in BuyValues.Keys) {
            traderSelectionButtons[currentTraderSelectionButtonsIndex] = GetTraderSelectionButton(item,BuyValues[item],true);
            currentTraderSelectionButtonsIndex += 1;
            
        }
        UpdateStock();
    }
    public void DestroyButtons() {
        if (traderSelectionButtons == null) {
            return;
        }
        foreach(GameObject but in traderSelectionButtons) {
            Destroy(but);
        }
        traderSelectionButtons = null;
    }
    
    GameObject GetTraderSelectionButton(string itemName, int coinValue, bool playerIsBuying) {
        GameObject but = Instantiate(Resources.Load("TraderSelectionButton") as GameObject);
        but.transform.SetParent(hud.TraderPanel.transform,false);
        but.GetComponent<TraderSelection>().Setup(itemName,coinValue,playerIsBuying,this);
        return but;
    }
    
    public void UpdateStock() {
        foreach(GameObject but in traderSelectionButtons) {
            TraderSelection butBehavior = but.GetComponent<TraderSelection>();
            but.GetComponent<Button>().interactable = butBehavior.PlayerCanTrade();
        }
    }
    
    void GetClickedOn(GameObject player) {
        hud.CurrentTrader = gameObject;
        hud.TraderMenuOpen = true;
    }
    
    static string randomItem() {
        return CoinValue.Keys.ToArray()[rnd.Next(CoinValue.Count)];
    }
    public static Dictionary<string,int> CoinValue = new Dictionary<string,int>() {
        {"Apple",5},
        {"Battery",30},
        {"BoltCutter",70},
        {"Bottle",5},
        {"Bow",150},
        {"Branch",2},
        {"Bread",20},
        {"Bucket",20},
        {"Canteen",10},
        {"Coconut",15},
        {"CookedMeat",15},
        {"UncookedMeat",15},
        {"DuctTape",15},
        {"Flashlight",30},
        {"Hammer",30},
        {"Hatchet",15},
        {"Knife",15},
        {"Lighter",15},
        {"Machete",15},
        {"Medpack",15},
        {"Orange",10},
        {"Pickaxe",15},
        {"Rifle",500},
        {"Rope",10},
        {"Shovel",15},
        {"Spear",15},
        {"Steel",70},
        {"Stone",10},
        {"Toolkit",150},
        {"Torch",10},
        {"Wood",10}
    };
}
