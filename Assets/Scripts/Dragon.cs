using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Dragon : MonoBehaviour {

    private int attackCooldown = 2;

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
   
    public int maxHealth = 15;
    
    private int distToNotice = 15;
    
    private float walkSpeed = 5;
    private float runSpeed = 8;
    private int maxRoamDistance = 20;
    
    private Vector3 spawnPoint;
    
    public float BleedTime = 0.5f;
    
    [SerializeField] private AudioClip successBiteSound;
    [SerializeField] private AudioClip failBiteSound;
    
    private static System.Random rnd = new System.Random();
    
    
    
    
    private GameObject player;
    
    
    private float currentHealth;
    public float Health {
        get{return currentHealth;}
        set {
            if(value<=0) {
                currentHealth = 0;
                Die();
            }
            else {
                currentHealth = value;
            }
        }
    }
    
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
        Health = maxHealth;
        player = GameObject.FindWithTag("MainPlayer");
        spawnPoint = transform.position;
        nav = GetComponent<UnityEngine.AI.NavMeshAgent>();
        lastAttackTime = Time.time - attackCooldown;
        BodyStage = "NewbornBody";
        SetDestination(transform.position);
        DoDelayedActions();
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
            ContinueRoaming();
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
            //TODO do attack here; decide which to do like breathe fire or whatever
            ThrowFireball();
            lastAttackTime = Time.time;
        }
    }
    
    void ThrowFireball() {
        GameObject fireball = Instantiate(Resources.Load("InWorld/Fireball") as GameObject);
        //fireball.transform.SetParent(transform,false);
        fireball.SetActive(true);
        fireball.transform.position = Body.transform.Find("MouthLocation").position;
        fireball.GetComponent<FireballBehavior>().GetShot(transform.forward);
        //fireball.GetComponent<Rigidbody>().velocity = transform.forward;
    }
    
    public void GetDamaged(int damage) {
        Health -= damage;
    }
    
    void Die() {
        gameObject.SetActive(false);
    }
    
    public void DoDelayedActions() {
        ThrowFireball();
        Invoke("DoDelayedActions",1);
    }
}
