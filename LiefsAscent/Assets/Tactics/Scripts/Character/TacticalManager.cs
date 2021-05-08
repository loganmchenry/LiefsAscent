/*  Documentation:
 *  Intended use
 *      Manage the Tactical UI
 *      Manage the Combat Phase States
 *          Perform related functions
 *  
 *  Last Time Documentation Updated: 5 May 2021
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Linq;


public enum ActState { SELECT, ATTACK, MOVE, ABILITY, ITEM, VICTORY, DEFEAT, WAIT, ENEMYTURN }
public class TacticalManager: MonoBehaviour
{
    public static ActState takeAct;
    [Header("Tilemaps used to draw the background and some UI")]
    [Tooltip("The layer that holds player data")]
    [SerializeField] Tilemap playerLayer;               //Possibly unnecessary?
    [Tooltip("The layer that draws the tiles that can be used")]
    [SerializeField] Tilemap targetLayer;
    [Tooltip("Tile...")]
    [SerializeField] Tile newTile;                      //Possibly unnecessary?
    /*                      UI Variables
     * ---------------------------------------------------------------
     * The following belong to the UI to display the character's stats           */
    [SerializeField] GameObject theCanvas;
    GameObject ActionList;  //The Action buttons    (Attack, Move, Ability, Item, End)
    GameObject HUDBoard;    //Primary hud           (Stats and Ability Info)
    GameObject AbilityUI;   //Child of HUD,         Ability Info - actual
    TMP_Text StatHeader1;   // HP - STR - DEF
    TMP_Text StatHeader2;   // SPD - INT - MYS
    TMP_Text StatHeader3;   // <empty> - AP - CLASS
    TMP_Text StatValue1;    // Curr/Max - Attack Power - Defense
    TMP_Text StatValue2;    // Speed - Intelligence - Mysticism
    TMP_Text StatValue3;    // <empty> - CurrAP/MaxAP - CLASSNAME
    GameObject TurnHeader;  //Your Turn Display
    GameObject WinMessage;  //Victory Display
    GameObject LoseMessage; //Defeat Display
    GameObject CancelAct;   //Right Click to Cancel message
    GameObject BossWinMessage; //Victory display for boss battles
    Color buttonColor;

    /*                      Manager Variables
     * -----------------------------------------------------------------
     * The following are used to control turn order,
     * or the various functions inputs                                      */
    [Tooltip("The unit who is currently taking action.")]
    public static GameObject selected;       //The unit OBJECT who is currently taking action
    private CharacterScript selectedChar;       //The current unit's data
    private CharacterScript unitTwo;            //The other unit needed to complete action (Used in: ABILITY)
    private Pathing Pathfinder;                    //Class handling finding path between two points
    //------- Turn Order Queue Related Variables
    private List<GameObject> turnOrder;         //The queue which holds who goes next
    int MAXQUEUE;
    int unitIndex;
    int myPartySize = 0;

    //------- MISC Variables-------------------------------------------
    private int skillIndex;                     //Which ability of the unit's to use.
    private bool isAllyOnTile;                  //Desc needed.
    private CharacterScript lief;               //char script var called lief which will be used for temporary AI for demo
    private bool rewardsGiven;                  //Keeping track of rewards for each encounters
    [SerializeField] TacticsInventorySystem tacticsInventory;
    int currDead;
    public static bool enemyMove = false;
    public static bool enemyAttack = false;
    public static bool isLokiFight = false;
    //Currently unused?
    //------------------------------------------
    private TileBase currCharTile;
    private Vector3Int currCharPos;
    SceneLoader loader;
    //------------------------------------------

    //Tilemap accessing Variables
    //--------------------------------
    private GameObject tilemap;
    private TileMapScript tileAccess;
    //--------------------------------

    /*                          SECTION: Unity Start and Update
     * -------------------------------------------------------------------------------------------------------
     * GENERAL DESCRIPTION:
     *  These functions are set by Unity because this is a MonoBehaviours script
     * 
     * NOTES:
     *  N/A
     *  
     * FUNCTIONS:
     * 1)   Start()                                     - This method is invoked by the Unity runtime at scene's initial load
     *      a)  See functions below for sub-functions   
     * 2)   Update()                                    - This method is invoked by the Unity runtime at every frame
     *      b)  See functions below for sub-functions
     * ------------------------------------------------------------------------------------------------------- */

    /* Function Name:   Start
     * Input:           None, automatically called at scene start
     * Output:          Setup all background work at start                   */
    void Start()
    {
        Pathfinder = GetComponent<Pathing>();
        SetVariables();              //Set initial statuses;
        CalculateTurnOrder();        //Calculate the turnOrder
        tilemap = GameObject.Find("proceduralMap");
        tileAccess = tilemap.GetComponent<TileMapScript>();

        //!!Riz: modify this AND enemyAI to use TileMapScript getters
        tileAccess.loadTileDatas();
        Pathfinder.mapData = tileAccess.tileInformation;
        Pathfinder.map = tileAccess.groundLayer;
        currDead = 0;
    }

    /* Function Name:   Update
     * Input:           None, called at each frame
     * Output:          See sections below for corresponding states         */
    void Update()
    {

        //checking tile positions, removable when done Riz
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Debug.Log($"Clicked TILE: {tileAccess.GetTilePosition(mousePosition)}");
        }

        switch (takeAct)
        {
            case ActState.SELECT:       StateSelect();      break;
            case ActState.ATTACK:       StateAttack();      break;
            case ActState.MOVE:         StateMove();        break;
            case ActState.ABILITY:      StateAbility();     break;
            case ActState.ITEM:         StateItem();        break;
            case ActState.VICTORY:      StateVictory();     break;
            case ActState.DEFEAT:       StateDefeat();      break;
            case ActState.ENEMYTURN:    StateEnemyTurn();   break;
            case ActState.WAIT:         default:            StateWait();    break;
        }
    }

    /*                          SECTION: ActState.SELECT Related Functions
     * -------------------------------------------------------------------------------------------------------
     * GENERAL DESCRIPTION:
     *  These functions are called while takeAct is set to ActState.SELECT
     * 
     * NOTES:
     *  The player should never be able to directly manipulate these functions apart from entering state
     *  
     * FUNCTIONS:
     * 1)   Action4End()                    - Button which leads to this state
     * 2)   StateSelect()                   - Main function of Select state
     *      a)  NextTurn()                  - Grab next living unit in turn order queue
     * ------------------------------------------------------------------------------------------------------- */

    /* Function Name:   Action #4 End
     * Input:           Click on the End Turn button in the UI
     * Output:          Hides the UI and sets selected character to null.   */
    public void Action4End()
    {
        buttonColor.a = 1f;
        ActionList.transform.GetChild(0).GetComponent<Image>().color = buttonColor;
        ActionList.transform.GetChild(1).GetComponent<Image>().color = buttonColor;
        ActionList.transform.GetChild(2).GetComponent<Image>().color = buttonColor;
        ActionList.transform.GetChild(3).GetComponent<Image>().color = buttonColor;
        ActionList.transform.GetChild(4).GetComponent<Image>().color = buttonColor;
        targetLayer.ClearAllTiles();
        theCanvas.SetActive(false);
        selectedChar.TurnEndResetValues();                                    //Reset unit's AP to full
        selectedChar = null;
        selected = null;
    }

    /* Function Name:   State Select
     * Input:           None, called when takeAct == ActState.SELECT by other functions
     * Output:          Perform related functions (Get next living unit in turn order queue)    */
    private void StateSelect()
    {
        if (selectedChar == null || selected == null)
        {
            NextTurn();
        }
        //A character was selected, display UI
        if (selected)
        {
            selectedChar = selected.GetComponent<CharacterScript>();        //Get the script that holds the variables
            ShowStats();                                                //Display the stats
            Camera.main.transform.position = new Vector3(selected.transform.position.x, selected.transform.position.y, Camera.main.transform.position.z);
            selectedChar.TurnStartResetValues();                                    //Reset unit's AP to full
            //NOTE: Change to check if selectedChar is enemey
            //if (selectedChar.UnitName == "Evil Wizard")
            if (!selectedChar.MyParty)
            {
                StartCoroutine(StateEnemyTurn()); //remove when pathing complete 
                takeAct = ActState.ENEMYTURN; //uncomment when pathing complete
            }
            else
                takeAct = ActState.WAIT;                                        //Wait for button to be pressed.
        }
    }

    /* Function Name:   Next Turn
     * Input:           Called in StateSelect() if no unit is currently selected
     * Output:          The current unit is set to the next living unit (Based on speed stat).  */
    public void NextTurn()
    {
        if (unitIndex == MAXQUEUE)
        {
            unitIndex = 0;                                                      //Return to the start of the queue
        }

        GameObject currUnit = turnOrder[unitIndex];                             //Grab current unit
        CharacterScript thisUnit = currUnit.GetComponent<CharacterScript>();    //Grab information
                                                                                 //Move on to next unit in queue
        //check if myPartySize is 0 (no allys left then go to defeat)

        if (thisUnit.Dead)                                                  //Check if unit is dead
        {
            while (thisUnit.CurrHP <= 0)                                       //While unit is dead
            {
                if (unitIndex == MAXQUEUE)
                    unitIndex = 0;
                currUnit = turnOrder[unitIndex++];                              //Go to next unit
                selected = currUnit;
                thisUnit = currUnit.GetComponent<CharacterScript>();
            }
        }
        else
        {
            unitIndex++;
            selected = currUnit;                                                //If not dead, current unit is now the selected unit
            tacticsInventory.unitSelected = selected;
            tacticsInventory.characterDisplay.transform.GetChild(0).GetComponent<Image>().sprite = selected.GetComponent<SpriteRenderer>().sprite;
        }
    }

    /*                          SECTION: ActState.ATTACK Related Functions
     * -------------------------------------------------------------------------------------------------------
     * GENERAL DESCRIPTION:
     *  These functions are called while takeAct is set to ActState.ATTACK
     * 
     * NOTES:
     *  This handles showing the UI for attack range.
     *  Actual attack function located in CharacterScript.attackCharacter(CharacterScript)
     *  
     * FUNCTIONS:
     *  1)  Action0Attack()                 - Button that leads to this state
     *      a)  DrawTargetting(int)         - See SECTION: UI
     *      b)  ButtonCheck()               - See SECTION: UI
     *  2)  StateAttack()                   - Lead function of Attack state
     *      a)  WaitForPlayerInput()        - See SECTION: Misc.
     * ------------------------------------------------------------------------------------------------------- */

    /* Function Name:   Action #0 Attack
     * Input:           Clicked on the attack button UI
     * Output:          Switch states to attack - 
     *                  Changes function of Update() to attack functions                        */
    public void Action0Attack()
    {
        takeAct = ActState.ATTACK;
        //Draw targetting (assuming melee range)  
        if (selectedChar.CurrAP > 0)
        {
            Camera.main.transform.position = new Vector3(selected.transform.position.x, selected.transform.position.y, Camera.main.transform.position.z);
            //Draw targetting (assuming melee range)
            DrawTargetting(selectedChar.AttackRange);
            ActionList.SetActive(false);                //Disable other buttons.
            CancelAct.SetActive(true);                  //Enable cancel action header
        }
        else
        {
            ButtonCheck();
            takeAct = ActState.WAIT;
        }
    }

    /* Function Name:   State Attack
     * Input:           (Internally of WaitForPlayerInput) Mouse Click
     *                      Left Mouse Click    - If on valid tile AND tile has opposing unit, attack it.
     *                      Right Mouse Click   - Return to ActState.WAIT
     * Output:          See lines above         */
    private void StateAttack()
    {
        if (selected.name == "EvilWizard0" || selected.name == "EvilWizard1" || selected.name == "EvilWizard2" || selected.name == "EvilWizard3" || selected.name == "LokiBoss" || selected.name == "Draugr0" ||
            selected.name == "Draugr1" || selected.name == "Draugr2" || selected.name == "Draugr3" || selected.name == "Goblin" || selected.name == "Fiel0" || selected.name == "Fiel1" || selected.name == "Fiel2" ||
            selected.name == "Fiel3" || selected.name == "Fiel4")
        {
            takeAct = ActState.WAIT;

            if (isAllyOnTile)
            {
                targetLayer.ClearAllTiles();
                selectedChar.AttackCharacter(lief);

                //Changing the lose condition
                /*
                if (lief.CurrHP <= 0)
                {
                    FindObjectOfType<PlayerLives>().LoseLife(1);
                    takeAct = ActState.DEFEAT;
                }
                else
                {
                */    
                    Action4End();
                //}
            }
            else
            {
                targetLayer.ClearAllTiles();
            }
        }
        else
            StartCoroutine(WaitForPlayerInput());
    }

    /*                          SECTION: ActState.MOVE Related Functions
     * -------------------------------------------------------------------------------------------------------
     * GENERAL DESCRIPTION:
     *  These functions are called while takeAct is set to ActState.MOVE
     * 
     * NOTES:
     *  This handles showing the UI for movement.
     *  Actual movement function located in CharacterScript.moveCharacter(int, int, Vector3Int) 
     *  
     * FUNCTIONS:
     *  1)  Action1Move()                   - Button that leads to this state
     *      a)  DrawTargetting(int)         - See General Functions Section
     *  2)  StateMove()                     - Lead function of Move state
     *      a)  WaitForPlayerInput()        - See General Functions Section
     * ------------------------------------------------------------------------------------------------------- */

    /* Function Name:   Action #1 Move
     * Input:           Clicked on the move button UI
     * Output:          Switch states to move - Changes function of Update() to only care about move related functionality
     */
    public void Action1Move()
    {
        takeAct = ActState.MOVE;
        if (selectedChar.CanMove)
        {
            Camera.main.transform.position = new Vector3(selected.transform.position.x, selected.transform.position.y, Camera.main.transform.position.z);
            //drawMovement();
            //DrawTargetting(selectedChar.Movement);
            //New Movement
            Vector3Int playerPosition = new Vector3Int(selectedChar.XPos, selectedChar.YPos, 0);
            tileAccess.drawMovement(selectedChar.Movement, playerPosition);
            ActionList.SetActive(false);                //Disable other buttons.
            CancelAct.SetActive(true);
        }
        else
            takeAct = ActState.WAIT;
    }

    /* Function Name:   State Move
     * Input:           (Internally of WaitForPlayerInput) Mouse Click
     *                      Left Mouse Click    - If on valid tile, perform unit's move function
     *                      Right Mouse Click   - Return to ActState.WAIT
     * Output:          See lines above         */
    private void StateMove()
    {
        StartCoroutine(WaitForPlayerInput());
    }

    /*                          SECTION: ActState.ABILITY Related Functions
     * -------------------------------------------------------------------------------------------------------
     * GENERAL DESCRIPTION:
     *  These functions are called while takeAct is set to ActState.ABILITY
     * 
     * NOTES:
     *  This handles UI for all ability functions such as ability range, buttons, and info
     *  See CharacterManagerScript.useAbility(int)
     *      For individual ability performance function
     *  See Skill and SkillCore scripts
     *      For individual ability details
     *  See "Assets/Tactics/Resources/Skills/"
     *      For all skills currently implemented
     *  See "Assets/Tactics/Resources/Classes/"
     *      For all classes and what skills they have
     *  
     * FUNCTIONS:
     *  1)  Action2Ability()                    - Button that leads to this state
     *  2)  StateAbility()                      - Lead function of Ability state
     *      a)  PerformAbility()                - Driving function of Ability State
     *      b)  AbilitySelect[0-3]()            - Specific ability to use.
     *          i)  AbilitySelectDisplay()      - Simple display for ability being used
     * ------------------------------------------------------------------------------------------------------- */

    /* Function Name:   Action #2 Ability
     * Input:           Clicked on the ability button UI
     * Output:          Switch states to ability - Changes function of Update() to only care about ability related functionality    */
    public void Action2Ability()
    {
        takeAct = ActState.ABILITY;
        if (selectedChar.CurrAP > 0 && selectedChar.TrueClass.Skills.Count != 0)
        {
            ActionList.SetActive(false);                                        //Disable other buttons.
            AbilityUI.SetActive(true);                                          //Enable abilityUI
            for(int i = 1; i < 7; i++)
                HUDBoard.transform.GetChild(i).gameObject.SetActive(false);     //Disable other members (Stat fields)
            CancelAct.SetActive(true);                                          //Enable cancel action header

            for (int i = 0; i < 4; i++)                                         //Reset ability buttons
                AbilityUI.transform.GetChild(i).gameObject.SetActive(false);    //Turn them all off
            
            for (int i = 0; i < selectedChar.TrueClass.Skills.Count; i++)                               //For each ability up to 4 (The limit of abilities a unit can hold)
            {
                AbilityUI.transform.GetChild(i).gameObject.SetActive(true);                             //Show the i'th ability
                AbilityUI.transform.GetChild(i).GetChild(0).GetComponent<TMP_Text>().text = selectedChar.TrueClass.Skills[i].Base.SkillName;    //To reflect the name of the skill.
            }
            //If we have abilities, enable the ability info UI
            if (selectedChar.TrueClass.Skills.Count > 0)
                AbilityUI.transform.GetChild(4).gameObject.SetActive(true);
            Camera.main.transform.position = new Vector3(selected.transform.position.x, selected.transform.position.y, Camera.main.transform.position.z);
        }
        else
        {
            takeAct = ActState.WAIT;
            ButtonCheck();
        }
    }

    /* Function Name:   State Ability
     * Input:           (Internally) Button Click or Right Mouse Click
     *                      UI Button   --> AbilitySelect[0-3] was clicked
     *                      RMB         --> Cancel, return to ActState.WAIT
     * Output:           See AbilitySelect[0-3] Function below*/
    private void StateAbility()
    {
        if (selected == null)
            takeAct = ActState.WAIT;
        else
        {
            if (skillIndex < 4)
            {
                StartCoroutine(PerformAbility());
                ButtonCheck();
            }
        }
        if (Input.GetMouseButtonDown(1))                                                //Right click to cancel
        {
            unitTwo = null;
            targetLayer.ClearAllTiles();
            ActionList.SetActive(true);                 //Re-enable buttons
            AbilityUI.SetActive(false);                 //Disable AbilityUI
            for(int i = 1; i < 7; i++)
                HUDBoard.transform.GetChild(i).gameObject.SetActive(true);     //Enable stat text
            takeAct = ActState.WAIT;
        }
    }

    /* Function Name:   Perform Ability
     * Input:           (Internally) Mouse Click
     *                      LMB     --> If on tile in range, and tile has unit, use ability
     *                      RMB     --> Cancel, return to ActState.WAIT
     * Output:          See related abilities*/
    IEnumerator PerformAbility()
    {
        if (unitTwo == null)        //If we don't have a target
        {
            CancelAct.SetActive(true);
            if (Input.GetMouseButtonDown(0))    //Wait for an input
            {
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);        //Get mouse location
                Vector3Int gridClickPosition = targetLayer.WorldToCell((Vector2)mousePosition);     //Check it's grid position on the target layer
                TileBase clickedTarget = targetLayer.GetTile(gridClickPosition);                    //Get the tile at that location.

                if (clickedTarget)                                                                  //If we got a valid tile (it is in range)
                {
                    RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);    //Get the location of the mouse as a 2D raycast
                    if (hit.collider != null)                                                       //If a box collider was there.
                    {
                        HUDBoard.SetActive(false);
                        CancelAct.SetActive(false);
                        GameObject affectedChar = hit.transform.gameObject;                         //Get the game object there.
                        unitTwo = affectedChar.GetComponent<CharacterScript>();                     //It should be a unit (We do not have anything else yet)
                        switch (selectedChar.TrueClass.Skills[skillIndex].Base.MyTarget)            //What kind of target does this skill affect? If it doesn't match, don't use.
                        {
                            case TargetTypes.Self:
                                if (unitTwo != selectedChar)
                                    unitTwo = null;
                                break;
                            case TargetTypes.AlliesALL:
                                if (unitTwo.MyParty != selectedChar.MyParty)
                                    unitTwo = null;
                                break;
                            case TargetTypes.AlliesOTHERS:
                                if (unitTwo.MyParty != selectedChar.MyParty || unitTwo == selectedChar)
                                    unitTwo = null;
                                break;
                            case TargetTypes.Enemies:
                                if (unitTwo.MyParty == selectedChar.MyParty)
                                    unitTwo = null;
                                break;
                            default:
                                unitTwo = null;
                                break;
                        }
                        AbilityUI.transform.GetChild(4).gameObject.GetComponent<TMP_Text>().text = "";
                        if (unitTwo != null)
                        {
                            selectedChar.UseAbility(unitTwo, skillIndex);                               //Use the ability
                            yield return new WaitForSeconds(2);
                            if (unitTwo.CurrHP <= 0)
                            {
                                if (unitTwo.MyParty)
                                    myPartySize--;
                                else
                                    currDead++;
                                unitTwo.SetPos(100, 100);
                                unitTwo.gameObject.transform.position = new Vector3(1000, 1000);
                            }
                            if (unitTwo.CurrHP <= 0 && unitTwo.gameObject.name == "LokiBoss")
                            {
                                takeAct = ActState.VICTORY; // show winning message
                                isLokiFight = false;
                            }
                            else if (currDead == TacticalProceduralGenScript.enemyCtr && PlayerStats.instance.inValh == false && isLokiFight == false)
                            {
                                //If the enemy has no more units alive
                                takeAct = ActState.VICTORY; // show winning message
                            }
                            else if (PlayerStats.instance.inValh == true && currDead == TacticalProceduralGenScript.enemyValhalla && isLokiFight == false)
                            {
                                //If the enemy has no more units alive
                                takeAct = ActState.VICTORY; // show winning message
                            }
                            else
                            {
                                ButtonCheck();
                                HUDBoard.SetActive(true);
                                takeAct = ActState.WAIT;
                                //Exit ABILITY actState.
                                unitTwo = null;
                                ActionList.SetActive(true);                 //Re-enable buttons
                                AbilityUI.SetActive(false);    //Disable ability buttons and text
                                for (int i = 1; i < 5; i++)
                                    theCanvas.transform.GetChild(1).GetChild(1).gameObject.SetActive(true);     //Enable header text
                            }
                        }
                    }
                    else
                    {
                        DrawTargetting(selectedChar.TrueClass.Skills[skillIndex].Base.Range);
                    }
                }
            }
            if (Input.GetMouseButtonDown(1))        //Right click to cancel
            {
                HUDBoard.SetActive(true);
                AbilityUI.SetActive(false);    //Disable ability buttons and text
                for (int i = 1; i < 5; i++)
                    theCanvas.transform.GetChild(1).GetChild(1).gameObject.SetActive(true);     //Enable header text

                AbilityUI.transform.GetChild(4).gameObject.GetComponent<TMP_Text>().text = "";
                skillIndex = 4;                     //Skill index is 4, which breaks the loop.
                unitTwo = null;
                ShowStats();
                yield return null;
            }
        }
        yield return null;

    }

    /* Functions:   AbilitySelect[0-3]
     *      NOTE
     *      The following four functions are just attached to buttons to set the correct index to use, they are otherwise identical
     * Input:       On the UI, an ability button is clicked
     * Output:      Change current ability to use*/
    public void AbilitySelect0()
    {
        targetLayer.ClearAllTiles();                                                //Clear the target layer
        skillIndex = 0;                                                             //Update skill being used

        AbilitySelectDisplay();
    }
    public void AbilitySelect1()
    {
        targetLayer.ClearAllTiles();                                                //Clear the target layer
        skillIndex = 1;                                                             //Update skill being used

        AbilitySelectDisplay();
    }
    public void AbilitySelect2()
    {
        targetLayer.ClearAllTiles();                                                //Clear the target layer
        skillIndex = 2;                                                             //Update skill being used

        AbilitySelectDisplay();
    }
    public void AbilitySelect3()
    {
        targetLayer.ClearAllTiles();                                                //Clear the target layer
        skillIndex = 3;                                                             //Update skill being used

        AbilitySelectDisplay();
    }

    /* Functions:   Ability Select Display
     * Input:       Called from AbilitySelect[0-3]
     * Output:      Change the UI from stats to ability related UI.
     *              Show ability's range            */
    private void AbilitySelectDisplay()
    {
        if (selectedChar.CurrAP >= selectedChar.TrueClass.Skills[skillIndex].COST)  //If they can use the ability
            DrawTargetting(selectedChar.TrueClass.Skills[skillIndex].Base.Range);   //Draw the range that it can affect

        TMP_Text abilityInfo = AbilityUI.transform.GetChild(4).gameObject.GetComponent<TMP_Text>();

        abilityInfo.text = selectedChar.TrueClass.Skills[skillIndex].Base.SkillName + "\n"
            + "AP Cost: " + selectedChar.TrueClass.Skills[skillIndex].COST + "\n"
            + selectedChar.TrueClass.Skills[skillIndex].Base.Description;
    }

    /*                          SECTION: ActState.ITEM Related Functions
     * -------------------------------------------------------------------------------------------------------
     * GENERAL DESCRIPTION:
     *  These functions are called while takeAct is set to ActState.ITEM
     * 
     * NOTES:
     *  This has not yet been implemented
     *  
     * FUNCTIONS:
     *  1)  Action3Item()                       - Button that leads to this state
     *  2)  State Item()                        - Primary function of state
     * ------------------------------------------------------------------------------------------------------- */

    /* Function Name:   Action #3 Item
     * Input:           Clicked on the item button UI
     * Output:          Switch states to item - Changes function of Update() to only care about item related functionality.
     *                  NOTE: Currently unimplemented, forced to actState.WAIT until implemented.           */
    public void Action3Item()
    {
        takeAct = ActState.ITEM;
        ButtonCheck();
    }

    /* Function Name:   State Item
     * Input:           NOT YET IMPLEMENTED
     * Output:          NOT YET IMPLEMENTED             */
    private void StateItem()
    {
        takeAct = ActState.WAIT;
        tacticsInventory.gameObject.SetActive(true);
    }

    /*                          SECTION: ActState.VICTORY Related Functions
     * -------------------------------------------------------------------------------------------------------
     * GENERAL DESCRIPTION:
     *  These functions are called while takeAct is set to ActState.VICTORY
     * 
     * NOTES:
     *  Unlike previous states which are reachable by a button, this state is called only once
     *  This state can only be reached when the enemy (computer) has no living units left.
     *  Not fully completed
     *      - Rewards (Boons)
     *      - Reward functions/display/etc.
     *  
     * FUNCTIONS:
     *  1)  StateVictory()                      - Primary function of state
     *  2)  BattleEndButton()                   - Exit Tactical Combat
     * ------------------------------------------------------------------------------------------------------- */

    /* Function Name:   State Victory
     * Input:           None, state reached when AI has no living units
     * Output:          Display victory screen                  */
    private void StateVictory()
    {
        ActionList.SetActive(false);    //Disable buttons
        HUDBoard.SetActive(false);      //Disable HUD
        TurnHeader.SetActive(false);    //Disable turn header
        GiveRewards();                  //Give rewards at the end of battle
    }
    
    /* Function Name:   Battle End Button
     * Input:           (Internally) Button click
     * Output:          Return to platformer section            */
    public void BattleEndButton()
    {
        //SceneManager.LoadScene("Base");
        //loader = FindObjectOfType<SceneLoader>();   //Reload Platformer Scene
        SceneLoader.instance.UnloadTacticalScene();
    }


    /* Function Name:   Give Rewards
     * Input:           None, state reached when AI has no living units
     * Output:          None, key object added to inventory, or flag set if boss battle          */
    public void GiveRewards()
    {
        if (RewardsManager.enterBossFight)
        {
            BossWinMessage.SetActive(true);
            RewardsManager.bossFinished = true;
            Debug.Log("won a boss fight!");
            RewardsManager.enterBossFight = false;
            rewardsGiven = true;
        } else if (!rewardsGiven)
        {
            WinMessage.SetActive(true);     //Enable victory message
            RewardsManager.keysCollected++;
            RewardsManager.setVictoryToTrue();
            rewardsGiven = true;
        }
    }

    /*                          SECTION: ActState.DEFEAT Related Functions
     * -------------------------------------------------------------------------------------------------------
     * GENERAL DESCRIPTION:
     *  These functions are called while takeAct is set to ActState.DEFEAT
     * 
     * NOTES:
     *  Not fully completed
     *      - Lose life? End run?
     *  
     * FUNCTIONS:
     *  1)  StateDefeat()                       - Primary function of state
     * ------------------------------------------------------------------------------------------------------- */
   
    /* Function Name:   State Defeat
     * Input:           None, state reached when Player has no living units
     * Output:          Display defeat screen                  */
    private void StateDefeat()
    {
        theCanvas.SetActive(true);
        ActionList.SetActive(false);    //Disable buttons
        HUDBoard.SetActive(false);      //Disable HUD
        TurnHeader.SetActive(false);    //Disable turn header
        LoseMessage.SetActive(true);     //Enable victory message
    }

    /*                          SECTION: ActState.WAIT Related Functions
     * -------------------------------------------------------------------------------------------------------
     * GENERAL DESCRIPTION:
     *  These functions are called while takeAct is set to ActState.WAIT
     * 
     * NOTES:
     *  Needs cleanup? Acts as a pseudo spinlock
     *  
     * FUNCTIONS:
     *  1)  StateWait()                       - Primary function of state
     * ------------------------------------------------------------------------------------------------------- */

    /* Function Name:   State Wait
     * Input:           None, called in Update() when takeAct is set to ActState.WAIT
     * Output:          Enable and update UI, set to SELECT state when no other action possible           */
    private void StateWait()
    {
        
        ActionList.SetActive(true);                //Enable other buttons.
        AbilityUI.SetActive(false);    //Enable header text
        for (int i = 1; i < 7; i++)
            HUDBoard.transform.GetChild(i).gameObject.SetActive(true);
        CancelAct.SetActive(false);               //Disable cancel action header

        if (selected)
        {
            ShowStats();                                                           //Update stat fields
            if (0 == selectedChar.CurrAP && selectedChar.CanMove)
                Action4End();                                                      //Auto end turn?
        }
        else
        {
            takeAct = ActState.SELECT;
        }
    }

    /*                          SECTION: General UI Functions
     * -------------------------------------------------------------------------------------------------------
     * GENERAL DESCRIPTION:
     *  These functions are used to control displaying the UI sections
     * 
     * NOTES:
     *  WIP: Constantly being updated as things are added,
     *  
     * FUNCTIONS:
     *  1)  ButtonCheck()                       - Visual feedback of unavailable actions
     *  2)  DrawTargetting(int)                 - Valid range of actions
     *  3)  ShowStats()                         - Current Unit stats
     * ------------------------------------------------------------------------------------------------------- */

    /* Function Name:   Button Check
     * Input:           None, called automatically at state ends.
     * Output:          Provide visual feedback to what buttons are available.          */
    private void ButtonCheck()
    {
        /*
         *  Attack and Abilities both use AP (Action Points)
         *  The unit cannot attack/use abilities if they have no AP, so display that by altering the button.
         */
        if (selectedChar.CurrAP == 0)  //AP check
        {
            buttonColor.a = 0.5f;
            ActionList.transform.GetChild(0).GetComponent<Image>().color = buttonColor;
            ActionList.transform.GetChild(2).GetComponent<Image>().color = buttonColor;
        }
        else
        {
            buttonColor.a = 1f;
            ActionList.transform.GetChild(0).GetComponent<Image>().color = buttonColor;
            ActionList.transform.GetChild(2).GetComponent<Image>().color = buttonColor;
        }
        /*
         *  A unit can only move once, see if it has moved this turn
         *  If it has, disable Move, else, enable it.
         */
        if (!selectedChar.CanMove)
        {
            buttonColor.a = 0.5f;
            ActionList.transform.GetChild(1).GetComponent<Image>().color = buttonColor;
        }
        else
        {
            buttonColor.a = 1f;
            ActionList.transform.GetChild(1).GetComponent<Image>().color = buttonColor;
        }
    }

    /* Function Name:   Draw Targetting
     * Input:           Integer range of tiles
     * Output:          Display a Square-Grid-Based range of input radius.          */
    private void DrawTargetting(int maxRange)
    {
        Color color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        int currCharMaxRange = maxRange + 1;
        Vector3Int currSpot = new Vector3Int();
        Vector3Int targetSpot = new Vector3Int();


        //Calculate all moveable tiles on the y axis
        int x = 0;
        int y = 0;
        //Set the current drawing point (currSpot)
        currSpot.x = selectedChar.XPos;
        currSpot.y = selectedChar.YPos;
        Debug.Log(selectedChar.XPos);
        Debug.Log(selectedChar.YPos);
        while (y < currCharMaxRange)
        {
            //Set the y of the spot to draw (below char)
            targetSpot.y = currSpot.y - y;
            x = 0;
            //Draw all tiles on the x-axis at a given y point
            while (x < currCharMaxRange - y)
            {
                targetSpot.x = currSpot.x - x;
                if (targetLayer.GetTile(targetSpot) == null && targetSpot.x < 8 && targetSpot.x > -9 && targetSpot.y < 8 && targetSpot.y > -9)
                {
                    targetLayer.SetTile(targetSpot, newTile);
                    targetLayer.SetTileFlags(targetSpot, TileFlags.None);
                    targetLayer.SetColor(targetSpot, color);
                }
                
                targetSpot.x = currSpot.x + x;
                if (targetLayer.GetTile(targetSpot) == null && targetSpot.x < 8 && targetSpot.x > -9 && targetSpot.y < 8 && targetSpot.y > -9)
                {
                    targetLayer.SetTile(targetSpot, newTile);
                    targetLayer.SetTileFlags(targetSpot, TileFlags.None);
                    targetLayer.SetColor(targetSpot, color);
                }
                x++;
               
            }

            x = 0;
            //Set the y spot to draw (above char)
            targetSpot.y = currSpot.y + y;
            //Draw all tiles on the x-axis at the given y point
            while (x < currCharMaxRange - y)
            {
                targetSpot.x = currSpot.x - x;
                if (targetLayer.GetTile(targetSpot) == null && targetSpot.x < 8 && targetSpot.x > -9 && targetSpot.y < 8 && targetSpot.y > -9)
                {
                    targetLayer.SetTile(targetSpot, newTile);
                    targetLayer.SetTileFlags(targetSpot, TileFlags.None);
                    targetLayer.SetColor(targetSpot, color);
                }
                
                targetSpot.x = currSpot.x + x;
                if (targetLayer.GetTile(targetSpot) == null && targetSpot.x < 8 && targetSpot.x > -9 && targetSpot.y < 8 && targetSpot.y > -9)
                {
                    targetLayer.SetTile(targetSpot, newTile);
                    targetLayer.SetTileFlags(targetSpot, TileFlags.None);
                    targetLayer.SetColor(targetSpot, color);
                }
                x++;
                
            }

            y++;
        }
        //Set tile at currSpot to null
        if (takeAct != ActState.ABILITY)         //If this is not an ability, don't pick self
            targetLayer.SetTile(currSpot, null);
        if (takeAct == ActState.ABILITY)         //If it is an ability
        {
            //If we CANNOT target ourself, do not pick the tile we are on.
            if (selectedChar.TrueClass.Skills[skillIndex].Base.MyTarget != TargetTypes.AlliesALL && selectedChar.TrueClass.Skills[skillIndex].Base.MyTarget != TargetTypes.Self)
                targetLayer.SetTile(currSpot, null);
        }
    }

    /* Function Name:   Show Stats
     * Input:           None - is called in StateWait()
     * Output:          Activate the UI and display corresponding stats for unit            */
    private void ShowStats()
    {
        theCanvas.SetActive(true);

        StatHeader1.text = "HP:" + '\n' + "STR:" + '\n' + "DEF:";
        StatValue1.text = selectedChar.CurrHP.ToString() + " / " + selectedChar.MaxHP.ToString() + "\n"
            + selectedChar.Strength.ToString() + "\n"
            + selectedChar.Defense.ToString();

        StatHeader2.text = "SPD:" + "\n" + "INT:" + "\n" + "MYS:";
        StatValue2.text = selectedChar.Speed.ToString() + "\n"
            + selectedChar.Intelligence.ToString() + "\n"
            + selectedChar.Mysticism.ToString();

        StatHeader3.text = "\n" + "AP" + "\n" + "CLASS:";
        StatValue3.text =  "\n" + selectedChar.CurrAP.ToString() + " / " + selectedChar.MaxAP.ToString() + "\n"
            + selectedChar.TrueClass.Core.ClassName;

        //If they are part of your party...
        if (selectedChar.MyParty)
        {
            //Show UI that says "Your Turn"
            TurnHeader.SetActive(true);
        }
        else
        {
            //Otherwise, do not.
            TurnHeader.SetActive(false);
        }
    }

    /* Function Name:   Show Stats
     * Input:           None - is called in StateWait()
     * Output:          Activate the UI and display corresponding stats for unit            */
    private void HideUI()
    {
        theCanvas.SetActive(false);
    }

    /*                          SECTION: Misc. Functions
     * -------------------------------------------------------------------------------------------------------
     * GENERAL DESCRIPTION:
     *  These functions are used outside of main states, 
     *  or have multiple states depending on ActState of takeAct
     * 
     * NOTES:
     *  WIP: Constantly being updated as things are added
     *  
     * FUNCTIONS:
     *  1)  CalculateTurnOrder()        - Determine Turn Order Queue based on speed
     *  2)  DoEnemyTurn()               - MOVE TO "EnemyAI" script in "Assets/Tactical/Scripts/Enemy/"
     *  3)  SetVariables()              - Set background variables
     *  3)  WaitForPlayerInput()        - See function comments
     * ------------------------------------------------------------------------------------------------------- */

    /* Function Name:   Calculate Turn Order
     * Input:           None, called in Start()
     * Output:          set turnOrder[] and unitIndex            */
    public void CalculateTurnOrder()
    {
        turnOrder = new List<GameObject>();
        GameObject[] allUnits = GameObject.FindGameObjectsWithTag("Unit");
        MAXQUEUE = allUnits.Length;                                         //Total size of turn order queue, based on how many units are active at the start
        unitIndex = 0;                                                      //Keeps track of current unit's position in turn ordder queue
        foreach (GameObject unitTurn in allUnits)
        {
            CharacterScript currUnit = unitTurn.GetComponent<CharacterScript>();                    //Grab characterscript information
            turnOrder = allUnits.OrderByDescending(x => x.GetComponent<CharacterScript>().Speed).ToList();    //adds and sorts the allUnits list into turnOrder based on speed from high to low
            //This unit is in our order add 1 to the party counter
            if (currUnit.MyParty)
            {
                myPartySize++;
            }
        }

        selected = turnOrder[0];
        tacticsInventory.unitSelected = selected;
        tacticsInventory.characterDisplay.transform.GetChild(0).GetComponent<Image>().sprite = selected.GetComponent<SpriteRenderer>().sprite;
        isAllyOnTile = false;
        //lief = GameObject.Find("PlayerUnit").transform.GetComponent<CharacterScript>(); //grabbing lief at the start of the scene for demo
    }

    /* Function Name:   Do Enemy Turn
 * Input:           None, is AI
 * Output:          does exactly as function name is
 * 
 * NOTE:
 *      MOVE TO "EnemyAI" script in "Assets/Tactical/Scripts/Enemy/"            */
    public IEnumerator StateEnemyTurn()
    {
        CharacterScript enemyUnit = selectedChar;
        HideUI();
        //Get a list of allied characterscripts
        IEnumerable<GameObject> alliedObjects = turnOrder.Where(character => character.GetComponent<CharacterScript>().MyParty == true);
        List<CharacterScript> alliedCharacterScripts = new List<CharacterScript>();
        foreach(GameObject ally in alliedObjects)
        {
            if (ally.gameObject.GetComponent<CharacterScript>().CurrHP > 0)
            {
                alliedCharacterScripts.Add(ally.GetComponent<CharacterScript>());
            }
        }

        enemyMove = true;
        //Calculate enemy movement, the enemy will move to the closest ally unit using A*
        Vector3Int enemyPos = new Vector3Int(enemyUnit.XPos, enemyUnit.YPos, 0);
        List<Vector3Int> pathInt = new List<Vector3Int>();
        foreach (CharacterScript ally in alliedCharacterScripts)
        {
            Vector3Int allyPos = new Vector3Int(ally.XPos, ally.YPos, 0);


            List<Vector3Int> testPath = new List<Vector3Int>();
            testPath = Pathfinder.findPath(enemyPos, allyPos);
            //if the tested path is shorter than the saved one or if no path has been saved, keep it
            if (pathInt.Count == 0 || testPath.Count < pathInt.Count)
                pathInt = testPath;

        }


        //checking the movement path, making sure there is no unit at the end of the line.
        //If there is remove that node. Continue until the ending movement node is open.
        bool freeSpace = true;
        for (int i = pathInt.Count - 1; i >= 0; i--)
        {
            for (int j = 0; j < turnOrder.Count(); j++)
            {
                freeSpace = true;
                CharacterScript currChar = turnOrder[j].GetComponent<CharacterScript>();
                Vector3Int currCharPos = new Vector3Int(currChar.XPos, currChar.YPos, 0);
                if (currCharPos == pathInt[i])
                {
                    pathInt.RemoveAt(i);
                    freeSpace = false;
                    i--;
                    break;
                }
            }
            if (!freeSpace)
                continue;
            else
                break;
        }

        List<Vector3> path = new List<Vector3>();
        
        foreach(Vector3Int point in pathInt)
        {
            path.Add(targetLayer.CellToWorld(point));
        }

        yield return enemyUnit.NewMoveCharacter(path, enemyUnit.Movement);
        //move enemy based on path and movement speed
        enemyMove = false;
        new WaitForSeconds(2);

        /*   Checking if ally unit is next to enemy
         *   if ally is in range then proceed to attack  */
        foreach (CharacterScript ally in alliedCharacterScripts)
        {
            Vector3Int allyPos = new Vector3Int(ally.XPos, ally.YPos, 0);
            if (ally.XPos == enemyUnit.XPos - 1 && ally.YPos == enemyUnit.YPos 
                || ally.XPos == enemyUnit.XPos + 1 && ally.YPos == enemyUnit.YPos 
                || ally.YPos == enemyUnit.YPos - 1 && ally.XPos == enemyUnit.XPos 
                || ally.YPos == enemyUnit.YPos + 1 && ally.XPos == enemyUnit.XPos)
            {
                enemyAttack = true;
                enemyUnit.AttackCharacter(ally);
                new WaitForSeconds(2);
                if (ally.CurrHP <= 0)
                {
                    ally.SetPos(100, 100);
                    ally.gameObject.transform.position = new Vector3(1000, 1000);
                    myPartySize--;
                }
                break;
            }
            enemyAttack = false;

            new WaitForSeconds(2);
        }

        //return to normal state
        //takeAct = ActState.WAIT;
        yield return new WaitForSeconds(1);
        Action4End();
        takeAct = ActState.SELECT;
        if (myPartySize <= 0)
        {
            takeAct = ActState.DEFEAT;
        }
    }

    /* Function Name:   Set Variables
     * Input:           None, called at Start()
     * Output:          Set variables to be used elsewhere in the code.              */
    private void SetVariables()
    {
        ActionList  = theCanvas.transform.GetChild(0).gameObject;
        HUDBoard    = theCanvas.transform.GetChild(1).gameObject;
        AbilityUI   = HUDBoard.transform.GetChild(0).gameObject;
        StatHeader1 = HUDBoard.transform.GetChild(1).GetComponent<TMP_Text>();
        StatHeader2 = HUDBoard.transform.GetChild(2).GetComponent<TMP_Text>();
        StatHeader3 = HUDBoard.transform.GetChild(3).GetComponent<TMP_Text>();
        StatValue1  = HUDBoard.transform.GetChild(4).GetComponent<TMP_Text>();
        StatValue2  = HUDBoard.transform.GetChild(5).GetComponent<TMP_Text>();
        StatValue3  = HUDBoard.transform.GetChild(6).GetComponent<TMP_Text>();
        TurnHeader  = theCanvas.transform.GetChild(2).gameObject;
        WinMessage  = theCanvas.transform.GetChild(3).gameObject;
        LoseMessage = theCanvas.transform.GetChild(4).gameObject;
        CancelAct   = theCanvas.transform.GetChild(5).gameObject;
        BossWinMessage = theCanvas.transform.GetChild(6).gameObject;

        buttonColor = ActionList.transform.GetChild(0).GetComponent<Image>().color;
        CancelAct.SetActive(false);    //Disable cancel action header

        takeAct = ActState.SELECT;
        skillIndex = 4;

        rewardsGiven = false;
    }

    /* Function Name:   Wait For Player Input
     * Input:           (Internally) Mouse Click
     *                      LMB
     *                      RMB
     * 
     * Output:              LMB     --> ActState.ATTACK
     *                                      If on valid tile and tile has unit on opposing team, attack it.
     *                                  ActState.MOVE
     *                                      If on valid tile, use CharacterScript.moveCharacter(int, int, Vector3Int)
     *                      RMB     --> Cancel action, return to ActState.WAIT      */
    IEnumerator WaitForPlayerInput()
    {
        while (true)
        {
            if(Input.GetMouseButtonDown(0))
            {
                bool goodClick = true;
                //Get the mouse click spot and set the z to 0
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                //convert mouse position to grid position
                Vector3Int gridClickPosition = targetLayer.WorldToCell((Vector2)mousePosition);
                mousePosition = targetLayer.CellToWorld(gridClickPosition);
                mousePosition.z = 0;
                //Setup x movement by making sure the y is the same as our unit's
                Vector3 xPos = mousePosition;
                if (gridClickPosition == null)
                {
                    takeAct = ActState.WAIT;
                    targetLayer.ClearAllTiles();
                    yield return new WaitForSeconds(2);
                }
                xPos.y = selected.transform.position.y;
                xPos.x += 2;
                Vector3 yPos = mousePosition;

                //check if there is a tile where they clicked
                TileBase clickedTarget = targetLayer.GetTile(gridClickPosition);
                if (clickedTarget)
                {

                    //Clear the targetting then move the character by x-axis then y-axis
                    targetLayer.ClearAllTiles();
                    //Do an action depending on what state they are in
                    switch (takeAct)
                    {
                        case ActState.ATTACK:
                            {
                                //Check if there is a character on the targetTile
                                HUDBoard.SetActive(false);
                                CancelAct.SetActive(false);
                                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                                CharacterScript attackedChar = selectedChar.GetComponent<CharacterScript>();
                                //We selected a tile with a character on it
                                for (int i = 0; i < turnOrder.Count(); i++)
                                {
                                    CharacterScript currChar = turnOrder[i].GetComponent<CharacterScript>();
                                    //There is a character at this position
                                    if (currChar.XPos == gridClickPosition.x && currChar.YPos == gridClickPosition.y)
                                    {
                                        goodClick = false;
                                        attackedChar = turnOrder[i].GetComponent<CharacterScript>();
                                    }
                                }
                                if (goodClick == false)
                                {
                                    //GameObject attackedChar = hit.transform.gameObject;
                                    
                                    selectedChar.AttackCharacter(attackedChar);
                                    if(attackedChar.CurrHP <= 0)
                                    {
                                        if (attackedChar.MyParty)
                                            myPartySize--;
                                        else
                                            currDead++;
                                        attackedChar.SetPos(100, 100);
                                        attackedChar.gameObject.transform.position = new Vector3(1000, 1000);
                                    }
                                    if (attackedChar.CurrHP <= 0 && attackedChar.gameObject.name == "LokiBoss")
                                    {
                                        takeAct = ActState.VICTORY; // show winning message
                                        isLokiFight = false;
                                        yield break;
                                    }
                                    else if (currDead == TacticalProceduralGenScript.enemyCtr && PlayerStats.instance.inValh == false && isLokiFight == false)
                                    {
                                        //If the enemy has no more units alive
                                        takeAct = ActState.VICTORY; // show winning message
                                        yield break;
                                    }
                                    else if (PlayerStats.instance.inValh == true && currDead == TacticalProceduralGenScript.enemyValhalla && isLokiFight == false)
                                    {
                                        //If the enemy has no more units alive
                                        takeAct = ActState.VICTORY; // show winning message
                                        yield break;
                                    }
                                }
                                HUDBoard.SetActive(true);
                                takeAct = ActState.WAIT;
                            }
                            break;
                        case ActState.MOVE:
                            {
                                HUDBoard.SetActive(false);
                                CancelAct.SetActive(false);
                                //Check if there is a unit where they chose to move
                                //Iterate through the turn order array and check if the mouse spot is the same as the gridspot, if not then do the movement

                                for (int i = 0; i < turnOrder.Count(); i++)
                                {
                                    CharacterScript currChar = turnOrder[i].GetComponent<CharacterScript>();
                                    //There is a character at this position
                                    if (currChar.XPos == gridClickPosition.x && currChar.YPos == gridClickPosition.y)
                                    {
                                        goodClick = false;
                                    }
                                }
                                //If no unit move 
                                //Mark that the char has moved
                                if (goodClick)
                                {
                                    selectedChar.HasMoved();
                                    ButtonCheck();
                                    Vector3Int currentPosition = new Vector3Int(selectedChar.XPos, selectedChar.YPos, 0);

                                    List<Vector3Int> movingPathInt = new List<Vector3Int>();
                                    //yield return Pathing.Run<List<Vector3Int>>(Pathfinder.findPath(currentPosition, gridClickPosition, 2.3), (output) => movingPathInt = (List<Vector3Int>) output);
                                    movingPathInt = Pathfinder.findPath(currentPosition, gridClickPosition);
                                    List<Vector3> movingPath = new List<Vector3>();
                                    foreach (Vector3Int point in movingPathInt)
                                    {
                                        movingPath.Add(targetLayer.CellToWorld(point));
                                    }
                                    //!!! Remove
                                    Debug.Log($"Calc end: {movingPath[movingPath.Count - 1]}");
                                    Debug.Log($"Clicked tile: {gridClickPosition}");

                                    //takeAct = ActState.WAIT;
                                    //break;
                                    //checking the movement path, making sure there is no unit at the end of the line.
                                    //If there is remove that node. Continue until the ending movement node is open.
                                    bool freeSpace = false;
                                    for(int i = movingPath.Count - 1; i >= 0; i--)
                                    {
                                        for (int j = 0; j < turnOrder.Count(); j++)
                                        {
                                            CharacterScript currChar = turnOrder[j].GetComponent<CharacterScript>();
                                            Vector3Int currCharPos = new Vector3Int(currChar.XPos, currChar.YPos, 0);
                                            if (currCharPos == movingPath[i])
                                            {
                                                movingPath.RemoveAt(i);
                                                break;
                                            }
                                            else
                                            {
                                                freeSpace = true;
                                                break;
                                            }
                                        }
                                        if (!freeSpace)
                                            continue;
                                        else
                                            break;
                                    }
                                    yield return StartCoroutine(selectedChar.NewMoveCharacter(movingPath));
                                    //yield return StartCoroutine(selectedChar.MoveCharacter(xPos, yPos, gridClickPosition));
                                    HUDBoard.SetActive(true);
                                    takeAct = ActState.WAIT;
                                }
                            }
                            break;
                    }
                    yield break;
                }

            }
            if (Input.GetMouseButtonDown(1))
            {
                takeAct = ActState.WAIT;
                targetLayer.ClearAllTiles();
                yield return new WaitForSeconds(2);
            }
            yield return null;
        }
    }
}