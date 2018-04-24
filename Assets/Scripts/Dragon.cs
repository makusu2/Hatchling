using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Dragon : LivingEntity {

    private float attackCooldown = 0.2f; //Should probably be higher for the real game
    

    private GameObject body;
    public GameObject Body {get {return body;}}
    public GameObject BodyMesh {
        get {
            return body.transform.Find(Stage.ToString()+"BodyMesh").gameObject;
        }
    }
    
    private readonly int newbornFoodToEvolve = 10;
    private readonly float heatLevelToEvolve = 0.6f;
    
    private DragonStage stage;
    public DragonStage Stage {
        get {return stage;}
        set {
            aniActions previousAction = aniAction;
            if (previousAction == aniActions.Unknown) {
                previousAction = aniActions.Idle;
            }
            transform.rotation = Quaternion.identity; //Set rotation to (0,0,0). Messes bounds up if you don't.
            stage = value;
            string goName = stage.ToString()+"Body";
            Transform possibleTrans = transform.Find(goName);
            if(possibleTrans == null) {
                throw new ArgumentException();
            }
            foreach (Transform child in transform) {
                child.gameObject.SetActive(child == possibleTrans); //If it's the trans, active, otherwise inactive
            }
            body = possibleTrans.gameObject;
            ani = body.GetComponent<Animator>();//update animator
            try {
                mouthGO = body.transform.Find("MouthLocation").gameObject;
            }
            catch(NullReferenceException) {
                //Mouth position wasn't found
            }
            FitColliderToChildren();
            aniAction = previousAction; //Make the new animator play the current animation;
            
            if(stage == DragonStage.Egg) {
                GetComponent<Faction>().faction = Factions.None;
            }
            else {
                GetComponent<Faction>().faction = Factions.player;
            }
        }
    }
    
    
    
    public float BleedTime = 0.5f;
    
    
    
    
    private float lastAttackTime;
    public bool OnCooldown {
        get {
            return Time.time - lastAttackTime < attackCooldown;
        }
    }
           
	// Use this for initialization
	void Start () {
        base.Prepare(distToNotice:30,walkSpeed:5,runSpeed:8,turnSpeed:10,maxRoamDistance:20,maxHealth:100, immunities:new Health.DamageTypes[]{Health.DamageTypes.fire});
        lastAttackTime = Time.time - attackCooldown;
        Stage = DragonStage.Egg;
        Invoke("CheckGrowDragon",5);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    
    
    
    void FixedUpdate() {
        if (Stage == DragonStage.Egg || OnCooldown || IsDead) {
            return;
        }
        else {
            UpdateAllTargets();
            if (targetEnemy != null) {
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
            else if (targetFood != null){
                SetDestination(targetFood.transform.position,isRunning:false);
                if (DistToDest() < 2) {
                    nav.isStopped = true;
                    if (LookingNearTarget()) {
                        EatFood();
                    }
                    else {
                        TurnTowardTarget();
                    }
                }
            }
            else {
                ContinueRoaming();
            }
        }
    }
    
    
    protected override void SetDestination(Vector3 position, bool isRunning = true) {
        if (!MobileStage()) {
            return;
        }
        else {
            base.SetDestination(position,isRunning:isRunning);
        }
    }
    
    
    void BeginAttack() {
        if (!OnCooldown) {
            aniAction = aniActions.Attacking;
            ThrowFireball();
            lastAttackTime = Time.time;
        }
    }
    
    protected override void Die() {
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
    
    
   
    public enum DragonStage { Egg, Newborn, Toddler, };
    public static string NameOfStage(int index) {
        return Enum.GetName(typeof(DragonStage), index);
    }
    public bool CanEvolve() {
        switch(Stage) {
            case DragonStage.Egg:
                return GetCurrentHeatLevel() >= heatLevelToEvolve;
            case DragonStage.Newborn:
                return foodEaten >= newbornFoodToEvolve;
            case DragonStage.Toddler:
                return false;
            default:
                Debug.LogError("No evolution condition for "+Stage.ToString());
                return false;
        }
    }
    public bool MobileStage() {
        DragonStage[] mobileStages = new DragonStage[]{DragonStage.Newborn,DragonStage.Toddler}; //Can't make static cuz C# is dumb
        return mobileStages.Contains(Stage);
    }
    
}