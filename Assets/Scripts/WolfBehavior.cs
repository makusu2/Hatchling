using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class WolfBehavior : LivingEntity {

    
    
    //private float bleedTime = 0.5f;
    
    //[SerializeField] private AudioClip successBiteSound;
    //[SerializeField] private AudioClip failBiteSound;
    
    //private GameObject bloodGO;
    
    
            
    
	// Use this for initialization
	void Start () {
        base.Prepare(drop: "1*Coin,2*UncookedMeat");
        foreach (Transform testMouth in gameObject.GetComponentsInChildren<Transform>()) {
            if (testMouth.CompareTag("MouthGO")) {
                mouthGO = testMouth.gameObject;
            }
        }
        mouthGO.GetComponent<EntityMouth>().Setup(this,attackLevel:2);
        //mouthGO = .Where(
        //transform.Find("TeethPoint").gameObject;
        /*bloodGO = mouthGO.transform.Find("BloodSprayEffect").gameObject;
        bloodGO.SetActive(false);*/
	}
    
    
    
    /*void Bleed() {
        bleeding = true;
        Invoke("StopBleeding",bleedTime);
    }
    void StopBleeding() {
        bleeding = false;
    }*/
    
    /*private bool bleeding {
        get {
            return bloodGO.activeInHierarchy;
        }
        set {
            bloodGO.SetActive(value);
        }
    }*/
    
    
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
                bool closeEnoughToAttack = DistToGO(targetEnemy) < distToAttack;
                if(closeEnoughToAttack) {
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
                else {
                    SetDestination(targetEnemy.transform.position,isRunning:true);
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
            //Invoke("TestHit",0.5f);
        }
    }
    
    /*void OnCollisionEnter(Collision col) {
        
    }*/
    
    /*void TestHit() {
        if (targetEnemy == null) {
            return;
        }
        Vector3 mouthPos = mouthGO.transform.position;
        Vector3 targetPos = targetEnemy.GetComponent<Collider>().ClosestPointOnBounds(mouthPos);
        float dist = Vector2.Distance(new Vector2(targetPos.x,targetPos.z),new Vector2(mouthPos.x,mouthPos.z));
        if (dist < distToDamage) {
            targetEnemy.GetComponent<Health>().GetDamaged(attackLevel);
            //MakuUtil.PlayBloodAt(mouthGO.transform.position);//Bleed();
            AudioSource.PlayClipAtPoint(successBiteSound,mouthPos);
        }
        else {
            AudioSource.PlayClipAtPoint(failBiteSound,mouthPos);
        }
    }*/
}
