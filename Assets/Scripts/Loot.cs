using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loot : MonoBehaviour {

    private string loot;
    
    private System.Random rnd = new System.Random();
    
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
        
        string[] components = loot.Split(',');
        foreach(string comp in components) {
            int count = int.Parse(comp.Split('*')[0]);
            string compMat = comp.Split('*')[1];
            GameObject lootGOTemplate = Resources.Load("InWorld/"+compMat) as GameObject;
            for (int i=0;i<count;i++) {
                GameObject lootGO = Instantiate(lootGOTemplate);
                lootGO.SetActive(true);
                Vector3 spawnPosition = transform.position;
                spawnPosition.y = spawnPosition.y + (float)rnd.NextDouble();
                lootGO.transform.position = spawnPosition;
            }
        }
    }
}
