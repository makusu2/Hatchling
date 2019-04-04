using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Linq;

public class ChestBehavior : MonoBehaviour, ContainerInt {

    private GameObject player;
    private HUD hud;
    
    protected ContainerCls container;

    public int CountOf(string item) { return container.CountOf(item);}
    public void ItemMovedTo(string item) { container.ItemMovedTo(item);}
    public void ItemTakenFrom(string item) { container.ItemTakenFrom(item);}
    public void CreateIcons() { container.CreateIcons();}
    public void DestroyIcons() { container.DestroyIcons();}
    public void AddItem(string name) { container.AddItem(name);}
    public void RemoveItem(string name) { container.RemoveItem(name);}
    public void PrepareEntityInventory(string loot) { container.PrepareEntityInventory(loot);}
    
    
    void Start() {
        container = gameObject.AddComponent<ContainerCls>();
        container.Prepare();
        player = player??GameObject.FindWithTag("MainPlayer");
        hud = hud??player.GetComponent<HUD>();
        
    }
    
    
    
    
    void GetClickedOn(GameObject player) {
        hud.CurrentChest = this.gameObject;
        hud.ChestMenuOpen = true;
    }
}
