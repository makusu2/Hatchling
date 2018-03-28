using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfBehavior : MonoBehaviour {

    public int attackLevel = 2;
    
    private int distToNotice = 15;
    private int distToAttack = 2;
    private float distToDamage = 0.1f;
    
    private UnityEngine.AI.NavMeshAgent nav;
    
    private Health health;
    
    private float walkSpeed = 2;
    private float runSpeed = 5;
    private float turnSpeed = 3;
    private int maxRoamDistance = 20;
    
    private Vector3 spawnPoint;
    
    private float bleedTime = 0.5f;
    
    private GameObject targetEnemy = null;
    
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
        else if (action == "Running") {
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
        else {
            Debug.LogWarning("Tried to set action to "+action+" but it doesn't exist");
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
        nav = GetComponent<UnityEngine.AI.NavMeshAgent>();
        GetComponent<EnemyFinder>().Setup(distToNotice: this.distToNotice);
        TestForNearbyEnemies();
        //gameObject.GetComponent<Animator>().SetBool("AllowIdle",true);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    
    
    float DistToDest() {
        return nav.pathPending?Vector3.Distance(transform.position, nav.destination):nav.remainingDistance;
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
                if(DistToDest() < 2) {
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
        
    
    void TestForNearbyEnemies() {
        UpdateTargetEnemy();
        Invoke("TestForNearbyEnemies",2);
    }
        
    
    void UpdateTargetEnemy() {
        if (targetEnemy != null) {
            if (targetEnemy.GetComponent<Health>().IsDead || Vector3.Distance(targetEnemy.transform.position,transform.position) > distToNotice) {
                targetEnemy = null;
            }
        }
        if (targetEnemy == null) {
            GameObject targetGO = GetComponent<EnemyFinder>().GetClosestEnemy();
            targetEnemy = targetGO;
        }
    }
        
    
    void SetDestination(Vector3 position, bool isRunning = true) {
        nav.destination = position;
        
        bool closeEnough = DistToDest() < distToAttack;
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
        
    
    
    bool LookingNearTarget() {
        Vector3 targetDir = targetEnemy.transform.position - transform.position;
        Vector3 currentDir = transform.forward;
        targetDir.y = 0;
        currentDir.y = 0;
        float angleBetween = Vector3.Angle(targetDir,currentDir);
        
        bool lookingNearTarget = angleBetween < 10;
        return lookingNearTarget;
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
        //float dist = Vector3.Distance(transform.position,player.transform.position);
        Vector3 targetPos = targetEnemy.GetComponent<Collider>().ClosestPointOnBounds(teethPointLocation);
        float dist = Vector2.Distance(new Vector2(targetPos.x,targetPos.z),new Vector2(teethPointLocation.x,teethPointLocation.z));
        if (dist < distToDamage) {
            targetEnemy.GetComponent<Health>().GetDamaged(attackLevel);
            Bleed();
            AudioSource.PlayClipAtPoint(successBiteSound,teethPointLocation);
        }
        else {
            AudioSource.PlayClipAtPoint(failBiteSound,teethPointLocation);
        }
        isAttacking = false;
    }
    
    
    public void Respawn() {
        health.Respawn();
    }
    
    void Die() {
        loot.ReleaseLoot();
        gameObject.SetActive(false);
    }
}
