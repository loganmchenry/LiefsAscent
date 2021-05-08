/*  Documentation:
 *  Intended use
 *      Manage the animations of an enemy unit
 *      
 *  Last Documentation Update: 4 May 2021
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draugr0 : MonoBehaviour
{
    /*              Variables for Animation
     * ------------------------------------------------------- */
    Animator anim;
    private GameObject thisUnit;

    // Initialization, grab current gameobject and the gameobjects animator
    void Start()
    {
        anim = GetComponent<Animator>();
        thisUnit = GameObject.Find("Draugr0");
    }

    // Activate animations of current unit related to corresponding STATES from TacticalManager script
    void Update()
    {
        //if in ENEMYTURN & is not attacking, then activate walking animation
        if (TacticalManager.enemyMove == true && TacticalManager.selected == thisUnit && !anim.GetCurrentAnimatorStateInfo(0).IsName("Tactics_Draugr_Attack"))
        {
            anim.SetBool("isWalking", true);
        }
        else //if not, remain in idle animation
        {
            anim.SetBool("isWalking", false);
        }
        //if in ATTACK state, trigger attack
        if (TacticalManager.enemyAttack == true && TacticalManager.selected == thisUnit && !anim.GetCurrentAnimatorStateInfo(0).IsName("Tactics_Draugr_Attack"))
        {
            anim.SetBool("isWalking", false);
            anim.SetTrigger("attack");
        }
    }
}
