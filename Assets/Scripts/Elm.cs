using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Elm : MonoBehaviour, ReceiveSwing {

    public int MaxWood = 30;
    public int MaxBranches = 4;
    public int RespawnTime = 20;
    
    private int currentWood;
    private int currentBranches;
    
    [SerializeField]
    private AudioClip successHitSound;
    [SerializeField]
    private AudioClip failHitSound;
    [SerializeField]
    private GameObject particles;
    public AudioClip SuccessHitSound {get { return successHitSound;}}
    public AudioClip FailHitSound {get {return failHitSound;}}
    public GameObject Particles { get {return particles;}}
    
    private static GameObject player;
    
    public int Wood {
        get {return currentWood;}
        set {
            currentWood = Math.Max(value,0);
            if (currentWood == 0) {
                Die();
            }
        }
    }
    public int Branches {
        get { return currentBranches;}
        set {
            currentBranches = value;
        }
    }
    
	// Use this for initialization
	void Start () {
        if(player == null) {
            player = GameObject.FindWithTag("MainPlayer");
        }
		currentWood = MaxWood;
        currentBranches = MaxBranches;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    
    void Disappear() {
        this.GetComponent<Collider>().enabled = false;
        this.GetComponent<MeshRenderer>().enabled = false;
    }
    
    void Reappear() {
        this.GetComponent<Collider>().enabled = true;
        this.GetComponent<MeshRenderer>().enabled = true;
    }
    
    public void GetSwungAt() {
        Transform strikePointTrans;
        if(player.GetComponent<PlayerBehavior>().CurrentItem.Equals("Hatchet")) {
            player.GetComponent<PlayerBehavior>().inventory.AddItem("Wood");
            Wood -= 1;
            strikePointTrans = player.GetComponent<PlayerBehavior>().HeldItemObject.transform.Find("StrikePoint");
            if (strikePointTrans == null) {
                Debug.LogWarning(player.GetComponent<PlayerBehavior>().CurrentItem+" does not have a StrikePoint");
                return;
            }
            AudioSource.PlayClipAtPoint(SuccessHitSound,strikePointTrans.position);
            //TODO play particles
        }
        else {
            Vector3 audioPos = player.GetComponent<PlayerBehavior>().EquippedContainer.transform.position;
            AudioSource.PlayClipAtPoint(FailHitSound,audioPos);
        }
    }
    
    void GetClickedOn(GameObject player) {
        if(Branches > 0) {
            player.GetComponent<PlayerBehavior>().inventory.AddItem("Branch");
            Branches -= 1;
        }
    }
    
    public void Die() {
        Disappear();
        Wood = MaxWood;
        Branches = MaxBranches;
        Invoke("Reappear",RespawnTime);
    }
}
