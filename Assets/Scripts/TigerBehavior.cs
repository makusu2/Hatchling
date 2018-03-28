using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TigerBehavior : MonoBehaviour {

    
    public int attackLevel = 2;
    //public int defenseLevel = 2;
    public int maxHealth = 15;
    
    [SerializeField]
    private int distToNotice = 15;
    [SerializeField]
    private  int distToAttack = 2;
    [SerializeField]
    private int distToDamage = 2;
    
    //[SerializeField]
    //private float walkSpeed = 2;
    [SerializeField]
    private float runSpeed = 5;
    [SerializeField]
    private float turnSpeed = 3;
    
    
    public string drop = "Coin";
    
    private GameObject player;
    
    
    private float currentHealth;
    public float Health {
        get{return currentHealth;}
        set {
            if(value<=0) {
                currentHealth = 0;
                Die(player.GetComponent<PlayerBehavior>());
            }
            else {
                currentHealth = value;
            }
        }
    }
    
    public bool IsAttacking {
        get {
            return gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("hit") || gameObject.GetComponent<Animator>().GetBool("BeginAttack");
        }
        set{}
    }
    
    public bool IsWalking {
        get {
            return gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("walk") || gameObject.GetComponent<Animator>().GetBool("BeginWalk");
        }
        set {}
    }
            
    
	// Use this for initialization
	void Start () {
        Health = maxHealth;
        player = GameObject.FindWithTag("MainPlayer");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    
    void TurnTowardPlayer() {
        if (!IsWalking) {
            gameObject.GetComponent<Animator>().SetTrigger("BeginWalk");
        }
            
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
    
    void RunForward() {
        
    }
    
    void WalkForward() {
        
    }
    
    void StopWalking() {
        
    }
    
    void DoRunStepToPlayer() {
        float step = runSpeed * Time.deltaTime;
        Vector3 desiredPosition = player.transform.position;
        desiredPosition.y = 0;
        transform.position = Vector3.MoveTowards(transform.position, desiredPosition, step);
        gameObject.GetComponent<Animator>().SetTrigger("Dash");
    }
    
    
    void FixedUpdate() {
        if (IsAttacking) {
            return;
        }
        float distToPlayer = Vector3.Distance(transform.position,player.transform.position);
        if (distToPlayer < distToAttack) {
            if (!LookingNearPlayer()) {
                TurnTowardPlayer();
            }
            else {
                BeginAttack();
            }
        }
        else if (distToPlayer < distToNotice) {
            if (!LookingNearPlayer()) {
                TurnTowardPlayer();
            }
            else {
                DoRunStepToPlayer();
            }
        }
        else {
            //TODO do idle stuff
        }
    }
    
    void BeginAttack() {
        gameObject.GetComponent<Animator>().SetTrigger("BeginAttack");
        Invoke("TestHit",0.5f);
    }
    
    void TestHit() {
        float dist = Vector3.Distance(transform.position,player.transform.position);
        if (dist < distToDamage) {
            player.GetComponent<Health>().GetDamaged(attackLevel);
        }
    }
    
    void GetClickedOn(GameObject player) {
        BeginAttack();
    }
    
    public void GetSwungAt(GameObject player) {
        int damageToTake = player.GetComponent<PlayerBehavior>().AttackLevel;
        Health -= damageToTake;
        
    }
    
    void Die(PlayerBehavior player) {
        player.inventory.AddItem(drop);
        Destroy(gameObject);
    }
}
