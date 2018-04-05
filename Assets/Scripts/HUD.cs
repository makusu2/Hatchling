using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.IO;

public class HUD : MonoBehaviour {
    
    public bool UsingUI;
    
    private Queue<Dialogue> dialogueQueue = new Queue<Dialogue>();
    
    private GameObject infoText;
    public string InfoStr {
        get {
            return infoText.GetComponent<Text>().text;
        }
        set {
            infoText.GetComponent<Text>().text = value;
        }
    }
    
    public void SetHealthStat(int healthStat) {
        HealthText.GetComponent<Text>().text = healthStat.ToString();
    }
    public void SetDefenseStat(int defenseStat) {
        DefenseText.GetComponent<Text>().text = defenseStat.ToString();
    }
    public void SetAttackStat(int attackStat) {
        AttackText.GetComponent<Text>().text = attackStat.ToString();
    }
    
    public GameObject CurrentTrader = null;
    
    [System.NonSerialized]
    public GameObject InventoryPanel;
    [System.NonSerialized]
    public GameObject ExtraInventoryPanel;
    [System.NonSerialized]
    public GameObject CraftingPanel;
    [System.NonSerialized]
    public GameObject CraftingPanelView;
    [System.NonSerialized]
    public GameObject TraderPanel;
    [System.NonSerialized]
    public GameObject TraderPanelView;
    //[System.NonSerialized]
    //public GameObject CraftingPanelScrollbar;
    [System.NonSerialized]
    public GameObject BuildingPanel;
    [System.NonSerialized]
    public GameObject BuildingPanelView;
    [System.NonSerialized]
    public GameObject PausePanel;
    [System.NonSerialized]
    public GameObject ArmorPanel;
    [System.NonSerialized]
    public GameObject TownPanel;
    [System.NonSerialized]
    public GameObject DialoguePanel;
    [System.NonSerialized]
    public GameObject DialogueFace;
    [System.NonSerialized]
    public GameObject DialogueText;
    [System.NonSerialized]
    public GameObject HealthText;
    [System.NonSerialized]
    public GameObject DefenseText;
    [System.NonSerialized]
    public GameObject AttackText;
    [System.NonSerialized]
    public GameObject HungerText;
    [System.NonSerialized]
    public GameObject ThirstText;
    [System.NonSerialized]
    public GameObject Player;
    [System.NonSerialized]
    public Inventory Inventory;
    [System.NonSerialized]
    public GameObject Town;
    [System.NonSerialized]
    public GameObject Cam;
    [System.NonSerialized]
    public CameraBehavior CamBehavior;
    
    public bool CursorFree {
        get {
            return Cursor.visible;
        }
        set {
            Cursor.lockState = value?CursorLockMode.None:CursorLockMode.Locked;
            Cursor.visible = value;
            gameObject.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController> ().m_MouseLook.lockCursor = !value;
        }
    }
    
    
    void Awake() {
        Player = GameObject.FindWithTag("MainPlayer");
        InventoryPanel = GameObject.FindWithTag("InventoryPanel");
        ExtraInventoryPanel = GameObject.FindWithTag("ExtraInventoryPanel");
        CraftingPanel = GameObject.FindWithTag("CraftingPanel");
        CraftingPanelView = GameObject.FindWithTag("CraftingPanelView");
        TraderPanel = GameObject.FindWithTag("TraderPanel");
        TraderPanelView = GameObject.FindWithTag("TraderPanelView");
        //CraftingPanelScrollbar = GameObject.FindWithTag("CraftingPanelScrollbar");
        BuildingPanel = GameObject.FindWithTag("BuildingPanel");
        BuildingPanelView = GameObject.FindWithTag("BuildingPanelView");
        PausePanel = GameObject.FindWithTag("PausePanel");
        ArmorPanel = GameObject.FindWithTag("ArmorPanel");
        TownPanel = GameObject.FindWithTag("TownPanel");
        DialoguePanel = GameObject.FindWithTag("DialoguePanel");
        DialogueFace = GameObject.FindWithTag("DialogueFace");
        DialogueText = GameObject.FindWithTag("DialogueText");
        HealthText = GameObject.FindWithTag("HealthText");
        DefenseText = GameObject.FindWithTag("DefenseText");
        AttackText = GameObject.FindWithTag("AttackText");
        HungerText = GameObject.FindWithTag("HungerText");
        ThirstText = GameObject.FindWithTag("ThirstText");
		infoText = GameObject.FindWithTag("InfoText");
        Town = GameObject.FindWithTag("Town");
        Inventory = Player.GetComponent<Inventory>();
        Cam = GameObject.FindWithTag("MainCamera");
        CamBehavior = Cam.GetComponent<CameraBehavior>();
    }
    
	void Start () {
        
        BuildingMenuOpen = false;
        PauseMenuOpen = false;
        TownMenuOpen = false;
        TraderMenuOpen = false;
        DialogueContinue(); //Unless we have dialogue at the start of the game, this just closes the pause menu
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    
    public void InitiateDialogue(string fileName) {
        string path = "Assets/Dialogue/"+fileName+".txt";
        StreamReader reader = new StreamReader(path);
        string currentLine;
        while(true){
            currentLine = reader.ReadLine();
            if(currentLine != null){
                int nameSeperator = currentLine.IndexOf(' ');
                string speakerName = currentLine.Substring(0,nameSeperator);
                string dialogueText = currentLine.Substring(nameSeperator+1);
                Dialogue nextDialogue = new Dialogue(speakerName, dialogueText);
                dialogueQueue.Enqueue(nextDialogue);
            }
            else {
                break;
            }
        }
        reader.Close();
        DialogueContinue();
    }
    
    public void DialogueContinue() { //called when continue button is pressed
        if (dialogueQueue.Count == 0) {
            DialoguePanelOpen = false;
        }
        else {
            DialoguePanelOpen = true;
            Dialogue nextDialogue = dialogueQueue.Dequeue();
            DialogueFace.GetComponent<Image>().sprite = nextDialogue.Face;
            DialogueText.GetComponent<Text>().text = nextDialogue.text;
        }
        
    }
    
    private bool dialoguePanelOpen = false;
    public bool DialoguePanelOpen {
        get { return dialoguePanelOpen;}
        set {
            CursorFree = value;
            DialoguePanel.SetActive(value);
            UsingUI = value;
        }
    }
    
    private bool inventoryMenuOpen = false;
    public bool InventoryMenuOpen {
        get {return inventoryMenuOpen;}
        set {
            CursorFree = value;
            CraftingPanelView.SetActive(value);
            ExtraInventoryPanel.SetActive(value);
            ArmorPanel.SetActive(value);
            if (value) {
                Inventory.UpdateCraftingRecipes();
                //CraftingPanelView.GetComponent<ScrollRect>().normalizedPosition = new Vector2(0, 1); //Should bring it to the top
            }
            inventoryMenuOpen = value;
            UsingUI = value;
        }
    }
    
    
    private bool buildingMenuOpen = false;
    public bool BuildingMenuOpen {
        get {
            return buildingMenuOpen;
        }
        set {
            CursorFree = value;
            BuildingPanelView.SetActive(value);
            if(value) {
                Inventory.UpdateBuildingRecipes();
            }
            buildingMenuOpen = value;
            UsingUI = value;
        }
    }
    
    private bool pauseMenuOpen = false;
    public bool PauseMenuOpen {
        get {
            return pauseMenuOpen;
        }
        set {
            CursorFree = value;
            PausePanel.SetActive(value);
            if(value) {
                //update the pause menu if necessary here
            }
            pauseMenuOpen = value;
            UsingUI = value;
        }
    }
    
    private bool townMenuOpen = false;
    public bool TownMenuOpen {
        get { return townMenuOpen;}
        set {
            CursorFree = value;
            TownPanel.SetActive(value);
            townMenuOpen = value;
            ExtraInventoryPanel.SetActive(value);
            UsingUI = value;
        }
    }
    
    private bool traderMenuOpen = false;
    public bool TraderMenuOpen {
        get { return traderMenuOpen;}
        set {
            if (value && CurrentTrader == null) {
                Debug.LogError("Attempted to open trade menu without CurrentTrader being assigned in HUD");
            }
            //TraderBehavior traderBehavior = CurrentTrader.GetComponent<TraderBehavior>();
            if(value) {
                CurrentTrader.GetComponent<TraderBehavior>().CreateButtons();
            }
            else if (CurrentTrader != null) {
                CurrentTrader.GetComponent<TraderBehavior>().DestroyButtons();
            }
            CursorFree = value;
            TraderPanelView.SetActive(value);
            traderMenuOpen = value;
            UsingUI = value;
            //Make sure tab closes it
        }
    }

}
public class Dialogue {
    
    private static Dictionary<string,Sprite> faces = new Dictionary<string,Sprite>();
    
    public string faceName;
    
    public Sprite Face {
        get {
            return faces[faceName];
        }
    }
    public string text;
    
    public Dialogue(string faceName, string text) {
        this.faceName = faceName;
        if (!faces.ContainsKey(faceName)) {
            faces[faceName] = GetSprite(faceName);
        }
        this.text = text;
    }
    
    
    
    
    
    
    
    
    Sprite GetSprite(string name) {
        Sprite t = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/Resources/DialogueFaces/"+name+".png", typeof(Sprite));
        if (t == null) {
            Debug.LogError("Could not find sprite for "+name);
            t = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/Resources/DialogueFaces/unknown.png", typeof(Sprite)); //Draw an unknown thing
        }
        return t;
    }
    
}