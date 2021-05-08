using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardsManager : MonoBehaviour
{
    private static bool victory; // set after winning a tactical encounter
    public static int keysCollected;
    public static bool bossFinished; // set after beating a boss
    public static bool enterBossFight;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static bool didWin()
    {
        return victory;
    }

    public static void setVictoryToTrue()
    {
        victory = true;
    }

    public static void setVictoryToFalse()
    {
        victory = false;
    }
}
