using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.Linq;
using UnityEditor;

public class LivingEntity : MonoBehaviour, ContainerInt{

    public int attackLevel = 2;
    
    protected float distToNotice = 15.0f;
    protected float distToAttack = 1.0f;
    protected float distToDamage = 0.8f;
    
    protected int foodEaten = 0;
    
    
    protected  GameObject targetEnemy = null;
    protected  GameObject targetFood = null;
    
    protected UnityEngine.AI.NavMeshAgent nav;
    
    protected ContainerCls container;
    
    protected Animator ani;
    
    protected GameObject mouthGO;
    
    protected bool doesRespawn;
    protected int respawnTime = -1;
    
    protected float walkSpeed = 2;
    protected float runSpeed = 5;
    protected float turnSpeed = 3;
    protected float maxRoamDistance = 20;
    
    protected Vector3 spawnPoint;
    protected Health health;
    //protected Loot loot;
    
    public bool IsDead { get { return health.IsDead;}}
    
    protected static GameObject player;
    protected static HUD hud;
    
    
    protected void UpdateTargetFood() {
        if(targetFood == null) {
            GameObject targetGO = GetComponent<EnemyFinder>().GetClosestFood();
            targetFood = targetGO;
        }
    }
    
    protected void TestForNearbyTargets() {
        UpdateAllTargets();
        Invoke("TestForNearbyTargets",2);
    }
    
    
    protected void UpdateAllTargets() {
        UpdateTargetFood();
        UpdateTargetEnemy();
    }
    protected bool LookingNearTarget() {
        Vector3 targetDir = nav.destination - transform.position;
        Vector3 currentDir = transform.forward;
        targetDir.y = 0;
        currentDir.y = 0;
        float angleBetween = Vector3.Angle(targetDir,currentDir);
        
        bool lookingNearTarget = angleBetween < 10;
        return lookingNearTarget;
    }
    
   
   public void EatFood(GameObject foodGO = default(GameObject)) {
       if (foodGO == default(GameObject)) {
           foodGO = targetFood;
       }
       if(Vector3.Distance(foodGO.transform.position,transform.position) > 10) {
           Debug.LogWarning("Food was WAY too far away when hatchling tried to eat it!");
       }
       foodEaten += foodGO.GetComponent<PickupBehavior>().StackCount;
       foodGO.GetComponent<PickupBehavior>().GetTaken();
       if (foodGO == targetFood) {
           targetFood = null;
       }
       UpdateAllTargets();
   }
    
    
    protected void UpdateTargetEnemy() {
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
        
    
    
    
    
    protected void Prepare(float distToNotice = 15.0f, float distToAttack = 1.0f, float distToDamage = 0.8f, float walkSpeed = 2, float runSpeed = 5, float turnSpeed = 3, float maxRoamDistance = 20, int attackLevel = 2, int maxHealth = 100,Health.DamageTypes[] immunities = null, string drop = "1*Coin", bool doesRespawn = false) {
        this.distToNotice = distToNotice;
        this.distToAttack = distToAttack;
        this.distToDamage = distToDamage;
        this.walkSpeed = walkSpeed;
        this.runSpeed = runSpeed;
        this.turnSpeed = turnSpeed;
        this.maxRoamDistance = maxRoamDistance;
        this.doesRespawn = doesRespawn;
        
        try {
            ani = GetComponent<Animator>();
        }
        catch(MissingComponentException) {}
        nav = GetComponent<UnityEngine.AI.NavMeshAgent>();
        health = GetComponent<Health>();
        
        health.Setup(maxHealth:maxHealth,deathMethod:Die);
        //loot = GetComponent<Loot>();
        //if (loot != null) {
        //    loot.Setup(drop);
        //}
        GetComponent<EnemyFinder>().Setup(distToNotice: this.distToNotice);
        this.spawnPoint = transform.position;
        player = player??GameObject.FindWithTag("MainPlayer");
        hud = hud??player.GetComponent<HUD>();
        TestForNearbyTargets();
        container = gameObject.AddComponent<ContainerCls>();
        container.Prepare();
        PrepareEntityInventory(drop);
    }
    
    
    
    
    
    protected Vector3 GetNewRoamDestination() {
        float xDif = UnityEngine.Random.Range(-maxRoamDistance,maxRoamDistance);
        float zDif = UnityEngine.Random.Range(-maxRoamDistance,maxRoamDistance);
        Vector3 newPos = spawnPoint;
        newPos.x += xDif;
        newPos.z += zDif;
        return newPos;
    }
    protected enum aniActions{Unknown,Idle,Walking,Running,Dead,Attacking};
    protected aniActions aniAction {
        get {
            AnimatorStateInfo aniInfo;
            try {
                aniInfo = ani.GetCurrentAnimatorStateInfo(0);
            }
            catch(MissingComponentException) {
                return aniActions.Unknown;
            }
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
            if (ani == null) {
                return;
            }
            if (value == aniActions.Walking) {
                ani.SetBool("Walking",true);
                ani.SetBool("Running",false);
                ani.SetBool("Dead",false);
            }
            else if (value == aniActions.Idle) {
                ani.SetBool("Walking",false);
                ani.SetBool("Running",false);
                ani.SetBool("Dead",false);
                
            }
            else if (value == aniActions.Running) {
                ani.SetBool("Walking",false);
                ani.SetBool("Running",true);
                ani.SetBool("Dead",false);
                
            }
            else if (value == aniActions.Attacking) {
                ani.Play("hit",0);
            }
            else if (value == aniActions.Dead) {
                ani.SetBool("Walking",false);
                ani.SetBool("Running",false);
                ani.SetBool("Dead",true);
                ani.Play("die",0);
            }
            else {
                Debug.LogWarning("Tried to set action to "+value.ToString()+" but it doesn't exist");
            }
        }
    }
    
   
    protected virtual void SetDestination(Vector3 position, bool isRunning = true) {
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
    
    protected void TurnTowardTarget() {
        
        aniAction = aniActions.Walking;
            
        Vector3 targetDir = Vector3.Normalize(nav.destination - transform.position);
        float step = turnSpeed * Time.deltaTime;
        Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0F);
        Quaternion newRotation = Quaternion.LookRotation(newDir);
        newRotation.x = 0.0f;
        newRotation.z = 0.0f;
        transform.rotation = newRotation;
    }
    protected float DistToDest() {
        Vector3 closestBodyPoint = GetComponent<Collider>().ClosestPointOnBounds(nav.destination);
        return Vector3.Distance(closestBodyPoint, nav.destination);
    }
    protected void ContinueRoaming() {
        if (DistToDest() < 2) {
            SetDestination(GetNewRoamDestination(),isRunning:false);
        }
    }
    
    protected float GetCurrentHeatLevel() {
        float maxDist = 10;
        float minEquivDist = 1;
        int minHeatLevel = 1;
        Func<GameObject,bool> qualFunc = (GameObject go) => go.GetComponent<Heated>() != null && go.GetComponent<Heated>().HeatLevel >= minHeatLevel;
        IEnumerable<GameObject> fireyGOs = GetComponent<EnemyFinder>().GetNearbyWithProp(qualFunc, requiredDist:maxDist);
        float totalHeat = 0;
        foreach (GameObject fireyGO in fireyGOs) {
            float dist = Math.Max(minEquivDist,Vector3.Distance(transform.position,fireyGO.transform.position));
            totalHeat += fireyGO.GetComponent<Heated>().HeatLevel / dist;
        }
        return totalHeat;
    }
    
    protected void FitColliderToChildren ()
    {
        BoxCollider bc = gameObject.GetComponent<BoxCollider>();
        Bounds bounds = new Bounds (Vector3.zero, Vector3.zero);
        bool hasBounds = false;
        Renderer[] renderers =  gameObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer render in renderers) {
            if (hasBounds) {
                bounds.Encapsulate(render.bounds);
            } 
            else {
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
    
    public void Respawn() {
        /*ani.enabled = true;
        foreach(Rigidbody rb in GetComponentsInChildren<Rigidbody>()) {
            rb.isKinematic = true;
        }*/
        aniAction = aniActions.Idle;
        
        health.Respawn();
    }
    
    protected virtual void Die() {
        aniAction = aniActions.Dead;
        if(doesRespawn) {
            Invoke("Respawn",respawnTime);
        }
        else {
            
        }
    }
    
    
    
    
    void GetClickedOn(GameObject player) {
        if(IsDead) {
            hud.CurrentChest = this.gameObject;
            hud.ChestMenuOpen = true;
        }
    }
    
    
    
    
    //Attacks
    
    protected void ThrowFireball() {
        GameObject fireball = Instantiate(Resources.Load("InWorld/Fireball") as GameObject);
        fireball.SetActive(true);
        fireball.transform.position = mouthGO.transform.position;
        fireball.GetComponent<FireballBehavior>().GetShot(transform.forward);
    }

    public int CountOf(string item) { return container.CountOf(item);}
    public void ItemMovedTo(string item) { container.ItemMovedTo(item);}
    public void ItemTakenFrom(string item) { container.ItemTakenFrom(item);}
    public void CreateIcons() { container.CreateIcons();}
    public void DestroyIcons() { container.DestroyIcons();}
    public void AddItem(string name) { container.AddItem(name);}
    public void RemoveItem(string name) { container.RemoveItem(name);}
    public void PrepareEntityInventory(string loot) { container.PrepareEntityInventory(loot);}
    
    
    
    
    
    
}
