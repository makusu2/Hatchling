using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using System;

public class InventoryIconBehavior : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler, IPointerClickHandler{

    private GameObject player;
    
    private HUD hud;
    
    private GameObject originalPanel;
    private Vector3 originalPosition;
    private GameObject inventoryPanel, extraPanel, townPanel, chestPanel;
    
    public string Item;
    
    
    public void OnBeginDrag(PointerEventData eventData) {
        originalPosition = transform.position;
        originalPanel = GetCurrentPanel();
    }
    
    public void OnDrag(PointerEventData eventData) {
        transform.position = Input.mousePosition;//rayPoint;
    }
    
    
    public void OnPointerClick(PointerEventData pointerEventData) {
        if(Input.GetKey(KeyCode.LeftShift)) {
            GameObject currentPanel = GetCurrentPanel();
            if ((currentPanel == inventoryPanel || currentPanel == extraPanel) && hud.TownMenuOpen) {
                hud.Town.GetComponent<TownBehavior>().ItemMovedTo(Item);
            }
            else if ((currentPanel == inventoryPanel || currentPanel == extraPanel) && hud.ChestMenuOpen) {
                hud.CurrentChest.GetComponent<ChestBehavior>().ItemMovedTo(Item);
            }
            else if (currentPanel == chestPanel) {
                hud.CurrentChest.GetComponent<ChestBehavior>().ItemTakenFrom(Item);
            }
            else if (currentPanel == inventoryPanel) {
                player.GetComponent<PlayerBehavior>().inventory.MoveToExtraInventory(Item);
            }
            else if (currentPanel == extraPanel) {
                player.GetComponent<PlayerBehavior>().inventory.MoveToNormalInventory(Item);
            }
        }
    }
    
    public void OnEndDrag(PointerEventData eventData) {
        GameObject currentPanel = GetCurrentPanel();
        if (currentPanel == originalPanel || originalPanel == townPanel) {
            transform.position = originalPosition;
        }
        else if (currentPanel == inventoryPanel && originalPanel == extraPanel) {
            player.GetComponent<PlayerBehavior>().inventory.MoveToNormalInventory(Item);
        }
        else if (currentPanel == extraPanel && originalPanel == inventoryPanel) {
            player.GetComponent<PlayerBehavior>().inventory.MoveToExtraInventory(Item);
        }
        else if (currentPanel == inventoryPanel && originalPanel == chestPanel) {
            hud.CurrentChest.GetComponent<ChestBehavior>().ItemTakenFrom(Item);
        }
        else if (currentPanel == extraPanel && originalPanel == chestPanel) {
            bool wasProbablyAlreadyMoved = player.GetComponent<PlayerBehavior>().inventory.NormalInventoryFull;
            hud.CurrentChest.GetComponent<ChestBehavior>().ItemTakenFrom(Item);
            if(!wasProbablyAlreadyMoved) {
                player.GetComponent<PlayerBehavior>().inventory.MoveToExtraInventory(Item);
            }
        }
        else if (currentPanel == chestPanel && (originalPanel == extraPanel || originalPanel == inventoryPanel)) {
            hud.CurrentChest.GetComponent<ChestBehavior>().ItemMovedTo(Item);
        }
        else if (currentPanel == townPanel) {
            hud.Town.GetComponent<TownBehavior>().ItemMovedTo(Item);
        }
        else if (currentPanel == null) {
            player.GetComponent<PlayerBehavior>().inventory.DiscardItem(Item);
        }
        else {
            transform.position = originalPosition;
        }
    }
    
    bool IsUIPanel(GameObject go) {
        return go.name.Contains("Panel");
    }
    
    public GameObject GetCurrentPanel() {
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
 
         pointerData.position = Input.mousePosition;

         List<RaycastResult> results = new List<RaycastResult>();
         EventSystem.current.RaycastAll(pointerData, results);
         try {
            return (results as IEnumerable<RaycastResult>).First(result => IsUIPanel(result.gameObject)).gameObject;
         }
         catch(InvalidOperationException) { return null;}
    }
    
	// Use this for initialization
	void Start () {
        player = GameObject.FindWithTag("MainPlayer");
        hud = player.GetComponent<HUD>();
        
        inventoryPanel = hud.InventoryPanel;
        extraPanel = hud.ExtraInventoryPanel;
        townPanel = hud.TownPanel;
        chestPanel = hud.ChestPanel;
	}
	
}
