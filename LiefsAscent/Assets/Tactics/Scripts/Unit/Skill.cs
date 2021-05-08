using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill
{
    public SkillCore Base { get; set; }
    public int COST { get; set; }

    public Skill(SkillCore theCore)
    {
        Base = theCore;
        COST = theCore.Cost;
    }
}