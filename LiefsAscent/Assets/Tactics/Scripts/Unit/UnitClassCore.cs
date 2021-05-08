using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Classes", menuName = "Classes/Create new Class")]
public class UnitClassCore : ScriptableObject
{
    [SerializeField] string className;

    [SerializeField] DamageType type1;
    [SerializeField] RoleType type2;

    /*  Base stats
     *  These stats shouldn't be altered except from the Inspector
     */

    [SerializeField] int maxHealth;
    [SerializeField] int actionPoint;
    [SerializeField] int range;
    [SerializeField] int movement;

    [SerializeField] int strength;
    [SerializeField] int defense;
    [SerializeField] int intelligence;
    [SerializeField] int mysticism;
    [SerializeField] int turnPosition;


    [SerializeField] List<ClassSkills> allSkills;


    public string ClassName { get { return className; } }
    public List<ClassSkills> AllSkills { get { return allSkills; } }

    public int MaxHealth { get { return maxHealth; } }
    public int ActionPoint { get { return actionPoint; } }
    public int AttackRange { 
        get { return range; }
        set { range = value; }
    }
    public int Move { get { return movement; } }
    public int Strength { get { return strength; } }
    public int Defense { get { return defense; } }
    public int Intelligence { get { return intelligence; } }
    public int Mysticism { get { return mysticism; } }
    public int MyTurn { get { return turnPosition; } }
    public void RandomizeStats(int minHP, int maxHP, int BaseRange, int BaseMovement, int minDMG, int maxDMG, int minDEF, int maxDEF, int speed)
    {
        maxHealth = Random.Range(minHP, maxHP);                 //Range of health
        actionPoint = 1;    //Auto attack dummy
        movement = BaseMovement + Random.Range(0, 2);           //Movement up to +2
        range = BaseRange + Random.Range(0, 2);                 //Attack range up to +2
        strength = intelligence = Random.Range(minDMG, maxDMG); //Base attack range
        defense = mysticism = Random.Range(minDEF, maxDEF);     //Base defense range (Remember, it will always deal/be dealt 1 damage at minimum, never 0)
        turnPosition = speed + Random.Range(-2,2);              //Turn order position
    }
    public void LokiUpgrade(int HP, int DMG, int DEF)
    {
        maxHealth += HP;
        range += 1;
        movement += 2;
        strength = intelligence += DMG;
        defense = mysticism += DEF;
        turnPosition += 1;
    }
}

[System.Serializable]
public class ClassSkills
{
    [SerializeField] SkillCore skillBase;
    [SerializeField] StatHit checkStat;
    [SerializeField] int statReq;

    public SkillCore SkillBase
    {
        get { return skillBase; }
    }
    public StatHit CheckStat
    {
        get { return checkStat; }
    }
    public int StatReq
    {
        get { return statReq; }
    }
}

public enum DamageType
{
    None,
    Magic,
    Physical,
}

public enum RoleType
{
    None,
    DPS_Melee,      //Thieves
    DPS_Range,      //Archers
    Support,        //Tanks & Mages
}