using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEngine.UI;
using System.IO;
using System;

public class Inventory : MonoBehaviour {
    

    private Dictionary<string,int> counts = new Dictionary<string,int>();
    private Dictionary<string,GameObject> visibleBoxes = new Dictionary<string,GameObject>();
    
    private GameObject player;
    
    private HUD hud;
    
    private Dictionary<string,Dictionary<string,int>> recipes;
    
    private int currentlySelectedSlot;
    public int CurrentlySelectedSlot {
        get { return currentlySelectedSlot;}
        set {
            string[] itemKeys = visibleBoxes.Keys.ToArray();
            
            int numSlots = itemKeys.Length;
            if(numSlots>0) {
                currentlySelectedSlot = value % numSlots;
                string relevantItem = visibleBoxes.Keys.ToArray()[currentlySelectedSlot];
                foreach(string key in visibleBoxes.Keys) {
                    if (key.Equals(relevantItem)) {
                        visibleBoxes[key].GetComponent<Image>().color = Color.yellow;
                    }
                    else {
                        visibleBoxes[key].GetComponent<Image>().color = Color.white;
                    }
                } 
            }
            else {
                CurrentlySelectedItem = "Hands";
            }
        }
    }
    
    public string CurrentlySelectedItem {
        get { 
            try {
                return visibleBoxes.Keys.ToArray()[CurrentlySelectedSlot]; 
            }
            catch(IndexOutOfRangeException) {
                return "Hands";
            }
        }
        set { 
            try {
                CurrentlySelectedSlot = Array.IndexOf(visibleBoxes.Keys.ToArray(),value);
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
        recipes = LoadCraftingRecipes();
        hud = player.GetComponent<HUD>();
        CloseInventoryMenu();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    
    public void AddItem(string name) {
        
        if (!counts.ContainsKey(name)) {
            counts[name] = 0;
        }
        counts[name] += 1;
        if(!visibleBoxes.ContainsKey(name)) {
            AddBox(name);
        }
        visibleBoxes[name].transform.Find("Text").GetComponent<Text>().text = ""+counts[name];
    }
    
    public void RemoveItem(string name) {
        try {
            counts[name] -= 1;
            visibleBoxes[name].transform.Find("Text").GetComponent<Text>().text = ""+counts[name];
            if(counts[name] == 0) {
                counts.Remove(name);
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
        //[MenuItem("AssetDatabase/LoadAssetExample")]
        Sprite t = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/Resources/InventoryIcons/"+name+".png", typeof(Sprite));
        if(t == null) {
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
        visibleBoxes[name] = box;
        if(visibleBoxes.Keys.ToArray().Length == 1) {
            CurrentlySelectedSlot = 0;
        }
        //return box;
    }
    
    public void CraftItem(string name) {
        Dictionary<string,int> ingredients = recipes[name];
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
    
    public string[] GetPossibleCrafts() {
        List<string> possibleCrafts = new List<string>();
        foreach(string possibleCraft in recipes.Keys) {
            if(CanCraftItem(possibleCraft)) {
                possibleCrafts.Add(possibleCraft);
            }
        }
        return possibleCrafts.ToArray();
    }
        
    public bool CanCraftItem(string possibleCraft) {
        Dictionary<string,int> ingredients = recipes[possibleCraft];
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
    public void OpenInventoryMenu() {
        //Update the crafting list
        hud.CursorFree = true;
        hud.FullInventoryPanel.SetActive(true);
        UpdateCraftingRecipes();
    }
    
    public void CloseInventoryMenu() {
        hud.CursorFree = false;
        hud.FullInventoryPanel.SetActive(false);
    }
    
    public void ToggleInventoryMenu() {
        if(hud.FullInventoryPanel.activeInHierarchy) {
            CloseInventoryMenu();
        }
        else {
            OpenInventoryMenu();
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
}