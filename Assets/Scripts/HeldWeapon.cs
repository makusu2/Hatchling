using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HeldWeapon : MonoBehaviour, ItemActivator {


    public int AttackLevel = 1;
    
    [SerializeField]
    public Action customActivateItem;
    
    private static GameObject player;
	// Use this for initialization
	void Start () {
        if(player == null) {
            player = GameObject.FindWithTag("MainPlayer");
        }
	}
	
	// Update is called once per frame
	void Update () {
        
	}
    void OnTriggerEnter(Collider col) {
        bool heldByCol = transform.IsChildOf(col.gameObject.transform);
        if (heldByCol) {
            return;
        }
        Health colHealth = col.gameObject.GetComponent<Health>();
        if (colHealth != null && player.GetComponent<PlayerBehavior>().IsSwinging) {
            colHealth.GetDamaged(AttackLevel);
            Vector3 contactPoint = col.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
            MakuUtil.PlayBloodAt(contactPoint);
        }
        ReceiveSwing swungGO = col.gameObject.GetComponent<ReceiveSwing>();
        if (swungGO != null && player.GetComponent<PlayerBehavior>().IsSwinging) {
            swungGO.GetSwungAt();
        }
        
    }
    
    void Setup(Action usageFunc) {
        this.customActivateItem = usageFunc;
    }
    
    public void ActivateItem() {
        if (customActivateItem == null) {
            Swing();
        }
        else {
            customActivateItem();
        }
    }
    void Swing() {
            player.GetComponent<PlayerBehavior>().HandAnimator.Play("swingWeapon",0);
    }
    
}
