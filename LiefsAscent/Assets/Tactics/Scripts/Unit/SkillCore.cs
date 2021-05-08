using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill", menuName = "Skill/Create new skill")]
public class SkillCore : ScriptableObject
{
    [SerializeField] string sName;      //What is the name of the skill as it will show up on the UI?

    [TextArea]
    [SerializeField] string description;//Give a basic description of what happens

    [SerializeField] DamageType type1;      //Used for Magic or Strength based damage
                                                //Note, if you are doing a buff/debuff, use Magic type
    [SerializeField] StatHit targetStat;    //What stat is effected of the main target?
    [SerializeField] bool useSetPower;      //Is the power of this ability set by inspector?
    [SerializeField] int powerStat1;            //If yes, by how much?

    [SerializeField] StatHit targetStat2;   //If a second stat is also effected, what is it? (Set SpecialReqs to MultiStat in inspector)
                                            //If this skill effects the enemy and current unit, targetstat2 is the current unit.
    [SerializeField] bool useSetPower2;     //Is the power of the secondary stat set by inspector?
    [SerializeField] int powerStat2;        //How much does the above stat get affected?
    [SerializeField] int range;             //What is the range?
    [SerializeField] int cost;              //How much AP does this cost?

    [SerializeField] bool isSpecial;            //Are there any special requirements to use this skill?
    [SerializeField] SpecialReqs requirement;       //If yes, what is the requirements? (See enumeration below)
    [SerializeField] TargetTypes myTarget;      //Who are the targets? Enemies, allies, and/or self?

    /*  The following list are getters for the above private fields
     *  
     */

    public string SkillName { get { return sName; } }
    public string Description { get { return description; } }
    public DamageType Type1 { get { return type1; } }
    public StatHit TargetStat { get { return targetStat; } }
    public bool SetPower { get { return useSetPower; } }
    public StatHit TargetStat2 { get { return targetStat2; } }
    public bool SetPower2 { get { return useSetPower2; } }
    public int Power { get { return powerStat1; } }
    public int Power2 { get { return powerStat2; } }
    public int Range { get { return range; } }
    public int Cost { get { return cost; } }
    public bool SpecialSkill { get { return isSpecial; } }
    public SpecialReqs MyReq { get { return requirement; } }
    public TargetTypes MyTarget { get { return myTarget; } }
}

public enum StatHit
{
    None,
    HP,     //HEALING and DAMAGING
    HPAttack,   //Perform an attack using INT or STR (Calculate damage by subtracting target MYS or DEF respectively)
    DEF,    //Change the tempDEF of a unit(s)
    STR,    //Change the tempSTR of a unit(s)
    MOVE,   //Change the tempMOVE of a unit(s)
}

    //List of targets
public enum TargetTypes
{
    None,                   //Used to catch any unlisted
    Self,                   //Target self
    AlliesALL,              //Target anyone in your party, including yourself.
    AlliesOTHERS,           //Target a different unit in your party
    Enemies,                //Target a specific enemy
    EnemyAreaOfEffect,      //Affect all enemy units in range
}

public enum SpecialReqs
{
    None,
    Rooted,                 //The unit has NOT moved yet
    MultiStatsSingleTarget, //The skill hits 2 stats on the same target
    MultiStatsSelfEnemy,    //The skill affects both the current unit and an enemy unit.
}
