﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEngine.UI;
using System.IO;
using System;
using System.Collections.Specialized;

public class Inventory : MonoBehaviour {
    

    private Dictionary<string,int> counts = new Dictionary<string,int>();
    private Dictionary<string,GameObject> visibleBoxes = new Dictionary<string,GameObject>();
    private LinkedList<string> itemOrder = new LinkedList<string>();
    
    private GameObject player;
    
    private HUD hud;
    
    private Dictionary<string,Dictionary<string,int>> craftingRecipes;
    private Dictionary<string,Dictionary<string,int>> buildingRecipes;
    
    [System.NonSerialized]
    public GameObject PrepareBuildObject;
    [System.NonSerialized]
    public float PrepareBuildOffset;
    [System.NonSerialized]
    public bool PreparingBuild = false;
    
    public readonly int maxNumItems = 5;
    
    public int CurrentNumItems {
        get {
            return itemOrder.ToArray().Length;
        }
    }
    
    private int currentlySelectedSlot;
    public int CurrentlySelectedSlot {
        get { return currentlySelectedSlot;}
        set {
            string relevantItem = "None";
            
            string[] itemKeys = itemOrder.ToArray();
            
            int numSlots = itemKeys.Length;
            
            
            if(numSlots>0) {
                currentlySelectedSlot = value % numSlots;
                relevantItem = itemOrder.ToArray()[currentlySelectedSlot];
                foreach(string key in itemOrder) {
                    if (key.Equals(relevantItem)) {
                        visibleBoxes[key].GetComponent<Image>().color = Color.yellow;
                    }
                    else {
                        visibleBoxes[key].GetComponent<Image>().color = Color.white;
                    }
                } 
            }
            player.GetComponent<PlayerBehavior>().SetEquippedItem(relevantItem);
        }
    }
    
    public void MoveToExtraInventory(string name) {
        visibleBoxes[name].transform.SetParent(hud.ExtraInventoryPanel.transform);
        itemOrder.Remove(name);
        SettleSelectedItem();
    }
    
    public void MoveToNormalInventory(string name) {
        if (CurrentNumItems >= maxNumItems) {
            Debug.LogError("Tried to move "+name+" to normal inventory but it was full");
            return;
        }
        visibleBoxes[name].transform.SetParent(hud.InventoryPanel.transform);
        itemOrder.AddLast(name);
        SettleSelectedItem();
    }
    
    public string CurrentlySelectedItem {
        get { 
            try {
                return itemOrder.ToArray()[CurrentlySelectedSlot]; 
            }
            catch(IndexOutOfRangeException) {
                return "Hands";
            }
        }
        set { 
            if (value.Equals("Hands")) {
                CurrentlySelectedSlot = 0;
                return;
            }
            try {
                CurrentlySelectedSlot = Array.IndexOf(itemOrder.ToArray(),value);
            }
            catch(KeyNotFoundException) {//
                Debug.LogError("Tried to select "+value+" on the inventory bar but it was not found");
            }
        }
    }
    
    public void SettleSelectedItem() {
        CurrentlySelectedSlot = currentlySelectedSlot;
    }
    
	// Use this for initialization
	void Start () {
        player = GameObject.FindWithTag("MainPlayer");
        craftingRecipes = LoadCraftingRecipes();
        buildingRecipes = LoadBuildingRecipes();
        hud = player.GetComponent<HUD>();
        hud.InventoryMenuOpen = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    
    public void AddItem(string name) {
        if (!itemOrder.Contains(name)) {
            itemOrder.AddLast(name);
        }
        if (!counts.ContainsKey(name)) {
            counts[name] = 0;
        }
        counts[name] += 1;
        if(!visibleBoxes.ContainsKey(name)) {
            AddBox(name);
        }
        visibleBoxes[name].transform.Find("Text").GetComponent<Text>().text = ""+counts[name];
        
        if (itemOrder.ToArray().Length == 1) {
            
            CurrentlySelectedSlot = 0;
        }
        if (itemOrder.ToArray().Length > maxNumItems) {
            MoveToExtraInventory(name);
        }
    }
    
    public void RemoveItem(string name) {
        try {
            counts[name] -= 1;
            visibleBoxes[name].transform.Find("Text").GetComponent<Text>().text = ""+counts[name];
            if(counts[name] == 0) {
                counts.Remove(name);
                itemOrder.Remove(name);
                Destroy(visibleBoxes[name]);
                visibleBoxes.Remove(name);
            }
        }
        catch (KeyNotFoundException) {
            Debug.Log("Attempted to remove item "+name+", but did not have any");
        }
        SettleSelectedItem();
        
    }
    
    Sprite GetSprite(string name) {
        Sprite t = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/Resources/InventoryIcons/"+name+".png", typeof(Sprite));
        if(t == null) {
            t = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/Resources/BuildingIcons/"+name+".png", typeof(Sprite)); //not currently using
        }
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
        box.transform.SetParent(hud.InventoryPanel.transform,false);
        box.transform.GetComponent<InventoryIconBehavior>().Item = name;
        visibleBoxes[name] = box;
    }
    
    /*void AddBuildingBox(string name) {
        GameObject box = Instantiate(Resources.Load("InventoryIcons/BoxFab") as GameObject);
        Sprite spr = GetSprite(name);
        box.GetComponent<Image>().sprite = spr;
        box.name = name+"Box";
        box.transform.GetComponent<InfoHover>().infoStr = name;
        box.transform.SetParent(hud.BuildingPanel.transform,false);
        visibleBoxes[name] = box;
        
    }*/
    
    public void CraftItem(string name) {
        Dictionary<string,int> ingredients = craftingRecipes[name];
        foreach(KeyValuePair<string,int> ingredientPair in ingredients) {
            string ingredient = ingredientPair.Key;
            int count = ingredientPair.Value;
                for (int i=0;i<count;i++) {
                    RemoveItem(ingredient);
                }
        }
        AddItem(name);
        UpdateCraftingRecipes();
    }
    
    
    
    
    
    
    public void PrepareBuildItem(string name) {
        hud.BuildingMenuOpen = false;
        try {
            GameObject prepareBuildTemplate = Resources.Load(name) as GameObject; //Have to check bounds from this since they don't get updated in the instantiated version
            PrepareBuildObject = Instantiate(prepareBuildTemplate);//load object from resources
            PrepareBuildOffset = prepareBuildTemplate.GetComponent<Collider>().bounds.min.y;
        }
        catch(NullReferenceException) {
            Debug.Log("Tried to prepare build "+name+" but couldn't find it in resources");
            return;
        }
        PrepareBuildObject.GetComponent<Collider>().enabled = false;
        PreparingBuild = true;
        //the rest should be taken care of in player's fixedupdate
    }
    public void CompleteBuildItem() {
        PreparingBuild = false;
        PrepareBuildObject.GetComponent<Collider>().enabled = true;
        //might need to do more here
    }
    
    public string[] GetPossibleCrafts() {
        List<string> possibleCrafts = new List<string>();
        foreach(string possibleCraft in craftingRecipes.Keys) {
            if(CanCraftItem(possibleCraft)) {
                possibleCrafts.Add(possibleCraft);
            }
        }
        return possibleCrafts.ToArray();
    }
    public string[] GetPossibleBuilds() {
        List<string> possibleBuilds = new List<string>();
        foreach(string possibleBuild in buildingRecipes.Keys) {
            if(CanBuildItem(possibleBuild)) {
                possibleBuilds.Add(possibleBuild);
            }
        }
        return possibleBuilds.ToArray();
    }
        
    public bool CanCraftItem(string possibleCraft) {
        Dictionary<string,int> ingredients = craftingRecipes[possibleCraft];
        foreach(KeyValuePair<string,int> ingredientPair in ingredients) {
            string ingredient = ingredientPair.Key;
            int count = ingredientPair.Value;
            try {
                if(counts[ingredient] < count) {
                    return false;
                }
            }
            catch(KeyNotFoundException) { //item isn't in inventory, therefore player has 0
                return false;
            }
        }
        return true;
    }
    public bool CanBuildItem(string possibleBuild) {
        Dictionary<string,int> ingredients = buildingRecipes[possibleBuild];
        foreach(KeyValuePair<string,int> ingredientPair in ingredients) {
            string ingredient = ingredientPair.Key;
            int count = ingredientPair.Value;
            try {
                if(counts[ingredient] < count) {
                    return false;
                }
            }
            catch(KeyNotFoundException) { //item isn't in inventory, therefore player has 0
                return false;
            }
        }
        return true;
    }
        
    public void UpdateCraftingRecipes() {
        foreach(Transform child in hud.CraftingPanel.transform) {
            Debug.Assert(child.GetComponent<Button>() != null);
            Destroy(child.gameObject);
        }
        string[] possibleCrafts = GetPossibleCrafts();
        foreach(string possibleCraft in possibleCrafts) {
            GameObject possibleCraftButton = Instantiate(Resources.Load("CraftingSelectionButton") as GameObject);
            possibleCraftButton.transform.SetParent(hud.CraftingPanel.transform,false);
            possibleCraftButton.name = "Craft"+possibleCraft+"Button";
            possibleCraftButton.transform.Find("Text").GetComponent<Text>().text = possibleCraft;
            possibleCraftButton.GetComponent<CraftingSelection>().itemName = possibleCraft;
        }
    }
    public void UpdateBuildingRecipes() {
        foreach(Transform child in hud.BuildingPanel.transform) {
            Debug.Assert(child.GetComponent<Button>() != null);
            Destroy(child.gameObject);
        }
        string[] possibleBuilds = GetPossibleBuilds();
        foreach(string possibleBuild in possibleBuilds) {
            GameObject possibleBuildButton = Instantiate(Resources.Load("BuildingSelectionButton") as GameObject);
            possibleBuildButton.transform.SetParent(hud.BuildingPanel.transform,false);
            possibleBuildButton.name = "Build"+possibleBuild+"Button";
            possibleBuildButton.transform.Find("Text").GetComponent<Text>().text = possibleBuild;
            possibleBuildButton.GetComponent<BuildingSelection>().itemName = possibleBuild;
        }
    }
    
    static Dictionary<string,Dictionary<string,int>> LoadCraftingRecipes() {
        Dictionary<string,Dictionary<string,int>> recipes = new Dictionary<string,Dictionary<string,int>>();
        string path = "Assets/SettingsFiles/CraftingRecipes.txt";
        StreamReader reader = new StreamReader(path); 
        string currentLine;//
        while(true){
            currentLine = reader.ReadLine();
            if(currentLine != null){
                string toCreate = currentLine.Split('=')[0];
                string remainder = currentLine.Split('=')[1];
                string[] components = remainder.Split(',');
                recipes[toCreate] = new Dictionary<string,int>();
                for(int i=0;i<components.Length;i++) {
                    string comp = components[i];
                    int count = int.Parse(comp.Split('*')[0]);
                    string compMat = comp.Split('*')[1];
                    recipes[toCreate][compMat] = count;
                }
            }
            else{
                break;
            }
        }
        reader.Close();
        return(recipes);
    }
    static Dictionary<string,Dictionary<string,int>> LoadBuildingRecipes() {
        Dictionary<string,Dictionary<string,int>> recipes = new Dictionary<string,Dictionary<string,int>>();
        string path = "Assets/SettingsFiles/BuildingRecipes.txt";
        StreamReader reader = new StreamReader(path); 
        string currentLine;//
        while(true){
            currentLine = reader.ReadLine();
            if(currentLine != null){
                string toCreate = currentLine.Split('=')[0];
                string remainder = currentLine.Split('=')[1];
                string[] components = remainder.Split(',');
                recipes[toCreate] = new Dictionary<string,int>();
                for(int i=0;i<components.Length;i++) {
                    string comp = components[i];
                    int count = int.Parse(comp.Split('*')[0]);
                    string compMat = comp.Split('*')[1];
                    recipes[toCreate][compMat] = count;
                }
            }
            else{
                break;
            }
        }
        reader.Close();
        return(recipes);
    }
}