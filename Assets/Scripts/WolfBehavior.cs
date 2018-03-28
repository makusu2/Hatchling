﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfBehavior : MonoBehaviour {

    public int attackLevel = 2;
    
    private int distToNotice = 15;
    private  int distToAttack = 2;
    private int distToDamage = 2;
    
    private Health health;
    
    private float walkSpeed = 2;
    private float runSpeed = 5;
    private float turnSpeed = 3;
    private int maxRoamDistance = 20;
    
    private Vector3 spawnPoint;
    
    private float bleedTime = 0.5f;
    
    private Loot loot;
    
    [SerializeField] private AudioClip successBiteSound;
    [SerializeField] private AudioClip failBiteSound;
    
    private static System.Random rnd = new System.Random();
    
    
    private GameObject teethPoint;
    private Vector3 teethPointLocation {
        get {
            return teethPoint.transform.position;
        }
    }
    
    private GameObject bloodGO;
    
    public string drop = "Coin";
    
    private GameObject player;
    
    
    
    private bool isAttacking = false;
    public bool IsAttacking {
        get {
            return isAttacking || gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Base.hit") || gameObject.GetComponent<Animator>().GetBool("BeginAttack");
        }
        set{}
    }
    
    public bool IsWalking {
        get {
            return gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Base.walk") || gameObject.GetComponent<Animator>().GetBool("Walking");
        }
        set {}
    }
    
    public bool IsRunning {
        get {
            return gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Base.run") || gameObject.GetComponent<Animator>().GetBool("Running");
        }
        set {}
    }
    
    
    
    
    void SetAction(string action) {
        if (action == "Walking") {
            GetComponent<Animator>().SetBool("AllowIdle",false);
            GetComponent<Animator>().SetBool("Walking",true);
            GetComponent<Animator>().SetBool("Running",false);
            GetComponent<Animator>().SetBool("Dead",false);
            GetComponent<Animator>().ResetTrigger("BeginAttack");
        }
        else if (action == "Idle") {
            GetComponent<Animator>().SetBool("AllowIdle",true);
            GetComponent<Animator>().SetBool("Walking",false);
            GetComponent<Animator>().SetBool("Running",false);
            GetComponent<Animator>().ResetTrigger("BeginAttack");
        }
        else if (action == "Dash") {
            GetComponent<Animator>().SetBool("AllowIdle",false);
            GetComponent<Animator>().SetBool("Walking",false);
            GetComponent<Animator>().SetBool("Running",true);
            GetComponent<Animator>().SetBool("Dead",false);
            GetComponent<Animator>().ResetTrigger("BeginAttack");
        }
        else if (action == "Attack") {
            GetComponent<Animator>().SetBool("AllowIdle",false);
            GetComponent<Animator>().SetBool("Walking",false);
            GetComponent<Animator>().SetBool("Running",false);
            GetComponent<Animator>().SetBool("Dead",false);
            GetComponent<Animator>().SetTrigger("BeginAttack");
        }
        else if (action == "Die") {
            GetComponent<Animator>().SetBool("AllowIdle",false);
            GetComponent<Animator>().SetBool("Walking",false);
            GetComponent<Animator>().SetBool("Running",false);
            GetComponent<Animator>().SetBool("Dead",true);
            GetComponent<Animator>().ResetTrigger("BeginAttack");
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
        //Health = maxHealth;
        loot = GetComponent<Loot>();
        loot.Setup("Coin");
        health = GetComponent<Health>();
        health.Setup(maxHealth:15);
        player = GameObject.FindWithTag("MainPlayer");
        teethPoint = transform.Find("TeethPoint").gameObject;
        bloodGO = teethPoint.transform.Find("BloodSprayEffect").gameObject;
        bloodGO.SetActive(false);
        spawnPoint = transform.position;
        //gameObject.GetComponent<Animator>().SetBool("AllowIdle",true);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    
    void TurnTowardPlayer() {
        SetAction("Walking");
            
        Vector3 targetDir = player.transform.position - transform.position;
        float step = turnSpeed * Time.deltaTime;
        Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0F);
        Quaternion newRotation = Quaternion.LookRotation(newDir);
        newRotation.x = 0.0f;
        newRotation.z = 0.0f;
        
        
        transform.rotation = newRotation;
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
    
    void Bleed() {
        bloodGO.SetActive(true);
        Invoke("StopBleeding",bleedTime);
    }
    void StopBleeding() {
        bloodGO.SetActive(false);
    }
    
    
    void FixedUpdate() {
        if (IsAttacking) {
            return;
        }
        float distToPlayer = Vector3.Distance(transform.position,player.transform.position);
        if (distToPlayer < distToAttack) {
            //gameObject.GetComponent<Animator>().SetBool("AllowIdle",false);
            GetComponent<UnityEngine.AI.NavMeshAgent>().isStopped = true;
            if (!LookingNearPlayer()) {
                TurnTowardPlayer();
            }
            else {
                BeginAttack();
            }
        }
        else if (distToPlayer < distToNotice) {
            GetComponent<UnityEngine.AI.NavMeshAgent>().isStopped = false;
            GetComponent<UnityEngine.AI.NavMeshAgent>().destination = player.transform.position;
            GetComponent<UnityEngine.AI.NavMeshAgent>().speed = runSpeed;
            if (!IsRunning) {
                SetAction("Dash");
            }
        }
        else {
            //gameObject.GetComponent<Animator>().SetBool("AllowIdle",true);
            ContinueRoaming();
        }
    }
    
    void ContinueRoaming() {
        if (GetComponent<UnityEngine.AI.NavMeshAgent>().remainingDistance < 2) {
            GetComponent<UnityEngine.AI.NavMeshAgent>().destination = GetNewRoamDestination();
            GetComponent<UnityEngine.AI.NavMeshAgent>().speed = walkSpeed;
            SetAction("Walking");
        }
    }
    
    void BeginAttack() {
        if (!IsAttacking) {
            SetAction("Attack");
            Invoke("TestHit",0.5f);
            isAttacking = true;
        }
    }
    
    void TestHit() {
        float dist = Vector3.Distance(transform.position,player.transform.position);
        if (dist < distToDamage) {
            player.GetComponent<Health>().GetDamaged(attackLevel);
            Bleed();
            AudioSource.PlayClipAtPoint(successBiteSound,teethPointLocation);
        }
        else {
            AudioSource.PlayClipAtPoint(failBiteSound,teethPointLocation);
        }
        isAttacking = false;
    }
    
    void GetClickedOn(GameObject player) {
        BeginAttack();
    }
    
    public void Respawn() {
        health.Respawn();
    }
    
    public void GetSwungAt(GameObject player) {
        int damageToTake = player.GetComponent<PlayerBehavior>().AttackLevel;
        health.GetDamaged(damageToTake);
        
    }
    
    void Die() {
        PlayerBehavior player = GameObject.FindWithTag("MainPlayer").GetComponent<PlayerBehavior>();
        loot.ReleaseLoot();
        gameObject.SetActive(false);
    }
}
