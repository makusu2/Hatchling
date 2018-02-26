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
    private GameObject inventoryPanel;
    private GameObject fullInventoryPanel;
    private GameObject craftingPanel;
    private GameObject FPC;
    
    private Dictionary<string,Dictionary<string,int>> recipes;
    
    //public string CurrentlySelectedItem = "Hands";
    
    private int currentlySelectedSlot;
    public int CurrentlySelectedSlot {
        get { return currentlySelectedSlot;}
        set {
            string[] itemKeys = visibleBoxes.Keys.ToArray();
            
            int numSlots = itemKeys.Length;
            if(numSlots>0) {
                currentlySelectedSlot = value % numSlots;
                //visibleBoxes.Keys.ToArray()[currentlySelectedSlot];
                //CurrentlySelectedItem = visibleBoxes.Keys.ToArray()[currentlySelectedSlot];
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
        get { return visibleBoxes.Keys.ToArray()[CurrentlySelectedSlot]; }
        set { 
            try {
                CurrentlySelectedSlot = Array.IndexOf(visibleBoxes.Keys.ToArray(),value);
            }
            catch(KeyNotFoundException e) {
                Debug.LogError("Tried to select "+value+" on the inventory bar but it was not found");
            }
        }
    }
        
    //public Crafting crafter;
    
    
	// Use this for initialization
	void Start () {
        //GameObject inventoryContainer = transform.parent.gameObject;
		player = transform.parent.gameObject;
        //player = MainPlayer.gameObject;
        inventoryPanel = GameObject.Find("InventoryPanel").gameObject;
        fullInventoryPanel = GameObject.Find("FullInventoryPanel").gameObject;
        //crafter = new Crafting(this);
        recipes = LoadCraftingRecipes();
        FPC = GameObject.Find("FPSController");
        craftingPanel = fullInventoryPanel.transform.Find("CraftingPanel").gameObject;
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
            GameObject box = GetBox(name);
            box.name = name+"Box";
            box.transform.SetParent(inventoryPanel.transform,false);
            visibleBoxes[name] = box;
            if(visibleBoxes.Keys.ToArray().Length == 1) {
                CurrentlySelectedSlot = 0;
            }
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
        catch (KeyNotFoundException k) {
            Debug.Log("Attempted to remove item "+name+", but did not have any");
        }
        
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
    
    GameObject GetBox(string name){
        GameObject box = Instantiate(Resources.Load("InventoryIcons/BoxFab") as GameObject);
        Sprite spr = GetSprite(name);
        box.GetComponent<Image>().sprite = spr;
        return box;
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
        foreach(Transform child in craftingPanel.transform) {
            Debug.Assert(child.GetComponent<Button>() != null);
            Destroy(child.gameObject);
        }
        string[] possibleCrafts = GetPossibleCrafts();
        foreach(string possibleCraft in possibleCrafts) {
            GameObject possibleCraftButton = Instantiate(Resources.Load("CraftingSelectionButton") as GameObject);
            possibleCraftButton.transform.SetParent(craftingPanel.transform,false);
            possibleCraftButton.name = "Craft"+possibleCraft+"Button";
            possibleCraftButton.transform.Find("Text").GetComponent<Text>().text = possibleCraft;
            possibleCraftButton.GetComponent<CraftingSelection>().itemName = possibleCraft;
        }
        
    }
    public void OpenInventoryMenu() {
        //Update the crafting list
        fullInventoryPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        FPC.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController> ().m_MouseLook.lockCursor = false;
        UpdateCraftingRecipes();
    }
    
    public void CloseInventoryMenu() {
        fullInventoryPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        FPC.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController> ().m_MouseLook.lockCursor = true;
    }
    
    public void ToggleInventoryMenu() {
        if(fullInventoryPanel.activeInHierarchy) {
            CloseInventoryMenu();
        }
        else {
            OpenInventoryMenu();
        }
    }
    
    Dictionary<string,Dictionary<string,int>> LoadCraftingRecipes() {
        Dictionary<string,Dictionary<string,int>> recipes = new Dictionary<string,Dictionary<string,int>>();
        string path = "Assets/SettingsFiles/CraftingRecipes.txt";
        StreamReader reader = new StreamReader(path); 
        string currentLine;
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