using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats instance;
    public int currentHealth;
    public int maxHealth;
    public int speed;
    public int attackPower;
    public string characterName;
    public int maxMove;
    public int maxAP;
    public int currentMove;
    public int currentAP;
    public int xPos;
    public int yPos;
    public int zPos;
    public int defense;
    public int levelsCompleted;
    public bool isGoblin = false;
    public bool isLoki = false;
    public bool inNifl = false;
    public bool inCave = false;
    public bool inValh = false;

    // list of items in inventory
    public List<GameObject> items;
    

    // demo positions
    public int
        demoXPos,
        demoYPos,
        demoZPos;

    //Party Serialized Field
    [SerializeField] GameObject PartyList;
    [SerializeField] UnitClass _Lief;
    [SerializeField] UnitClass _Thief;
    [SerializeField] UnitClass _Archer;
    [SerializeField] UnitClass _Healer;
    [SerializeField] UnitClass _Mage;
    [SerializeField] UnitClass _Warrior;

    //And Loki
    [SerializeField] UnitClass _Loki;


    //Party Getters
    public UnitClass Lief { get { return _Lief; } }
    public UnitClass Thief { get { return _Thief; } }
    public UnitClass Archer { get { return _Archer; } }
    public UnitClass Healer { get { return _Healer; } }
    public UnitClass Mage { get { return _Mage; } }
    public UnitClass Warrior { get { return _Warrior; } }
    public List<PartyMember> activeMembers;

    //... And Loki
    public UnitClass Loki { get { return _Loki; } }

    private void Start()
    {
        _Lief.SetupUnit();
        _Thief.SetupUnit();
        _Archer.SetupUnit();
        _Healer.SetupUnit();
        _Mage.SetupUnit();
        _Warrior.SetupUnit();
        items = new List<GameObject>();

        /*  The following code should be commented out while testing SOLELY in tacticalDemo,
         *  If you are testing Party transition, keep this line.
         */
        levelsCompleted = 0;
        activeMembers = new List<PartyMember>();
    }

    public void UpdateActiveParty()
    {
        if (PartyMembersInstance.instance != null)
        {
            PartyList = PartyMembersInstance.instance.getPartyList();
        }
        if (activeMembers.Count != 0)
            activeMembers.Clear();
        for(int i = 0; i < PartyList.transform.childCount; i++)
        {
            GameObject slot = PartyList.transform.GetChild(i).gameObject;
            if (slot.transform.childCount != 0)
                activeMembers.Add(slot.transform.GetChild(0).GetComponent<DragHandler>().myUnit);
        }
    }
    public void updatePlayerInfo()                          //Save player position prior to transition to tactics 
    {
       Transform playerTransform = GameObject.Find("Player").GetComponent<Transform>();

        xPos = (int)playerTransform.position.x;
        yPos = (int)playerTransform.position.y;
        zPos = (int)playerTransform.position.z;
    }

    public void startPlayerInfo()                          //Save player position prior to transition to tactics 
    {
        Transform playerTransform = GameObject.Find("Player").GetComponent<Transform>();

        xPos = (int)-15.62f;
        yPos = (int)-5.84f;
        zPos = (int)0f;
    }


    // Demo start positions
    public void DemoPlayerInfo()
    {
        demoXPos = (int)-329.5f;
        xPos = demoXPos;
        demoYPos = (int)73.5f;
        yPos = demoYPos;
        demoZPos = (int)0f;
        zPos = demoZPos;
    }

    public void retrievePlayerInfo()                        //Obtain and Set player position prior to transition to Platformers 
    {
        Transform playerTransform = GameObject.Find("Player").GetComponent<Transform>();
        PlayerCombatController Combat = GameObject.Find("Player").GetComponent<PlayerCombatController>();
        PlayerPosition PP = GameObject.Find("PlayerPosition").GetComponent<PlayerPosition>();

        Vector3 position;
        
        // only if they trigger the demoDone collider do they re-spawn in the base
        if(PP.demoDone){
            Debug.Log("in normal");
            startPlayerInfo();
            position.x = xPos;
            position.y = yPos;
            position.z = zPos;
        } else {
            Debug.Log("in demo");
            DemoPlayerInfo();
            position.x = demoXPos;
            position.y = demoYPos;
            position.z = demoZPos;
        }
        

        playerTransform.position = position;
    }

    public void LokiUpgrade()
    {
        int healthUp = 5;
        int damageUp = 5;
        int defenseUp = 5;
        Loki.Core.LokiUpgrade(healthUp, damageUp, defenseUp);
    }
    
    void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}

public enum PartyMember
{
    Lief,
    Thief,
    Archer,
    Healer,
    Mage,
    Warrior,
}
