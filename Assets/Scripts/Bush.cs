using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bush : MonoBehaviour {
    
    private GameObject fruit;
    public int maxFruit = 8;
    
    void SpawnFruit()
    {
        Vector3 fruitPos = new Vector3(this.transform.position.x+Random.Range(-1.0f,1.0f),
                                       this.transform.position.y+Random.Range(0.0f,1.0f),
                                       this.transform.position.z+Random.Range(-1.0f,1.0f));
        GameObject fruitInstance = Instantiate(fruit,fruitPos,Quaternion.identity);
        fruitInstance.transform.SetParent(transform);
        fruitInstance.GetComponent<PickupBehavior>().RespawnTime = 5;
    }
    
	// Use this for initialization
	void Start () {
        fruit = Resources.Load("InWorld/Apple") as GameObject;
		for (int i=0;i<maxFruit;i++) {
            SpawnFruit();
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
