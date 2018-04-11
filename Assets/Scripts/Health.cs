using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Health : MonoBehaviour {
    
    public enum DamageTypes {unknown, fire, fall,}

    public int maxHealth;
    
    public bool IsPlayer;
    
    public Action deathMethod;
    
    public DamageTypes[] immunities = {};
    
    public HUD Hud;
    private float currentHealth;
    public float health {
        get{return currentHealth;}
        set {
            if(value<=0) {
                currentHealth = 0;
                if(IsPlayer) {
                    Hud.SetHealthStat(0);
                }
                deathMethod();
            }
            else {
                if(IsPlayer) {
                    Hud.SetHealthStat((int)value);
                }
                currentHealth = value;
            }
        }
    }
    
    public bool IsDead {
        get {
            return health <= 0;
        }
    }
    
    
	// Use this for initialization
	void Start () {
		health = maxHealth;
	}
    
    public void Setup(int maxHealth=10, bool isPlayer = false, DamageTypes[] immunities = null, HUD hud = null, Action deathMethod = null) {
        this.maxHealth = maxHealth;
        this.IsPlayer = isPlayer;
        if (immunities != null) {
            this.immunities = immunities;
        }
        this.Hud = hud;
        if (deathMethod == null) {
            this.deathMethod = Die;
        }
        else {
            this.deathMethod = deathMethod;
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    public void GetDamaged(int dmg,DamageTypes damageType = DamageTypes.unknown) {
        if (immunities.Contains(damageType)) {
            return; //Immune to damage
        }
        else {
            health -= dmg;
        }
    }
    
    public void Respawn() {
        health = maxHealth;
    }
    
    void Die() {
        print("No die method was specified for "+gameObject.name);
    }
    
}
