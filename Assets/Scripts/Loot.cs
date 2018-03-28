using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loot : MonoBehaviour {

    private string loot;
    
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void Setup(string loot) {
        this.loot = loot;
    }
    
    public void ReleaseLoot() {
        GameObject lootGO = Instantiate(Resources.Load("InWorld/"+loot) as GameObject);
        lootGO.SetActive(true);
        lootGO.transform.position = transform.position;
    }
}
