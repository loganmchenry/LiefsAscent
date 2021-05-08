/*
 * Class: UnitClass
 * Uses: Used to determine what skills are available to the character.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UnitClass
{
    [SerializeField] UnitClassCore _core;
    [SerializeField] int scaleFactor;
    public List<Skill> Skills { get; set; }

    public void SetupUnit()
    {

        Skills = new List<Skill>();
        foreach (var skill in _core.AllSkills)
        {
            Skills.Add(new Skill(skill.SkillBase));

            //For now, we will limit ourselves to 4 skills.
            if (Skills.Count >= 4)
                break;
        }

        CurrHP = MaxHP;
    }
    public void SetupUnit(UnitClassCore newCore)
    {
        _core = newCore;

        Skills = new List<Skill>();
        foreach (var skill in _core.AllSkills)
        {
            Skills.Add(new Skill(skill.SkillBase));

            //For now, we will limit ourselves to 4 skills.
            if (Skills.Count >= 4)
                break;
        }

        CurrHP = MaxHP;
    }
    public UnitClassCore Core { get { return _core; } }
    public int ScaleFactor { get { return scaleFactor; } }
    public int MaxHP { get { return Core.MaxHealth; } }
    public int CurrHP { get; set; }
    public int AP { get { return Core.ActionPoint; } }
    public int MOVE { get { return Core.Move; } }
    public int ATKRange { get { return Core.AttackRange; } }
    public int STR { get { return Core.Strength; } }
    public int DEF { get { return Core.Defense; } }
    public int INT { get { return Core.Intelligence; } }
    public int MYS { get { return Core.Mysticism; } }
}
