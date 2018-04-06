﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HeldWeapon : MonoBehaviour, ItemActivator {


    public int AttackLevel = 1;
    
    [SerializeField] private AudioClip hitSound;
    
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
        Health colHealth = col.gameObject.GetComponent<Health>();
        if (colHealth != null && player.GetComponent<PlayerBehavior>().IsSwinging) {
            colHealth.GetDamaged(AttackLevel);
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
            player.GetComponent<PlayerBehavior>().Arms.GetComponent<Animator>().SetTrigger("Swing");
    }
}
