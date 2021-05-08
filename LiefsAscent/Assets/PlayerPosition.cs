using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPosition : MonoBehaviour
{

    // maybe check for demo here?
    public bool hasSword = false;

    [HideInInspector]
    public bool demoDone = false;

    [SerializeField]
    private GameObject player;

    private void Awake()
    {
        if(hasSword == false){
            player.GetComponent<PlayerCombatController>().disableCombat();
        }
        PlayerStats.instance.retrievePlayerInfo();
        
    }

    public void savePos()
    {
        PlayerStats.instance.updatePlayerInfo();
    }




}
