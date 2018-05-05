using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using UnityEditor;
using UnityStandardAssets.Characters.FirstPerson;


public class PlayerBehavior : MonoBehaviour, WaterEnterer {

    public Inventory inventory;
    public HUD Hud;
    public CameraBehavior Cam;
    
    public Animator HandAnimator;
    private int currentUpdate = 0;
    private int hungerDecreasePeriod = 500;
    private int thirstDecreasePeriod = 200;
    private int oxygenDecreasePeriod = 20;
    
    [SerializeField]
    private AudioClip NatureSound;
    [SerializeField]
    private AudioClip UnderwaterSound;
    [SerializeField]
    private AudioClip DiveUnderwaterSound;
    [SerializeField]
    private AudioClip SurfaceUnderwaterSound;
    [SerializeField]
    private AudioClip ErrorSound;
    [SerializeField]
    private AudioClip BuildingSound;
    [SerializeField]
    private AudioClip CraftingSound;
    [SerializeField]
    private AudioClip TradingSound;
    [SerializeField]
    private AudioClip HurtSound;
    [SerializeField] public AudioClip[] FootstepSounds;
    public AudioClip JumpSound;
    public AudioClip LandSound;
    
    public bool noclip = false;
    public bool godmode {get { return health.godmode;} set { health.godmode = value;}}
    
    FirstPersonController fpc;
    
    [SerializeField]
    AudioSource backgroundAudioPlayer;
    [SerializeField]
    AudioSource fxAudioPlayer;
    
    public void PlayBackgroundAudio(AudioClip audio, bool repeat = true) {
        backgroundAudioPlayer.clip = audio;
        backgroundAudioPlayer.Play();
        backgroundAudioPlayer.loop = repeat;
    }
    public void PlayFXAudio(AudioClip audio, bool repeat = false) {
        fxAudioPlayer.clip = audio;
        fxAudioPlayer.Play();
        fxAudioPlayer.loop = repeat;
    }
    public void PlayFXAudio(AudioClip[] audioClips, bool repeat = false) {
        PlayFXAudio(audioClips[MakuUtil.rnd.Next(audioClips.Length)]);
    }
    
    public bool HeadUnderwater = false;
    
    public void OnHeadUnderwater() {
        PlayFXAudio(DiveUnderwaterSound);
        PlayBackgroundAudio(UnderwaterSound);
        HeadUnderwater = true;
    }
    public void OnHeadOverwater() {
        PlayFXAudio(SurfaceUnderwaterSound);
        PlayBackgroundAudio(NatureSound);
        HeadUnderwater = false;
    }
    
    public float WalkSpeed {
        get { return fpc.WalkSpeed;}
        set { fpc.WalkSpeed = value;}
    }
    public float RunSpeed {
        get { return fpc.RunSpeed;}
        set { fpc.RunSpeed = value;}
    }
    
    static int scrollSensitivity = 10;
    
    public int Money {
        get {
            return inventory.CountOf("Coin");
        }
        set {
            inventory.SetCount("Coin",value);
        }
    }
    
    public Health health;
    
    private int attackLevel;
    public int AttackLevel {
        get {
            return attackLevel;
        }
        set {
            attackLevel = value;
            Hud.SetAttackStat(value);
        }
    }
    
    bool inWater;
    public bool InWater{
        get {
            return inWater;
        }
        set {
            inWater = value;
        }
    }
    public void OnWaterEnter() {
        InWater = true;
    }
    public void OnWaterExit() {
        InWater = false;
    }
    
    public GameObject HeldItemObject {
        get {
            return EquippedContainer.transform.Cast<Transform>().Select(t=>t.gameObject).First(go => go.activeInHierarchy);
        }
    }
            
    
    public GameObject EquippedContainer;
    
    public GameObject Arms;
    public GameObject HandLeft;
    public GameObject HandRight;
    
    public bool UsingHands = true;
    
    public bool IsSwinging {
        get {
            return HandAnimator.GetCurrentAnimatorStateInfo(0).IsName("punch") || HandAnimator.GetCurrentAnimatorStateInfo(0).IsName("swingWeapon");
        }
    }
    
    public bool IsSwingingValid {
        get {
            if (!IsSwinging) {
                return false;
            }
            else {
                float aniPct = HandAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;
                return aniPct > 0.35 && aniPct < 0.5; //It's in the swinging part of the animation
            }
        }
    }
    
    void CorrectHeldItems() {
        foreach(Transform transChild in EquippedContainer.transform) {
            try {
                transChild.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
            }
            catch(MissingComponentException) {}
        }
    }
    
    public void SetEquippedItem(string item) {
        bool wasFound = false;
        
        foreach(Transform transChild in EquippedContainer.transform) {
            if (transChild.gameObject.name == item) {
                wasFound = true;
                transChild.gameObject.SetActive(true);
                try {
                    AttackLevel = transChild.gameObject.GetComponent<HeldWeapon>().AttackLevel;
                }
                catch(NullReferenceException) {
                    AttackLevel = 1;
                }
            }
            else {
                transChild.gameObject.SetActive(false);
            }
        }
        
        if (wasFound) {
            UsingHands = false;
            HandAnimator.SetBool("holdingWeapon",true);
        }
        else {
            //load resource and equip
            try {
                GameObject newHeld = Instantiate(Resources.Load("Equipables/"+item) as GameObject);
                wasFound = true;
                newHeld.name = item;
                newHeld.transform.SetParent(EquippedContainer.transform,false);
                newHeld.SetActive(true);
                try {
                    AttackLevel = newHeld.GetComponent<HeldWeapon>().AttackLevel;
                }
                catch(NullReferenceException) {
                    AttackLevel = 1;
                }
                UsingHands = false;
                HandAnimator.SetBool("holdingWeapon",true);
                
            }
            catch(ArgumentException) {
                //Item doesn't have an equippable version
                UsingHands = true;
                HandAnimator.SetBool("holdingWeapon",false);
                AttackLevel = 1;
            }
        }
    }
    
    
    public string CurrentItem {
        get {
            return inventory.CurrentlySelectedItem;
        }
        set {
            inventory.CurrentlySelectedItem = value;
        }
    }
    
    void Awake() {
        Arms = GameObject.FindWithTag("Arms");
        HandLeft = GameObject.FindWithTag("HandLeft");
        HandRight = GameObject.FindWithTag("HandRight");
        HandAnimator = HandRight.GetComponent<Animator> ();
        Hud = GetComponent<HUD>();
        Cam = GetComponentInChildren<CameraBehavior>();
        health = GetComponent<Health>();
        health.Setup(maxHealth:100,isPlayer:true, hud:Hud,deathMethod:Die);
        fpc = GetComponent<FirstPersonController>();
        
    }
	// Use this for initialization
	void Start () {
        
        inventory = gameObject.GetComponent<Inventory>();
        EquippedContainer = GameObject.FindWithTag("EquipContainer").gameObject;
        SetEquippedItem("Hands");
        CorrectHeldItems();
        HungerLevel = 100;
        ThirstLevel = 100;
        OxygenLevel = 100;
        PlayBackgroundAudio(NatureSound);
	}
    
    public bool MovementLocked {
        get {
            return Hud.ConsoleMenuOpen;
        }
    }
	
	bool wasSuccessfulLastUpdate = false;
	void Update () {
        if(Hud.ConsoleMenuOpen) {
            return; //Console takes priority
        }
        if (inventory.PreparingBuild) {
            if (Input.GetButtonDown("Fire1")) {
                if(wasSuccessfulLastUpdate) {
                    inventory.CompleteBuildItem();
                }
                else {
                    //play failure sound
                }
            }
            else {
                float scrollChange = Input.GetAxis("Mouse ScrollWheel");
                inventory.PrepareBuildObject.transform.Rotate(new Vector3(0,scrollChange*scrollSensitivity,0));
                RaycastHit hit = Cam.GetRayHit();
                if(hit.collider != null && hit.collider.gameObject.CompareTag("Ground")) {
                    Vector3 newPosition = hit.point;
                    newPosition.y = newPosition.y + inventory.PrepareBuildOffset;
                    inventory.PrepareBuildObject.transform.position = newPosition;
                    wasSuccessfulLastUpdate = true;
                }
                else {
                    wasSuccessfulLastUpdate = false;
                    //Make it red or something
                }
            }
            return;
        }
        if (!Hud.UsingUI && Input.GetButtonDown("Fire1")) {
            RaycastHit hit = Cam.GetRayHit();
            try {
                ClickOn(hit.transform.gameObject);
            }
            catch (NullReferenceException) {/*Nothing was hit, don't need to do anything*/}
        }
        if (!Hud.UsingUI && Input.GetButtonDown("Fire2")) { 
            UseItem();
        }
        if (Input.GetKeyDown(KeyCode.Tab)) {
            if(Hud.AnySpecialPanelOpen()) {
                Hud.CloseAllUI();
            }
            else {
                Hud.InventoryMenuOpen = !Hud.InventoryMenuOpen;
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Hud.PauseMenuOpen = !Hud.PauseMenuOpen;
        }
        for (int i=0;i<10;i++) {
            if (Input.GetKeyDown(i.ToString())) {
                inventory.CurrentlySelectedSlot = i-1; //minus 1 so that 1 is the first element rather than 0
            }
        }
		
	}
    
    void FixedUpdate() {
        currentUpdate++;
        if (currentUpdate % hungerDecreasePeriod == 0) {
            HungerLevel--;
        }
        if (currentUpdate % thirstDecreasePeriod == 0) {
            ThirstLevel--;
        }
        if(currentUpdate % oxygenDecreasePeriod == 0) {
            if(HeadUnderwater) {
                OxygenLevel--;
            }
            else {
                OxygenLevel += 5;
            }
        }
        if(!Hud.UsingUI) {
            RaycastHit hit = Cam.GetRayHit();
            try {
                Hud.InfoStr = hit.transform.gameObject.name;
            }
            catch(NullReferenceException) {
                Hud.InfoStr = "";
            }
        }
    }
    
    private int hungerLevel;
    public int HungerLevel {
        get { return hungerLevel;}
        set {
            hungerLevel = value;
            if (hungerLevel > 100) {
                hungerLevel = 100;
            }
            if (hungerLevel <= 0) {
                hungerLevel = 0;
                Die();
            }
            Hud.HungerText.GetComponent<Text>().text = hungerLevel.ToString();
        }
    }
    private int thirstLevel;
    public int ThirstLevel {
        get { return thirstLevel;}
        set {
            thirstLevel = value;
            if (thirstLevel > 100) {
                thirstLevel = 100;
            }
            if (thirstLevel <= 0) {
                thirstLevel = 0;
                Die();
            }
            Hud.ThirstText.GetComponent<Text>().text = thirstLevel.ToString();
        }
    }
    private int oxygenLevel;
    public int OxygenLevel {
        get { return oxygenLevel;}
        set {
            oxygenLevel = value;
            if(oxygenLevel > 100) {
                oxygenLevel = 100;
            }
            if(oxygenLevel <= 0) {
                oxygenLevel = 0;
                Die();
            }
            Hud.OxygenText.GetComponent<Text>().text = oxygenLevel.ToString();
        }
    }
    
    void UseItem() {
        if (UsingHands) {
            //swing with fists
            HandAnimator.Play("punch",0);
        }
        else {
            ItemActivator itemActivator = HeldItemObject.GetComponent<ItemActivator>();
            if (itemActivator == null) {
                //swing with fists
                HandAnimator.Play("punch",0);
            }
            else {
                itemActivator.ActivateItem(); //Searches for the interface and activates the corresponding method
            }
        }
    }
    
    void ClickOn(GameObject obj) {
        obj.SendMessage("GetClickedOn",this.gameObject,SendMessageOptions.DontRequireReceiver);
        
    }
    
    void OnGUI(){
        GUI.Box(new Rect(Screen.width/2,Screen.height/2, 10, 10), ""); //Drawing crosshair
    }
    
    public void Die() {
        print("YOU HAVE DIED");
    }
}