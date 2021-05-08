/*  Documentation:
 *  Intended use
 *      Manage individual Unit stats
 *  
 *  Last Time Documentation Updated: 2 April 2021
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using TMPro;

public class CharacterScript : MonoBehaviour
{
    [SerializeField] string unitName;               //Used to differentiate names of units (May or may not do anything)

    /*  UNIT_VARIABLES: BOOLEANS
     *  ------------------------------
     *  1) My Party
     *  2) Can Move
     *  3) Dead                                 */
    [SerializeField] bool myParty;                  //Set to true if they are part of the "Allies," and false if they are enemies
    private bool canMove = true;                    //Default true, set to false when moved and set back to true on the start of a turn
    private bool dead = false;                      //Wouldn't it be funny if they were dead right away.

    [SerializeField] HealthBar healthBar;           //Visualization of HP   - Do we have one to show missing HP?
    [SerializeField] int currAP;                    //Whenever an action is done subtract to this, set equal to maxAP on the start of a turn
    [SerializeField] int xPos;                      //Holds the x-coordinate of this unit
    [SerializeField] int yPos;                      //Holds the y-coordinate of this unit

    /*  UNIT_VARIABLES: Misc.
     *  ------------------------------
     *  1) Game Stats
     *  2) My Class Core
     *  3) True Class
     *  4) Damage Text Prefab
     *  ------------------------------*/
    [SerializeField] GameObject gameStats;          //Will be used to update Liefs global stats for all scenes
    [SerializeField] UnitClassCore myClassCore;     //Used during initializations
    [SerializeField] UnitClass trueClass;           //The actual class derived from myClassCore
    [SerializeField] GameObject damageTextPrefab;   //Display DMG numbers

    /* Generic Get Functions for:
     * --------------------------------------------------
     * Description: 
     *      All of these will ONLY retrieve their
     *      corresponding data.
     * --------------------------------------------------
     *      ***** Status *****
     * 1)   UnitName
     * 2)   MyParty
     *      ***** Location & Movement *****
     * 3)   Movement
     * 4)   CanMove
     * 5)   XPos
     * 6)   YPos
     *      ***** Health *****
     * 7)   CurrHP
     * 8)   MaxHP
     * 9)   Dead
     *      ***** Action Points *****
     * 10)  CurrAP
     * 11)  MaxAP
     *      ***** Battle Stats *****
     * 12)  Strength
     * 13)  Defense
     * 14)  Speed
     * 15)  TrueClass
     * -------------------------------------------------- */

    //      STATUS
    public UnitClass TrueClass
    {
        get { return trueClass; }
        set { trueClass = value; 
            TurnEndResetValues();
            TurnStartResetValues();
        }
    }
    public string UnitName { get{ return TrueClass.Core.ClassName; } }
    public bool MyParty { get { return myParty; } }

    //      LOCATION AND MOVEMENT
    public int TempMove { get; set; }
    public int Movement
    {
        get
        {
            int tempTotal = TrueClass.MOVE;
            tempTotal += TempMove;
            if (tempTotal < 0)
                tempTotal = 0;
            return tempTotal;
        }
    }
    public bool CanMove { get { return canMove; } }
    public int XPos { get { return xPos; } }
    public int YPos { get { return yPos; } }
    
    //      HEALTH
    public int CurrHP 
    { 
        get { return TrueClass.CurrHP; }
        set { TrueClass.CurrHP = value; }
    }
    public int MaxHP { get { return TrueClass.MaxHP; } }
    public bool Dead { get { return dead; } }
    //      ACTION POINTS
    public int MaxAP { get { return TrueClass.AP; } }
    public int CurrAP { get { return currAP; } }

    //      BATTLE STATS
    public int AttackRange { get { return TrueClass.ATKRange; } }
    public int Strength { get { return TrueClass.STR+TempSTR; } }
    public int TempSTR { get; set; }
    public int Defense { get { return TrueClass.DEF+TempDEF; } }
    public int TempDEF { get; set; }
    public int Intelligence { get { return TrueClass.INT+TempINT; } }
    public int TempINT { get; set; }
    public int Mysticism { get { return TrueClass.MYS+TempMYS; } }
    public int TempMYS { get; set; }
    public int Speed { get { return TrueClass.Core.MyTurn; } }

    /* Set Functions
     * --------------------------------------------------
     *      ***** Status *****
     * 1)   ResetValues()
     *      ***** Location & Movement *****
     * 2)   HasMoved()
     * 3)   MoveCharacter(Vector3, Vector3, Vector3Int)
     * 4)   SetPos(int, int)
     *      ***** Health *****
     * 5)   UpdateHealth(int)
     * 6)   AttackCharacter(CharacterScript)
     *      ***** Action Points *****
     * 7)   PayAP(int) 
     *      ***** Battle Stats *****
     * 8)   SetStrength(int)
     * 9)   SetDefense(int)
     * 10)  UseAbility(CharacterScript, int)
     *      ***** Initialization *****
     * 11)  SetStats(string, int, int, int, int, int, int)
     * 12)  CharacterClass(string, string, string, int)
     * -------------------------------------------------- */

    /*              STATUS FUNCTIONS
     *~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
    /* Function Name:   Turn End Reset Values
     * Input:           None, called in TacticalManager.cs during takeAct == ActState.SELECT
     * Output:          Allow this unit to move, and refill their AP                        */
    public void TurnEndResetValues()
    {
        TempSTR = TempINT = TempMove = 0;
    }
    public void TurnStartResetValues()
    {
        canMove = true;
        currAP = MaxAP;
        TempDEF = TempMYS = 0;
    }

    /*         LOCATION AND MOVEMENT FUNCTIONS
     *~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
    /* Function Name:   Has Moved
     * Input:           Called during TacticalManager.cs in WaitForPlayerInput() during ActState.MOVE
     * Output:          Set "canMove" to false                                              */
    public void HasMoved()
    {
        canMove = false;
    }

    /* Function Name: MoveCharacter
     * Input: List<Vector3Int>
     * Output: Update this unit's stored position and move unit to new location on board.
     * 
    /* Function Name:   Move Character
     * Input:           Vector3, Vector3, Vector3Int
     * Output:          Update this unit's position                                         */
    public IEnumerator MoveCharacter(Vector3 xMovement, Vector3 yMovement, Vector3Int newPosition)
    {
        SetPos(newPosition.x, newPosition.y);

        float speed = 10;

        while ((xMovement - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, xMovement, speed * Time.deltaTime);
            Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z);
            yield return null;
        }
        yMovement.x = transform.position.x;
        yMovement.y += 2;
        while ((yMovement - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, yMovement, speed * Time.deltaTime);
            Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z);
            yield return null;
        }
        /*
         * Old Code:
        while (transform.position != xMovement)
        {
            transform.position = Vector3.MoveTowards(transform.position, xMovement, speed);
            Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z);
            yield return new WaitForSeconds(1);
        }
        yMovement.x = transform.position.x;
        yMovement.y += 2;
        while (transform.position != yMovement)
        {
            transform.position = Vector3.MoveTowards(transform.position, yMovement, speed);
            Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z);
            yield return new WaitForSeconds(1);
        }
        */

    }
    // New MoveCharacter(List<Vector3> path)
    //
    //
    //
    public IEnumerator NewMoveCharacter(List<Vector3> movementPath, int movement = 99)
    {
        float speed = 10;
        //foreach (Vector3 newposition in movementPath)
        for (int i = 0; i < movementPath.Count; i++)
        {
            SetPos((int)(movementPath[i].x/4), (int)(movementPath[i].y/4));
            
            Vector3 position = movementPath[i];
            //transform.position = Vector3.MoveTowards(transform.position);
            Vector3 modifiedPosition = position;
            //modifiedPosition.y += 2;
            modifiedPosition.y += 2;
            modifiedPosition.x += 2;
            while (transform.position != modifiedPosition)
            {
                transform.position = Vector3.MoveTowards(transform.position, modifiedPosition, speed * Time.deltaTime);
                Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z);
                yield return null;
            }
            movement--;
            if (movement == 0) break;
        }
    }
    /* Function Name:   Set Pos
     * Input:           int, int
     * Output:          Update the stored grid location*/
    public void SetPos(int x, int y)
    {
        xPos = x;
        yPos = y;
    }
   
    /*                  HEALTH FUNCTIONS
     *~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
    /* Function Name:   Update Health
     * Input:           int
     * Output:          If input was negative, decrease health
     *                      If less than or equal to 0, set to 0, set dead to true
     *                  If input was positive, increase health
     *                      If above max health, set to max health
     *                  Update this unit's healthbar as well                                */
    public void UpdateHealth(int increment)
    {
        GameObject damageTextInstance = Instantiate(damageTextPrefab, gameObject.transform);
        damageTextInstance.transform.GetChild(0).GetComponent<TextMeshPro>().SetText(increment.ToString());

        //Perform update
        CurrHP += increment;

        if (increment < 0)
        {
            if (CurrHP <= 0)
            {
                CurrHP = 0;
                dead = true;
            }
        }
        else if (increment > 0)
        {
            if (CurrHP > MaxHP)
            {
                CurrHP = MaxHP;
            }
        }

        healthBar.SetHealth(CurrHP);
    }

    /* Function Name:   AttackCharacter
     * Input:           CharacterScript - The target receiving damage
     * Output:          Update the targetted unit's health and health bar                   */
    public void AttackCharacter(CharacterScript enemyChar)
    {
        int damage = enemyChar.Defense - Strength;          //Calculate the damage the enemey would take
        PayAP(1);                                           //The attacker must pay the cost to attack (1);

        if (damage < 0)
        {
            enemyChar.UpdateHealth(damage);                     //If they take damage, update their health,
            enemyChar.healthBar.SetHealth(enemyChar.CurrHP);    //And set their health bar.
        }
        else
        {
            enemyChar.UpdateHealth(-1);
            enemyChar.healthBar.SetHealth(enemyChar.CurrHP);
        }
    }
    /* Function Name:   Ability Strike Character
     * Input:           CharacterScript - The target receiving damage, int - Skill index being used
     * Output:          Update the targetted unit's health and health bar                   */
    public void AbilityStrikeCharacter(CharacterScript enemyChar, int skillIndex, int strikeForce)
    {
        int damage = 0;

        if (trueClass.Skills[skillIndex].Base.Type1 == DamageType.Physical)
            damage = enemyChar.Defense;
        else if (trueClass.Skills[skillIndex].Base.Type1 == DamageType.Magic)
            damage =  enemyChar.Mysticism;

        damage -= strikeForce;

        if (damage < 0)
        {
            enemyChar.UpdateHealth(damage);                     //If they take damage, update their health,
            enemyChar.healthBar.SetHealth(enemyChar.CurrHP);    //And set their health bar.
        }
        else
        {
            enemyChar.UpdateHealth(-1);
            enemyChar.healthBar.SetHealth(enemyChar.CurrHP);
        }
    }

    /*              ACTION POINT FUNCTIONS
     *~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
    /* Function Name:   Pay AP
     * Input:           int
     * Output:          Decrease this unit's available AP by the input
     *
     * NOTE:
     *  No handling for less than 0, the functions that call this must check that they CAN pay it.*/
    public void PayAP(int cost)
    {
        currAP -= cost;
    }

    /*              BATTLE STATS FUNCTIONS
     *~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
    /* Function Name:   Set Strength
     * Input:           int
     * Output:          Change Strength stat by input                                       */
    private void SetStrength(int updateBy)
    {
        TempSTR += updateBy;
    }

     /* Function Name:  Set Defense
     * Input:           int
     * Output:          Change Defense stat by input                                        */
    private void SetDefense(int updateBy)
    {
        TempDEF += updateBy;
    }

    /* Function Name:  Set Move
    * Input:           int
    * Output:          Change Movement stat by input                                        */
    private void SetMove(int updateBy)
    {
        TempMove += updateBy;
    }

    /* Function Name:   Use Ability
    * Input:            CharacterScript (Target),   int (Index)
    * Output:           Use the skill of this unit's class at the [Index],
                            on the Target.                                                  */
    public void UseAbility(CharacterScript unitScript, int skillIndex)
    {
        bool useSkill = true;
        Skill activeSkill = trueClass.Skills[skillIndex];

        int aPower = activeSkill.Base.Power;
        if (!activeSkill.Base.SetPower)
        {
            if (activeSkill.Base.Type1 == DamageType.Magic)
                aPower = Intelligence;
            else
                aPower = Strength;
        }

        int aPower2 = activeSkill.Base.Power2;
        if (!activeSkill.Base.SetPower)
        {
            if (activeSkill.Base.Type1 == DamageType.Magic)
                aPower2 = Intelligence;
            else
                aPower2 = Strength;
        }

        //Does the skill require us to stay in place? (Have NOT moved yet)
        if (activeSkill.Base.SpecialSkill && activeSkill.Base.MyReq == SpecialReqs.Rooted)
            if (!canMove)
                useSkill = false;

        //If it does and we haven't, or if it did not, use skill as normal.
        if (useSkill)
        {
            PayAP(TrueClass.Skills[skillIndex].COST);
            //All skills have a primary target/stat effected
            switch (activeSkill.Base.TargetStat)
            {
                case StatHit.HP: unitScript.UpdateHealth(aPower); break;
                case StatHit.HPAttack: AbilityStrikeCharacter(unitScript, skillIndex, aPower); break;
                case StatHit.DEF: unitScript.SetDefense(aPower); break;
                case StatHit.STR: unitScript.SetStrength(aPower); break;
                default: break;
            }

            //Some skills have a second target
            if (activeSkill.Base.SpecialSkill)
            {
                if (activeSkill.Base.MyReq == SpecialReqs.MultiStatsSelfEnemy || activeSkill.Base.MyReq == SpecialReqs.MultiStatsSingleTarget)
                {
                    switch (activeSkill.Base.TargetStat2)
                    {
                        case StatHit.HP: unitScript.UpdateHealth(aPower2); break;
                        case StatHit.HPAttack: AbilityStrikeCharacter(unitScript, skillIndex, aPower2); break;
                        case StatHit.DEF: unitScript.SetDefense(aPower2); break;
                        case StatHit.STR: unitScript.SetStrength(aPower2); break;
                        case StatHit.MOVE: unitScript.SetMove(aPower2); break;
                    }
                }
            }
        }
    }

    /*           INITIALIZATION FUNCTIONS
     *~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
    /* Function Name:   Set Stats
    * Input:            string, int, int, int, int, int, int
    * Output:           Initialize by input                                                 */
    public void SetStats(string myName, int currXPos, int currYPos, UnitClass generateClass)
    {
        unitName = myName;
        xPos = currXPos;
        yPos = currYPos;
        trueClass = generateClass;
        trueClass.SetupUnit();
        TurnEndResetValues();
        TurnStartResetValues();
    }

    public void ForceStats()
    {
        trueClass.SetupUnit(myClassCore);
        TurnEndResetValues();
        TurnStartResetValues();
        healthBar.SetHealth(MaxHP);
    }

    public void CreateStats(int minHP, int maxHP, int BaseRange, int BaseMovement, int minDMG, int maxDMG, int minDEF, int maxDEF, int speed)
    {
        trueClass.Core.RandomizeStats(minHP, maxHP, BaseRange, BaseMovement, minDMG, maxDMG, minDEF, maxDEF, speed);
    }
}
