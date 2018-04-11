using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.IO;

public class TownBehavior : MonoBehaviour, ItemHolderInt{
    
    private GameObject player;
    private HUD hud;
    private Inventory inv;
    
    private Dictionary<string,int> itemCounts = new Dictionary<string,int>();
    private Dictionary<string,GameObject> visibleBoxes = new Dictionary<string,GameObject>();
    
    private Dictionary<string,Dictionary<string,int>> recipes;
    private Dictionary<string,GameObject> buildingContainers = new Dictionary<string,GameObject>();
    private Dictionary<string,int> maxCount = new Dictionary<string,int>();
    private Dictionary<string,int> currentCount = new Dictionary<string,int>();
    
    private int updateDelay = 2;
    
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
        
        recipes = LoadTownRecipes();
        foreach(string key in recipes.Keys) {
            buildingContainers[key] = transform.Find(key+"s").gameObject;
            currentCount[key] = 0;
            maxCount[key] = 0;
            foreach(Transform child in buildingContainers[key].transform) {
                child.gameObject.SetActive(false);
                maxCount[key]++;
            }
        }
        
    }
    
	// Use this for initialization
	void Start () {
        hud = player.GetComponent<HUD>();
        inv = hud.Inventory;
        Invoke("UseResources",updateDelay);
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
        box.transform.SetParent(hud.TownPanel.transform,false);
        box.transform.GetComponent<InventoryIconBehavior>().Item = name;
        visibleBoxes[name] = box;
    }
    
    void UseResources() {
        string[] possibleBuilds = GetPossibleBuilds();
        if (possibleBuilds.Length > 0) {
            System.Random rnd = new System.Random();
            string chosenBuild = possibleBuilds[rnd.Next(possibleBuilds.Length)];
            Build(chosenBuild);
        }
        Invoke("UseResources",updateDelay);
    }
    
    void Build(string build) {
        foreach(KeyValuePair<string,int> ingredientPair in recipes[build]) {
            string ingredient = ingredientPair.Key;
            int count = ingredientPair.Value;
                for (int i=0;i<count;i++) {
                    RemoveItem(ingredient);
                }
        }
        foreach(Transform child in buildingContainers[build].transform) {
            if (!child.gameObject.activeInHierarchy) {
                child.gameObject.SetActive(true);
                break;
            }
        }
        currentCount[build]++;
    }
    
    
    public bool CanBuildItem(string possibleBuild) {
        if (currentCount[possibleBuild] >= maxCount[possibleBuild]) {
            return false;
        }
        Dictionary<string,int> ingredients = recipes[possibleBuild];
        foreach(KeyValuePair<string,int> ingredientPair in ingredients) {
            string ingredient = ingredientPair.Key;
            int count = ingredientPair.Value;
            if(CountOf(ingredient) < count) {
                return false;
            }
        }
        return true;
    }
    
    public string[] GetPossibleBuilds() {
        List<string> possibleBuilds = new List<string>();
        foreach(string possibleBuild in recipes.Keys) {
            if(CanBuildItem(possibleBuild)) {
                possibleBuilds.Add(possibleBuild);
            }
        }
        return possibleBuilds.ToArray();
    }
    
    static Dictionary<string,Dictionary<string,int>> LoadTownRecipes() {
        return MakuUtil.LoadRecipeFile("Assets/SettingsFiles/TownRecipes.txt");
    }
}
