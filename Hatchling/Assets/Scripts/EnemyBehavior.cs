using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBehavior : MonoBehaviour {

    
    public int attackLevel = 2;
    //public int defenseLevel = 2;
    public int maxHealth = 15;
    public string drop = "Coin";
    
    
    private float currentHealth;
    public float Health {
        get{return currentHealth;}
        set {
            if(value<=0) {
                currentHealth = 0;
                transform.parent.gameObject.transform.Find("HealthText").GetComponent<TextMesh>().text = "0";
                Die(GameObject.FindWithTag("MainPlayer").GetComponent<PlayerBehavior>());
            }
            else {
                currentHealth = value;
                transform.parent.gameObject.transform.Find("HealthText").GetComponent<TextMesh>().text = value.ToString();
            }
        }
    }
    
	// Use this for initialization
	void Start () {
        Health = maxHealth;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    
    public void GetSwungAt(GameObject player) {
        int damageToTake = player.GetComponent<PlayerBehavior>().AttackLevel;
        Health -= damageToTake;
        
    }
    
    void Die(PlayerBehavior player) {
        player.inventory.AddItem(drop);
        Destroy(transform.parent.gameObject);
    }
}
