using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfBehavior : MonoBehaviour {

    public int attackLevel = 2;
    
    private int distToNotice = 15;
    private float distToAttack = 1.0f;
    private float distToDamage = 0.8f;
    
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
    
    
    private Animator ani {
        get {
            return GetComponent<Animator>();
        }
    }
    
    private GameObject teethPoint;
    private Vector3 teethPointLocation {
        get {
            return teethPoint.transform.position;
        }
    }
    
    private GameObject bloodGO;
    
    private string drop = "1*Coin,2*UncookedMeat";
    
    private enum aniActions{Unknown,Idle,Walking,Running,Dead,Attacking};
    private aniActions aniAction {
        get {
            AnimatorStateInfo aniInfo = ani.GetCurrentAnimatorStateInfo(0);
            if (aniInfo.IsTag("attack")) {return aniActions.Attacking;}
            else if (aniInfo.IsTag("walk")) {return aniActions.Walking;}
            else if (aniInfo.IsTag("run")) {return aniActions.Running;}
            else if (aniInfo.IsTag("die")) {return aniActions.Dead;}
            else if (aniInfo.IsTag("idle")) {return aniActions.Idle;}
            else {
                Debug.LogWarning("Tried to get aniAction but didn't match any");
                return aniActions.Unknown;
            }
        }
        set {
            if (value == aniActions.Walking) {
                ani.SetBool("AllowIdle",false);
                ani.SetBool("Walking",true);
                ani.SetBool("Running",false);
                ani.SetBool("Dead",false);
            }
            else if (value == aniActions.Idle) {
                ani.SetBool("AllowIdle",true);
                ani.SetBool("Walking",false);
                ani.SetBool("Running",false);
                
            }
            else if (value == aniActions.Running) {
                ani.SetBool("AllowIdle",false);
                ani.SetBool("Walking",false);
                ani.SetBool("Running",true);
                ani.SetBool("Dead",false);
                
            }
            else if (value == aniActions.Attacking) {
                ani.Play("hit",0);
            }
            else if (value == aniActions.Dead) {
                ani.SetBool("AllowIdle",false);
                ani.SetBool("Walking",false);
                ani.SetBool("Running",false);
                ani.SetBool("Dead",true);
            }
            else {
                Debug.LogWarning("Tried to set action to "+value.ToString()+" but it doesn't exist");
            }
        }
    }
    
    private Vector3 GetNewRoamDestination() {
        int xDif = MakuUtil.rnd.Next(-maxRoamDistance,maxRoamDistance);
        int zDif = MakuUtil.rnd.Next(-maxRoamDistance,maxRoamDistance);
        Vector3 newPos = spawnPoint;
        newPos.x += xDif;
        newPos.z += zDif;
        return newPos;
    }
            
    
	// Use this for initialization
	void Start () {
        loot = GetComponent<Loot>();
        loot.Setup(drop);
        health = GetComponent<Health>();
        health.Setup(maxHealth:15,deathMethod:Die);
        teethPoint = transform.Find("TeethPoint").gameObject;
        bloodGO = teethPoint.transform.Find("BloodSprayEffect").gameObject;
        bloodGO.SetActive(false);
        spawnPoint = transform.position;
        nav = GetComponent<UnityEngine.AI.NavMeshAgent>();
        GetComponent<EnemyFinder>().Setup(distToNotice: this.distToNotice);
        TestForNearbyEnemies();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    
    
    float DistToDest() {
        return Vector3.Distance(teethPointLocation, nav.destination);
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
        aniAction = aniActions.Walking;
            
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
        float distToDest = DistToDest();
        bool closeEnough = distToDest < distToAttack;
        if(closeEnough) {
            nav.isStopped = true;
            aniAction = aniActions.Idle;
        }
        else {
            nav.isStopped = false;
            nav.speed = isRunning?runSpeed:walkSpeed;
            if(isRunning) {
                aniAction = aniActions.Running;
            }
            else {
                aniAction = aniActions.Walking;
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
            aniAction = aniActions.Walking;
        }
    }
    
    void BeginAttack() {
        if (aniAction != aniActions.Attacking) {
            aniAction = aniActions.Attacking;
            Invoke("TestHit",0.5f);
        }
    }
    
    void OnCollisionEnter(Collision col) {
        
    }
    
    void TestHit() {
        //float dist = Vector3.Distance(transform.position,player.transform.position);
        if (targetEnemy == null) {
            return;
        }
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
    }
    
    
    public void Respawn() {
        health.Respawn();
    }
    
    void Die() {
        loot.ReleaseLoot();
        gameObject.SetActive(false);
    }
}
