using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FistBehavior : MonoBehaviour {

    private GameObject player;
    
    private int AttackLevel = 1;
    
	// Use this for initialization
	void Start () {
        player = GameObject.FindWithTag("MainPlayer");
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    void OnTriggerEnter(Collider col) {
        bool heldByCol = transform.IsChildOf(col.gameObject.transform);
        if (heldByCol) {
            return;
        }
        if (player.GetComponent<PlayerBehavior>().UsingHands) {
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
    }
}
