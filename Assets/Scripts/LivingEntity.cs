using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.Linq;
using UnityEditor;


public enum Factions { None, player, wolf, };

public class LivingEntity : MonoBehaviour, ContainerInt{

    public int attackLevel = 2;
    
    protected float distToNotice = 15.0f;
    protected float distToAttack = 0.04f;
    protected float distToDamage = 0.5f;
    
    protected int foodEaten = 0;
    
    public Factions Fac;
    
    protected  GameObject targetEnemy = null;
    protected  GameObject targetFood = null;
    
    protected Collider mainBodyCol = null;
    public Collider MainBodyCol { get{return mainBodyCol;}}
    
    protected UnityEngine.AI.NavMeshAgent nav;
    
    protected ContainerCls container;
    
    protected Animator ani;
    
    protected GameObject mouthGO;
    
    protected Collider hitbox;
    
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
    
    public Factions[] GetEnemyFactions() {
        return enemyFactions[Fac];
    }
    public static Dictionary<Factions,Factions[]> enemyFactions = new Dictionary<Factions,Factions[]>(){
        {Factions.player, new Factions[]{Factions.wolf,}},
        {Factions.wolf, new Factions[]{Factions.player,}},
        {Factions.None,new Factions[]{}},
    };
    
    
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
        
    
    
    
    protected void Prepare(float distToNotice = 15.0f, float distToAttack = 1.0f, float distToDamage = 0.8f, float walkSpeed = 2, float runSpeed = 5, float turnSpeed = 3, float maxRoamDistance = 20, int attackLevel = 2, int maxHealth = 100,Health.DamageTypes[] immunities = null, string drop = "1*Coin", bool doesRespawn = false, Factions fac = Factions.None) {
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
        Invoke("TestForNearbyTargets",0.1f);
        container = gameObject.AddComponent<ContainerCls>();
        container.Prepare();
        PrepareEntityInventory(drop);
        foreach (Transform keyTest in gameObject.GetComponentsInChildren<Transform>()) {
            if (keyTest.CompareTag("MainBodyCollider")) {
                mainBodyCol = keyTest.gameObject.GetComponent<Collider>();
            }
            if(keyTest.CompareTag("Hitbox")) {
                this.hitbox = keyTest.gameObject.GetComponent<Collider>();
            }
        }
        hitbox = hitbox??GetComponent<Collider>();
        mainBodyCol = mainBodyCol??GetComponent<Collider>();
        this.Fac = fac;
    }
    
    
    protected void OnTriggerEnter() {
        
    }
    
    protected virtual void FixedUpdate() {
        //GetComponent<Rigidbody>().AddForce(9.8f*Time.deltaTime*-Vector3.up);
        //nav.velocity = nav.velocity + GetComponent<Rigidbody>().velocity;
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
                ani.SetBool("Walking",false);
                ani.SetBool("Running",false);
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
        if(!nav.enabled) {
            return;
        }
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
    
    protected void TurnToward(GameObject destGO) {
        TurnToward(destGO.transform.position);
    }
    protected void TurnToward(Vector3 destVec){
        InstantlyTurn(destVec);
    }
    protected void TurnTowardTarget() {
        InstantlyTurn(nav.destination);
    }
    protected bool LookingAt(GameObject destGO) {
        return LookingAt(destGO.transform.position);
    }
    protected bool LookingAt(Vector3 destination) {
        Vector3 destVec = destination;
        Vector3 myVec = transform.position;
        destVec.y = 0;
        myVec.y = 0;
        float angleBetween = Vector3.Angle((destVec-myVec),transform.forward);
        return angleBetween < 1;
    }
    
     private void InstantlyTurn(Vector3 destination) {
         //When on target -> dont rotate!
         if (LookingAt(destination)) return; 
         
         aniAction = aniActions.Walking;
         
         
         Vector3 direction = (destination - transform.position).normalized;
         Quaternion  qDir= Quaternion.LookRotation(direction);
         transform.rotation = Quaternion.Slerp(transform.rotation, qDir, Time.deltaTime * turnSpeed);
         
     }
    protected float DistToDest() {
        Vector3 closestBodyPoint = hitbox.ClosestPointOnBounds(nav.destination);
        return Vector3.Distance(closestBodyPoint, nav.destination);
    }
    protected float DistToGO(GameObject other) {
        Vector3 myClosest = hitbox.ClosestPointOnBounds(other.transform.position);
        Vector3 otherClosest = other.GetComponent<Collider>().ClosestPointOnBounds(myClosest);
        return Vector3.Distance(myClosest,otherClosest);
    }
    protected float DistToGOSimple(GameObject other) { //Should return same result as DistToDest(). DistToGO gives a more accurate result.
        Vector3 closestBodyPoint = hitbox.ClosestPointOnBounds(other.transform.position);
        return Vector3.Distance(closestBodyPoint, other.transform.position);
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
        transform.rotation = Quaternion.identity; //Set rotation to (0,0,0). Messes bounds up if you don't.
        BoxCollider bc;
        if (hitbox != null) {
            bc = (BoxCollider)hitbox;
        }
        else {
            bc = gameObject.GetComponent<BoxCollider>();
        }
        Bounds bounds = new Bounds (Vector3.zero, Vector3.zero);
        bool hasBounds = false;
        Renderer[] renderers =  gameObject.GetComponentsInChildren<Renderer>();
        foreach(SkinnedMeshRenderer ren in gameObject.GetComponentsInChildren<SkinnedMeshRenderer>()) {
            ren.updateWhenOffscreen = true;
        }
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
        foreach(SkinnedMeshRenderer ren in gameObject.GetComponentsInChildren<SkinnedMeshRenderer>()) {
            ren.updateWhenOffscreen = false;
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
        Invoke("FitColliderToChildren",1);
        if(doesRespawn) {
            Invoke("Respawn",respawnTime);
        }
        else {
            
        }
    }
    
    /*protected void OnCollisionEnter(Collision col) {
        if(col.gameObject.CompareTag("Ground")) {
            SwitchToNav();
        }
    }
    protected void OnCollisionExit(Collision col) {
        if(col.gameObject.CompareTag("Ground")) {
            SwitchToRigidbody();
        }
    }
    
    protected void SwitchToRigidbody() {
        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<Rigidbody>().isKinematic = false;
        nav.enabled = false;
    }
    protected void SwitchToNav() {
        Vector3 prevPos = transform.position;
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Rigidbody>().isKinematic = true;
        nav.enabled = true;
        transform.position = prevPos;
    }*/
    
    
    
    
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
