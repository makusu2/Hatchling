using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Dragon : MonoBehaviour {

    private float attackCooldown = 0.2f; //Should probably be higher for the real game
    
    private GameObject targetEnemy = null;
    
    private Health health;
    
    //public Factions faction = Factions.player;

    private GameObject body;
    public GameObject Body {
        get {
            return body;
        }
    }
    
    public string BodyStage {
        get {
            return body.name;
        }
        set {
            Transform possibleTrans = transform.Find(value);
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
            return gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Base.walk") || gameObject.GetComponent<Animator>().GetBool("Walking");
        }
    }
    
    public bool IsRunning {
        get {
            return gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Base.run") || gameObject.GetComponent<Animator>().GetBool("Running");
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
        BodyStage = "NewbornBody";
        SetDestination(transform.position);
        //DoDelayedActions();
        TestForNearbyEnemies();
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
        if (IsAttacking) {
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
    
    /*public void DoDelayedActions() {
        ThrowFireball();
        Invoke("DoDelayedActions",1);
    }*/
    
    
    
    
    
    
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
        //print(GetNearbyFactionGOs().ToString());
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
}
