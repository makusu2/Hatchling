using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBehavior : MonoBehaviour {

    
    public int attackLevel = 2;
    public int defenseLevel = 2;
    public int maxHealth = 5;
    private float currentHealth;
    public string drop = "Coin";
    
    
    public float Health {
        get{return currentHealth;}
        set {
            currentHealth = value;
            transform.parent.gameObject.transform.Find("HealthText").GetComponent<TextMesh>().text = value.ToString();
        }
    }
    
	// Use this for initialization
	void Start () {
        Health = maxHealth;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    
    
    void GetClickedOn(ClickArg args) {
        int damageToTake = (int)((float)(args.player.attackLevel) / (float)defenseLevel);
        if (damageToTake >= currentHealth) {
            Health = 0;
            Die(args.player);
        }
        else {
            Health -= damageToTake;
        }
    }
    
    void Die(PlayerBehavior player) {
        player.inventory.AddItem(drop);
        Destroy(transform.parent.gameObject);
    }
}
