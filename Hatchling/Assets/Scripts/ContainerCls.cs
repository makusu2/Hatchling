using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEditor;

public class ContainerCls : MonoBehaviour, ContainerInt{
    private Dictionary<string,int> itemCounts = new Dictionary<string,int>();
    private Dictionary<string,GameObject> visibleBoxes = new Dictionary<string,GameObject>();
    
    protected static GameObject player;
    protected static HUD hud;
    
    void Start() {//
        Prepare();
    }
    public void Prepare() { 
        player = player??GameObject.FindWithTag("MainPlayer");
        hud = hud??player.GetComponent<HUD>();
    
    }
    
    
    public int CountOf(string item) {
        try {
            return itemCounts[item];
        }
        catch(KeyNotFoundException){
            return 0;
        }
    }
    
    
    public void ItemMovedTo(string item) {
        int numItemsToMove = hud.Inventory.CountOf(item);//TODO ask user how many to move
        for (int i=0;i<numItemsToMove;i++) {
            hud.Inventory.RemoveItem(item);
            this.AddItem(item);
        }
    }
    
    public void ItemTakenFrom(string item) {
        int numItemsToMove = CountOf(item);
        for (int i=0;i<numItemsToMove;i++) {
            hud.Inventory.AddItem(item);
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
    
    
    
    public void PrepareEntityInventory(string loot) {
        string[] components = loot.Split(',');
        foreach(string comp in components) {
            int count = int.Parse(comp.Split('*')[0]);
            string compMat = comp.Split('*')[1];
            for (int i=0;i<count;i++) {
                AddItem(compMat);
            }
        }
        DestroyIcons();
    }
}
