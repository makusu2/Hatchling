using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WolfBehavior : LivingEntity {

    
    
    private float bleedTime = 0.5f;
    
    [SerializeField] private AudioClip successBiteSound;
    [SerializeField] private AudioClip failBiteSound;
    
    private GameObject bloodGO;
    
    
            
    
	// Use this for initialization
	void Start () {
        base.Prepare(drop: "1*Coin,2*UncookedMeat");
        mouthGO = transform.Find("TeethPoint").gameObject;
        bloodGO = mouthGO.transform.Find("BloodSprayEffect").gameObject;
        bloodGO.SetActive(false);
	}
    
    
    
    void Bleed() {
        bleeding = true;
        Invoke("StopBleeding",bleedTime);
    }
    void StopBleeding() {
        bleeding = false;
    }
    
    private bool bleeding {
        get {
            return bloodGO.activeInHierarchy;
        }
        set {
            bloodGO.SetActive(value);
        }
    }
    
    
    void FixedUpdate() {
        if (IsDead) {
           return;
        }
        if (aniAction == aniActions.Attacking) {
            return;
        }
        else {
            if (targetEnemy == null) {
                ContinueRoaming();
            }
            else {
                if (targetEnemy.GetComponent<Health>().IsDead) {
                    targetEnemy = null;
                    return;
                }
                SetDestination(targetEnemy.transform.position,isRunning:true);
                if(DistToDest() < distToAttack) {
                    nav.isStopped = true;
                    nav.velocity = Vector3.zero;
                    if (LookingNearTarget()) {
                        if (CanBeginAttack()) {
                            BeginAttack();
                        }
                    }
                    else {
                        TurnTowardTarget();
                    }
                }
            }
        }
    }
    
    bool CanBeginAttack() {
        return aniAction != aniActions.Attacking;
    }
    
        
    
    
    
    
    void BeginAttack() {
        if (CanBeginAttack()) {
            aniAction = aniActions.Attacking;
            Invoke("TestHit",0.5f);
        }
    }
    
    void OnCollisionEnter(Collision col) {
        
    }
    
    void TestHit() {
        if (targetEnemy == null) {
            return;
        }
        Vector3 mouthPos = mouthGO.transform.position;
        Vector3 targetPos = targetEnemy.GetComponent<Collider>().ClosestPointOnBounds(mouthPos);
        float dist = Vector2.Distance(new Vector2(targetPos.x,targetPos.z),new Vector2(mouthPos.x,mouthPos.z));
        if (dist < distToDamage) {
            targetEnemy.GetComponent<Health>().GetDamaged(attackLevel);
            Bleed();
            AudioSource.PlayClipAtPoint(successBiteSound,mouthPos);
        }
        else {
            AudioSource.PlayClipAtPoint(failBiteSound,mouthPos);
        }
    }
}
