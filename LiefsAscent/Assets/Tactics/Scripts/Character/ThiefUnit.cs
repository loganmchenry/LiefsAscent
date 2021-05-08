/*  Documentation:
 *  Intended use
 *      Manage the animations of a party member
 *      
 *  Last Documentation Update: 30 April 2021
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThiefUnit : MonoBehaviour
{
    /*              Variables for Animation
     * ------------------------------------------------------- */
    Animator anim;
    private GameObject thisUnit;

    // Initialization, grab current gameobject and the gameobjects animator
    void Start()
    {
        anim = GetComponent<Animator>();
        thisUnit = GameObject.Find("Thief");
    }

    // Activate animations of current unit related to corresponding STATES from TacticalManager script
    void Update()
    {
        //if in MOVE state, toggle isWalking to on
        if (TacticalManager.takeAct == ActState.MOVE && TacticalManager.selected == thisUnit && !anim.GetCurrentAnimatorStateInfo(0).IsName("Tactics_Thief_Attack"))
        {
            anim.SetBool("isWalking", true);
        }
        else //if not, remain in Idle State
        {
            anim.SetBool("isWalking", false);
        }
        //if in ATTACK state, trigger attack
        if (TacticalManager.takeAct == ActState.ATTACK && TacticalManager.selected == thisUnit && !anim.GetCurrentAnimatorStateInfo(0).IsName("Tactics_Thief_Attack"))
        {
            anim.SetBool("isWalking", false);
            anim.SetTrigger("attack");
        }
    }
}
