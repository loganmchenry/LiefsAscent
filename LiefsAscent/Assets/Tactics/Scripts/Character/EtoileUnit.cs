/*  Documentation:
 *  Intended use
 *      Manage the animations of an enemy unit
 *      
 *  Last Documentation Update: 27 April 2021
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EtoileUnit : MonoBehaviour
{
    /*              Variables for Animation
     * ------------------------------------------------------- */
    Animator anim;
    private GameObject thisUnit;

    // Initialization, grab current gameobject and the gameobjects animator
    void Start()
    {
        anim = GetComponent<Animator>();
        thisUnit = GameObject.Find("Warrior");
    }

    // Activate animations of current unit related to corresponding STATES from TacticalManager script
    void Update()
    {
        //if in MOVE state, toggle isWalking to on
        if (TacticalManager.takeAct == ActState.MOVE && TacticalManager.selected == thisUnit && !anim.GetCurrentAnimatorStateInfo(0).IsName("Tactics_Etoile_Attack"))
        {
            anim.SetBool("isWalking", true);
        }
        else //if not, remain in Idle State
        {
            anim.SetBool("isWalking", false);
        }
        //if in ATTACK state, trigger attack
        if (TacticalManager.takeAct == ActState.ATTACK && TacticalManager.selected == thisUnit && !anim.GetCurrentAnimatorStateInfo(0).IsName("Tactics_Etoile_Attack"))
        {
            anim.SetBool("isWalking", false);
            anim.SetTrigger("attack");
        }
    }
}
