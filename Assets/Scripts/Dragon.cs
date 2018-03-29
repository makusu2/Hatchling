﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Dragon : MonoBehaviour {

    private float attackCooldown = 0.2f; //Should probably be higher for the real game
    
    private GameObject targetEnemy = null;
    
    private Health health;

    private GameObject body;
    public GameObject Body {
        get {
            return body;
        }
    }
    
    private DragonStage stage;
    public DragonStage Stage {
        get {
            return stage;
        }
        set {
            stage = value;
            string goName = stage.ToString()+"Body";
            Transform possibleTrans = transform.Find(goName);
            if(possibleTrans == null) {
                throw new ArgumentException();
            }
            foreach (Transform child in transform) {
                if (child == possibleTrans) {
                    child.gameObject.SetActive(true);
                }
                else {
                    child.gameObject.SetActive(false);
                }
            }
            body = possibleTrans.gameObject;
            
            FitColliderToChildren();
            
            if(stage == DragonStage.Egg) {
                GetComponent<Faction>().faction = Factions.None;
            }
            else {
                GetComponent<Faction>().faction = Factions.player;
            }
        }
    }
    
    
    private Animator ani {
        get {
            return Body.GetComponent<Animator>();
        }
    }
    private UnityEngine.AI.NavMeshAgent nav;
   
    //public int maxHealth = 15;
    
    private int distToNotice = 30; //Higher than others on purpose; can sense things far away
    
    private float walkSpeed = 5;
    private float runSpeed = 8;
    private float turnSpeed = 3;
    private int maxRoamDistance = 20;
    
    private Vector3 spawnPoint;
    
    public float BleedTime = 0.5f;
    
    
    private static System.Random rnd = new System.Random();
    
    
    
    
    private GameObject player;
    
    
    
    private float lastAttackTime;
    public bool IsAttacking {
        get {
            return Time.time - lastAttackTime < attackCooldown;
        }
    }
    
    public bool IsWalking {
        get {
            return ani.GetCurrentAnimatorStateInfo(0).IsName("Base.walk") || ani.GetBool("Walking");
        }
    }
    
    public bool IsRunning {
        get {
            return ani.GetCurrentAnimatorStateInfo(0).IsName("Base.run") || ani.GetBool("Running");
        }
    }
    
    
    
    
    void SetAction(string action) {
        if (action == "Walking") {
            ani.SetBool("Walking",true);
            ani.SetBool("Running",false);
        }
        else if (action == "Idle") {
            ani.SetBool("Walking",false);
            ani.SetBool("Running",false);
        }
        else if (action == "Running") {
            ani.SetBool("Walking",false);
            ani.SetBool("Running",true);
        }
        else if (action == "Attack") {
            ani.SetBool("Walking",false);
            ani.SetBool("Running",false);
            //TODO have it do some mini animation
        }
        else if (action == "Die") {
            ani.SetBool("Walking",false);
            ani.SetBool("Running",false);
            ani.SetBool("Dead",true);
        }
    }
    
    private Vector3 GetNewRoamDestination() {
        int xDif = rnd.Next(-maxRoamDistance,maxRoamDistance);
        int zDif = rnd.Next(-maxRoamDistance,maxRoamDistance);
        Vector3 newPos = spawnPoint;
        newPos.x += xDif;
        newPos.z += zDif;
        return newPos;
    }
           
	// Use this for initialization
	void Start () {
        GetComponent<EnemyFinder>().Setup(distToNotice: this.distToNotice);
        health = GetComponent<Health>();
        health.Setup(maxHealth:100,immunities:new Health.DamageTypes[]{Health.DamageTypes.fire});
        player = GameObject.FindWithTag("MainPlayer");
        spawnPoint = transform.position;
        nav = GetComponent<UnityEngine.AI.NavMeshAgent>();
        lastAttackTime = Time.time - attackCooldown;
        Stage = DragonStage.Egg; //Change to egg
        SetDestination(transform.position);
        //DoDelayedActions();
        TestForNearbyEnemies();
        Invoke("CheckGrowDragon",5);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    
    bool LookingNearPlayer() {
        Vector3 targetDir = player.transform.position - transform.position;
        Vector3 currentDir = transform.forward;
        targetDir.y = 0;
        currentDir.y = 0;
        float angleBetween = Vector3.Angle(targetDir,currentDir);
        
        bool lookingNearPlayer = angleBetween < 10;
        return lookingNearPlayer;
    }
    
    
    
    void FixedUpdate() {
        if (Stage == DragonStage.Egg || IsAttacking) {
            return;
        }
        else {
            if (targetEnemy == null) {
                ContinueRoaming();
            }
            else{
                if (targetEnemy.GetComponent<Health>().IsDead) {
                    targetEnemy = null;
                    return;
                }
                SetDestination(targetEnemy.transform.position, isRunning: true);
                if (DistToDest() < 5) {
                    nav.isStopped = true;
                    if (LookingNearTarget()) {
                        BeginAttack();
                    }
                    else {
                        TurnTowardTarget();
                    }
                }
            }
        }
    }
    
    
    void SetDestination(Vector3 position, bool isRunning = true) {
        if (!MobileStage()) {
            return;
        }
        nav.destination = position;
        
        bool closeEnough = DistToDest() < 2;
        if(closeEnough) {
            nav.isStopped = true;
            SetAction("Idle");
        }
        else {
            nav.isStopped = false;
            nav.speed = isRunning?runSpeed:walkSpeed;
            if(isRunning) {
                SetAction("Running");
            }
            else {
                SetAction("Walking");
            }
        }
    }
    float DistToDest() {
        return nav.pathPending?Vector3.Distance(transform.position, nav.destination):nav.remainingDistance;
    }
    
    void ContinueRoaming() {
        if (DistToDest() < 2) {
            SetDestination(GetNewRoamDestination(),isRunning: false);
        }
    }
    
    void BeginAttack() {
        if (!IsAttacking) {
            SetAction("Attack");
            //TODO do attack here; decide which to do like breathe fire or whatever
            ThrowFireball();
            lastAttackTime = Time.time;
        }
    }
    
    void ThrowFireball() {
        GameObject fireball = Instantiate(Resources.Load("InWorld/Fireball") as GameObject);
        fireball.SetActive(true);
        fireball.transform.position = Body.transform.Find("MouthLocation").position;
        fireball.GetComponent<FireballBehavior>().GetShot(transform.forward);
    }
    
    void Die() {
        print("Dragon has died");
    }
    
    
    void CheckGrowDragon() {
        //Make a bunch of dicts of enum to whatever info you need, and make the dragon stages enums
        if (CanEvolve()) {
            GrowDragon();
        }
        Invoke("CheckGrowDragon",5);
    }
    void GrowDragon() {
        Stage += 1;
    }
    
    
    
    bool NearFire() {
        int requiredDistToFire = 5;
        Collider[] possibleColliders = Physics.OverlapSphere(transform.position,requiredDistToFire);
        foreach (Collider possibleCollider in possibleColliders) {
            GameObject possibleGO = possibleCollider.gameObject;
            int heatLevel;
            try {
                heatLevel = possibleGO.GetComponent<Heated>().HeatLevel;
                if(heatLevel >= 3) {
                    return true;
                }
            }
            catch(NullReferenceException) {
                continue;
            }
        }
        return false;
    }
    
    
    void TurnTowardTarget() {
        SetAction("Walking");
            
        Vector3 targetDir = targetEnemy.transform.position - transform.position;
        float step = turnSpeed * Time.deltaTime;
        Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0F);
        Quaternion newRotation = Quaternion.LookRotation(newDir);
        newRotation.x = 0.0f;
        newRotation.z = 0.0f;
        
        
        transform.rotation = newRotation;
    }
    
    bool LookingNearTarget() {
        Vector3 targetDir = targetEnemy.transform.position - transform.position;
        Vector3 currentDir = transform.forward;
        targetDir.y = 0;
        currentDir.y = 0;
        float angleBetween = Vector3.Angle(targetDir,currentDir);
        
        bool lookingNearTarget = angleBetween < 10;
        return lookingNearTarget;
    }
    
    void TestForNearbyEnemies() {
        UpdateTargetEnemy();
        Invoke("TestForNearbyEnemies",2);
    }
    
    void UpdateTargetEnemy() {
        if (targetEnemy != null) {
            if (targetEnemy.GetComponent<Health>().IsDead) {
                targetEnemy = null;
            }
        }
        if (targetEnemy == null) {
            GameObject targetGO = GetComponent<EnemyFinder>().GetClosestEnemy();
            targetEnemy = targetGO;
        }
    }
    
    
     void FitColliderToChildren ()
    {
        BoxCollider bc = gameObject.GetComponent<BoxCollider>();
        Bounds bounds = new Bounds (Vector3.zero, Vector3.zero);
        bool hasBounds = false;
        Renderer[] renderers =  gameObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer render in renderers) {
            if (hasBounds) {
                bounds.Encapsulate(render.bounds);
            } else {
                bounds = render.bounds;
                hasBounds = true;
           }
       }
      if (hasBounds) {
            bc.center = bounds.center - gameObject.transform.position;
            bc.size = bounds.size;
      } else {
            bc.size = bc.center = Vector3.zero;
            bc.size = Vector3.zero;
      }
   }
   
    public enum DragonStage { Egg, Newborn, };
    public static string NameOfStage(int index) {
        return Enum.GetName(typeof(DragonStage), index);
    }
    public bool CanEvolve() {
        switch(Stage) {
            case DragonStage.Egg:
                return NearFire();
            case DragonStage.Newborn:
                return false;
            default:
                Debug.LogError("No evolution condition for "+Stage.ToString());
                return false;
        }
    }
    public bool MobileStage() {
        switch(Stage) {
            case DragonStage.Egg:
                return false;
            case DragonStage.Newborn:
                return true;
            default:
                Debug.LogError("No mobility found for "+Stage.ToString());
                return false;
        }
    }
    
}