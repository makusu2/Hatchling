using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEngine.UI;

public class Inventory : MonoBehaviour {

    private Dictionary<string,int> counts = new Dictionary<string,int>();
    private Dictionary<string,GameObject> visibleBoxes = new Dictionary<string,GameObject>();
    
    private GameObject player;
    private GameObject inventoryPanel;
    
	// Use this for initialization
	void Start () {
        //GameObject inventoryContainer = transform.parent.gameObject;
		player = transform.parent.gameObject;
        inventoryPanel = GameObject.Find("InventoryPanel").gameObject;
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
            //box.transform.GetComponent<Text>().transform.SetParent(box.transform,false);
            visibleBoxes[name] = box;
        }
        //print("Count: "+counts[name]);
        visibleBoxes[name].transform.Find("Text").GetComponent<Text>().text = ""+counts[name];
        
        /*else {
            //make the sprite
        }*/
    }
    
    Sprite GetSprite(string name) {
        //[MenuItem("AssetDatabase/LoadAssetExample")]
        Sprite t = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/Resources/InventoryIcons/"+name+".png", typeof(Sprite));
        return t;
    }
    GameObject GetBox(string name) {
        GameObject box = Instantiate(Resources.Load("InventoryIcons/BoxFab") as GameObject);
        Sprite spr = GetSprite(name);
        box.GetComponent<Image>().sprite = spr;
        return box;
    }
}
