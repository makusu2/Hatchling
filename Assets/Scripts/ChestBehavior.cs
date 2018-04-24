using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Linq;

public class ChestBehavior : MonoBehaviour, ContainerInt {


    
    
    
    
    private GameObject player;
    private HUD hud;
    private Inventory inv;
    
    private Dictionary<string,int> itemCounts = new Dictionary<string,int>();
    private Dictionary<string,GameObject> visibleBoxes = new Dictionary<string,GameObject>();
    
    
    
    
    public int CountOf(string item) {
        try {
            return itemCounts[item];
        }
        catch(KeyNotFoundException){
            return 0;
        }
    }
    
    
    
    void Awake() {
		player = GameObject.FindWithTag("MainPlayer");
    }
    
    
	// Use this for initialization
	void Start () {
        hud = player.GetComponent<HUD>();
        inv = hud.Inventory;
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    
    
    public void ItemMovedTo(string item) {
        int numItemsToMove = inv.CountOf(item);//TODO ask user how many to move
        for (int i=0;i<numItemsToMove;i++) {
            inv.RemoveItem(item);
            this.AddItem(item);
        }
    }
    
    public void ItemTakenFrom(string item) {
        int numItemsToMove = CountOf(item);
        for (int i=0;i<numItemsToMove;i++) {
            inv.AddItem(item);
            this.RemoveItem(item);
        }
    }
    
    
    public void CreateIcons() {
        DestroyIcons();
        foreach(string item in itemCounts.Keys) {
            AddBox(item);
            //itemCounts[item] = itemCounts[item];
            visibleBoxes[item].transform.Find("Text").GetComponent<Text>().text = ""+itemCounts[item];
        }
    }
    public void DestroyIcons() {
        string[] items = visibleBoxes.Keys.ToArray();
        foreach(string item in items) {
            GameObject icon = visibleBoxes[item];
            Destroy(icon);
            visibleBoxes.Remove(item);
        }
        //visibleBoxes = new Dictionary<string,GameObject>();
    }
    
    
    
    public void AddItem(string name) {
        if (!itemCounts.ContainsKey(name)) {
            itemCounts[name] = 0;
        }
        itemCounts[name] += 1;
        if(!visibleBoxes.ContainsKey(name)) {
            AddBox(name);
        }
        visibleBoxes[name].transform.Find("Text").GetComponent<Text>().text = ""+itemCounts[name];
    }
    
    public void RemoveItem(string name) {
        try {
            itemCounts[name] -= 1;
            visibleBoxes[name].transform.Find("Text").GetComponent<Text>().text = ""+itemCounts[name];
            if(itemCounts[name] == 0) {
                itemCounts.Remove(name);
                Destroy(visibleBoxes[name]);
                visibleBoxes.Remove(name);
            }
        }
        catch (KeyNotFoundException) {
            Debug.Log("Attempted to remove item "+name+", but did not have any");
        }
        
    }
    
    Sprite GetSprite(string name) {
        Sprite t = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/Resources/InventoryIcons/"+name+".png", typeof(Sprite));
        if (t == null) {
            Debug.LogError("Could not find sprite for "+name);
            t = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/Resources/InventoryIcons/unknown.png", typeof(Sprite)); //Draw an unknown thing
        }
        return t;
    }
    
    void AddBox(string name){
        GameObject box = Instantiate(Resources.Load("InventoryIcons/BoxFab") as GameObject);
        Sprite spr = GetSprite(name);
        box.GetComponent<Image>().sprite = spr;
        box.name = name+"Box";
        box.transform.GetComponent<InfoHover>().infoStr = name;
        box.transform.SetParent(hud.ChestPanel.transform,false);
        box.transform.GetComponent<InventoryIconBehavior>().Item = name;
        visibleBoxes[name] = box;
    }
    
    
    
    void GetClickedOn(GameObject player) {
        hud.CurrentChest = this.gameObject;
        hud.ChestMenuOpen = true;
    }
}
