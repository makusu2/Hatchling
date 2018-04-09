using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryIconBehavior : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler, IPointerClickHandler{

    private GameObject player;
    
    private HUD hud;
    
    private Vector3 originalPosition;
    private Rect originalRect; //Rec when started
    //private bool wasInNormal;
    
    private Rect inventoryRect;
    private Rect extraRect;
    private Rect townRect;
    
    public string Item;
    
    
    public void OnBeginDrag(PointerEventData eventData) {
        originalPosition = transform.position;
        originalRect = GetCurrentRect();
    }
    
    public void OnDrag(PointerEventData eventData) {
        transform.position = Input.mousePosition;//rayPoint;
    }
    
    public void OnPointerClick(PointerEventData pointerEventData) {
        if(Input.GetKey(KeyCode.LeftShift)) {
            //TODO have the clicked item go to whatever it should go to
            Rect rec = GetCurrentRect();
            if ((rec == inventoryRect || rec == extraRect) && hud.TownMenuOpen) {
                hud.Town.GetComponent<TownBehavior>().ItemMovedTo(Item);
            }
            else if (rec == inventoryRect) {
                player.GetComponent<PlayerBehavior>().inventory.MoveToExtraInventory(Item);
            }
            else if (rec == extraRect) {
                player.GetComponent<PlayerBehavior>().inventory.MoveToNormalInventory(Item);
            }
        }
    }
    
    public void OnEndDrag(PointerEventData eventData) {
        Rect rec = GetCurrentRect();
        if (rec == originalRect || originalRect == townRect) {
            transform.position = originalPosition;
        }
        else if (rec == inventoryRect) {
            player.GetComponent<PlayerBehavior>().inventory.MoveToNormalInventory(Item);
        }
        else if (rec == extraRect) {
            player.GetComponent<PlayerBehavior>().inventory.MoveToExtraInventory(Item);
        }
        else if (rec == townRect) {
            hud.Town.GetComponent<TownBehavior>().ItemMovedTo(Item);
        }
        else if (rec == default(Rect)) {
            player.GetComponent<PlayerBehavior>().inventory.DiscardItem(Item);
        }
        else {
            Debug.LogError("This shouldn't happen");
        }
    }
    
    public Rect GetCurrentRect() {
        Vector2 mp = Input.mousePosition;
        if (inventoryRect.Contains(mp)) {
            return inventoryRect;
        }
        else if (extraRect.Contains(mp) && hud.InventoryMenuOpen) {
            return extraRect;
        }
        else if (townRect.Contains(mp) && hud.TownMenuOpen) {
            return townRect;
        }
        else {
            return default(Rect);
        }
    }
    
    Rect RectFromTrans(RectTransform trans) {
        Rect tempRect = trans.rect;
        float trueRectX = trans.anchoredPosition.x + tempRect.x;
        float trueRectY = trans.anchoredPosition.y + tempRect.y;
        Vector2 truePos = new Vector2(trueRectX,trueRectY);
        Vector2 trueDims = new Vector2(tempRect.width,tempRect.height);
        return new Rect(truePos,trueDims);
    }
    
	// Use this for initialization
	void Start () {
        player = GameObject.FindWithTag("MainPlayer");
        hud = player.GetComponent<HUD>();
        inventoryRect = RectFromTrans(hud.InventoryPanel.GetComponent<RectTransform>());
        extraRect = RectFromTrans(hud.ExtraInventoryPanel.GetComponent<RectTransform>());
        townRect = RectFromTrans(hud.TownPanel.GetComponent<RectTransform>());
	}
	
}
