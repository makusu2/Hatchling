using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class WolfBehavior : LivingEntity {

    
    private float lastAttackTime;
    private float cooldownTime = 1f;
    
    
    [SerializeField] private AudioClip successBiteSound;
    [SerializeField] private AudioClip failBiteSound;
    
    
	// Use this for initialization
	void Start () {
        base.Prepare(drop: "1*Coin,2*UncookedMeat",fac:Factions.wolf);
        foreach (Transform testMouth in gameObject.GetComponentsInChildren<Transform>()) {
            if (testMouth.CompareTag("MouthGO")) {
                mouthGO = testMouth.gameObject;
            }
        }
        lastAttackTime = Time.time;
	}
    
    
    
    protected override void FixedUpdate() {
        base.FixedUpdate();
        if (IsDead) {
           return;
        }
        if (aniAction == aniActions.Attacking) {
            return;
        }
        else {
            if (targetEnemy != null) {
                if (targetEnemy.GetComponent<Health>().IsDead) {
                    targetEnemy = null;
                    return;
                }
                bool closeEnoughToAttack = DistToGOSimple(targetEnemy) < distToAttack;
                if(closeEnoughToAttack) {
                    nav.isStopped = true;
                    nav.velocity = Vector3.zero;
                    if (EnemyInFrontOfTeeth() != null && CanBeginAttack() /*&& LookingNearTarget()*/){
                        //print("Enemy in front of teeth isn't null, it's: "+EnemyInFrontOfTeeth().name);
                        BeginAttack();
                    }
                    else if(!LookingAt(targetEnemy)) {
                        TurnToward(targetEnemy);
                    }
                }
                else {
                    SetDestination(targetEnemy.transform.position,isRunning:true);
                }
            }
        }
    }
    
    protected override void DelayedUpdate() {
       base.DelayedUpdate();
       if(targetEnemy == null) {
           ContinueRoaming();
       }
    }
    
    bool CanBeginAttack() {
        return Time.time - lastAttackTime > cooldownTime && aniAction != aniActions.Attacking;
    }
    
        
    
    
    
    
    void BeginAttack() {
        if (CanBeginAttack()) {
            lastAttackTime = Time.time;
            aniAction = aniActions.Attacking;
            Invoke("TestHit",0.3f);
        }
    }
    GameObject EnemyInFrontOfTeeth(float sphereRadius = 0.03f) { //Returns the gameobject that has a health component or null if none
        Vector3 mouthPos = mouthGO.transform.position;
        
        RaycastHit hit;
        
        if (Physics.Raycast(mouthPos,transform.forward,out hit,maxDistance:distToDamage)) {
            Collider col = hit.collider;
            GameObject hitGO = col.gameObject;
            Health enemyHealth;
            try {
                enemyHealth = hitGO.GetComponent<Health>();
                if (enemyHealth == null) {
                    Component[] possibleComponents = hitGO.GetComponentsInParent(typeof(Health));
                    if (possibleComponents.Length > 1) {
                        Debug.LogWarning("Got more than one health in parents");
                    }
                    Component firstComponent = possibleComponents[0];
                    enemyHealth = (Health)firstComponent;
                    return enemyHealth.gameObject;
                }
            }
            catch(IndexOutOfRangeException) {
                //Don't do anything, go to out of if block
            }
        }
        Collider[] colliders = Physics.OverlapSphere(mouthPos, sphereRadius/* Radius */);
        foreach (Collider col in colliders) {
            if(col.gameObject != this.gameObject && col.gameObject.GetComponent<Health>() != null) {
                return col.gameObject;
            }
        }
        if (targetEnemy != null) {
            foreach(Collider col in targetEnemy.GetComponentsInChildren<Collider>()) {
                if(col.bounds.Contains(mouthPos)) {
                    return targetEnemy;
                }
            }
        }
        return null;
    }
    void TestHit() {
        GameObject hitGO = EnemyInFrontOfTeeth();
        if (hitGO != null) {
            hitGO.GetComponent<Health>().GetDamaged(attackLevel);
            MakuUtil.PlayBloodAtPoint(mouthGO.transform.position);
            AudioSource.PlayClipAtPoint(successBiteSound,mouthGO.transform.position);
        }
        else {
            AudioSource.PlayClipAtPoint(failBiteSound,mouthGO.transform.position);
        }
    }
}
