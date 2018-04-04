using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeldWeapon : MonoBehaviour {


    public int AttackLevel = 1;
    
    [SerializeField] private AudioClip hitSound;
    
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
    
    public void ActivateItem() {
        player.GetComponent<PlayerBehavior>().Arms.GetComponent<Animator>().SetTrigger("Swing");
    }
}
