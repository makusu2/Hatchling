using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfBehavior : MonoBehaviour {

    public int attackLevel = 2;
    //public int defenseLevel = 2;
    public int maxHealth = 15;
    
    [SerializeField]
    private int distToNotice = 15;
    [SerializeField]
    private  int distToAttack = 2;
    [SerializeField]
    private int distToDamage = 2;
    
    [SerializeField]
    private float walkSpeed = 2;
    [SerializeField]
    private float runSpeed = 5;
    [SerializeField]
    private float turnSpeed = 3;
    [SerializeField]
    private int maxRoamDistance = 20;
    
    private Vector3 spawnPoint;
    
    private float bleedTime = 0.5f;
    
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
    
    private bool isAttacking = false;
    public bool IsAttacking {
        get {
            return isAttacking || gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Base.hit") || gameObject.GetComponent<Animator>().GetBool("BeginAttack");
        }
        set{}
    }
    
    public bool IsWalking {
        get {
            return gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Base.walk") || gameObject.GetComponent<Animator>().GetBool("BeginWalk");
        }
        set {}
    }
    
    public bool IsRunning {
        get {
            return gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Base.run") || gameObject.GetComponent<Animator>().GetBool("Dash");
        }
        set {}
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
        teethPoint = transform.Find("TeethPoint").gameObject;
        bloodGO = teethPoint.transform.Find("BloodSprayEffect").gameObject;
        bloodGO.SetActive(false);
        spawnPoint = transform.position;
        gameObject.GetComponent<Animator>().SetBool("AllowIdle",false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    
    void TurnTowardPlayer() {
        if (!IsWalking && !IsRunning) {
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
            gameObject.GetComponent<Animator>().SetBool("AllowIdle",false);
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
            gameObject.GetComponent<Animator>().SetBool("AllowIdle",false);
            if (!IsRunning) {
                gameObject.GetComponent<Animator>().SetTrigger("Dash");
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
            GetComponent<Animator>().SetTrigger("BeginWalk");
        }
    }
    
    void BeginAttack() {
        gameObject.GetComponent<Animator>().ResetTrigger("Dash");
        gameObject.GetComponent<Animator>().ResetTrigger("BeginWalk");
        if (!IsAttacking) {
            gameObject.GetComponent<Animator>().SetTrigger("BeginAttack");
            Invoke("TestHit",0.5f);
            isAttacking = true;
        }
    }
    
    void TestHit() {
        float dist = Vector3.Distance(transform.position,player.transform.position);
        if (dist < distToDamage) {
            player.GetComponent<PlayerBehavior>().GetDamaged(attackLevel);
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
        Health = maxHealth;
    }
    
    public void GetSwungAt(GameObject player) {
        int damageToTake = player.GetComponent<PlayerBehavior>().AttackLevel;
        Health -= damageToTake;
        
    }
    
    void Die(PlayerBehavior player) {
        player.inventory.AddItem(drop);
        gameObject.SetActive(false);
    }
}
