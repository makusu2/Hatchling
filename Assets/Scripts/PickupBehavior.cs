using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PickupBehavior : MonoBehaviour {
    
    public int RespawnTime = -1;
    
    [System.NonSerialized]
    public int StackCount = 1;
    
    private Vector3 respawnLocation;
    
    public bool PreparingCombine = false; //Only true if another gameobject is preparing to combine with it. If true, don't combine with that gameobject.
    
	// Use this for initialization
	void Start () {
		respawnLocation = transform.position;
	}
    
    public bool DoesRespawn { get { return RespawnTime>0;}}
    
    public string PickupName;
    
    void Disappear() {
        this.GetComponent<Collider>().enabled = false;
        this.GetComponent<MeshRenderer>().enabled = false;
        try {
            this.GetComponent<Rigidbody>().isKinematic = true;
        }
        catch(MissingComponentException) {}
        foreach (Transform childTrans in transform) {
            childTrans.GetComponent<MeshRenderer>().enabled = false;
        }
    }
    
    void Reappear() {
        this.GetComponent<Collider>().enabled = true;
        this.GetComponent<MeshRenderer>().enabled = true;
        try {
            this.GetComponent<Rigidbody>().isKinematic = false;
        }
        catch(MissingComponentException){}
        foreach (Transform childTrans in transform) {
            childTrans.GetComponent<MeshRenderer>().enabled = true;
        }
        transform.position = respawnLocation;
    }
    
    public void OnCollisionEnter(Collision col) {
        if(!DoesRespawn) { //Only combine if they don't respawn
            PickupBehavior otherPickupBehavior = col.gameObject.GetComponent<PickupBehavior>();
            if (otherPickupBehavior != null && !otherPickupBehavior.DoesRespawn && otherPickupBehavior.PickupName == this.PickupName && !this.PreparingCombine && !otherPickupBehavior.PreparingCombine) {
                this.PreparingCombine = otherPickupBehavior.PreparingCombine = true;
                this.StackCount = this.StackCount + otherPickupBehavior.StackCount;
                Destroy(otherPickupBehavior.gameObject);
                this.PreparingCombine = false;
            }
        }
    }
    
    public void StackWithNearby() {
        foreach(GameObject nearbyGO in GetNearbyStackables()) {
            AbsorbStackable(nearbyGO);
        }
    }
    
    public GameObject[] GetNearbyStackables() {
        int overlapTestDist = 1;
        Collider[] possibleColliders = Physics.OverlapSphere(transform.position,overlapTestDist);
        List<GameObject> GOs = new List<GameObject>();
        foreach (Collider possibleCollider in possibleColliders) {
            if (this.CanCombineWith(possibleCollider.gameObject)) {
                GOs.Add(possibleCollider.gameObject);
            }
        }
        return GOs.ToArray();
        
    }
    
    public bool CanCombineWith(GameObject go) {
        PickupBehavior otherPB = go.GetComponent<PickupBehavior>();
        return (this.gameObject != go && otherPB != null && !DoesRespawn && !otherPB.DoesRespawn && otherPB.PickupName == this.PickupName && !this.PreparingCombine && !otherPB.PreparingCombine);
    }
    
    public void AbsorbStackable(GameObject go) {
        if (!CanCombineWith(go)) {
            throw new ArgumentException("Tried to absorb stackable but wasn't compatible");
        }
        else {
            PickupBehavior otherPB = go.GetComponent<PickupBehavior>();
            this.PreparingCombine = otherPB.PreparingCombine = true;
            this.StackCount = this.StackCount + otherPB.StackCount;
            Destroy(otherPB.gameObject);
            this.PreparingCombine = false;
        }
    }
    
    public void GetTaken() {
        if(DoesRespawn) {
            Disappear();
            Invoke("Respawn",RespawnTime);
        }
        else {
            Destroy(this);
        }
        
    }
    
    void GetClickedOn(GameObject player) {
        for (int i=0;i<StackCount;i++) {
            player.GetComponent<PlayerBehavior>().inventory.AddItem(PickupName);
        }
        GetTaken();
    }
    
    void Respawn() {
        Reappear();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
