using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatController : MonoBehaviour
{

    // can the player attack
    [HideInInspector]
    public bool combateEnabled = false;

    [SerializeField]
    private float 
        inputTimer,
        attack1Radius,
        attack1Damage;

    [SerializeField]
    private Transform attack1HitBoxPos;

    [SerializeField]
    private LayerMask whatIsDamageable;

    private bool 
        gotInput,
        isAttacking;

    private float lastInputTime = Mathf.NegativeInfinity;

    private float[] attackDetails = new float[2];

    private Animator anim;

    private Movement MV;
    private DashMove DM;
    private PlayerLives PL;
    private InventorySystem IS;

    private void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetBool("canAttack", combateEnabled);
        MV = GetComponent<Movement>();
        DM = GetComponent<DashMove>();
        PL = GetComponent<PlayerLives>();
        IS = GetComponent<InventorySystem>();
    }

    private void Update()
    {
        CheckForEnable();
        CheckCombatInput();
        CheckAttacks();
    }


    private void CheckCombatInput()
    {
        // we attack with X
        if (Input.GetKeyDown(ControlManager.instance.ActionControls["Attack"]))
        {
            if (ControlManager.instance.inputsActive) // Dont do anything if we are in the middle of changing the controls
            {
                Debug.Log("got X");

                if (combateEnabled)
                {
                    Debug.Log("here");
                    // Attempt combat
                    gotInput = true;
                    lastInputTime = Time.time;
                }
            }
        }
    }

    private void CheckAttacks()
    {
        if (gotInput)
        {
            //perform attack
            if (!isAttacking)
            {
                Debug.Log("trying to perform attack");
                gotInput = false;
                isAttacking = true;
                FindObjectOfType<SoundManager>().Play("PlayerAttack");
                anim.SetBool("attack1", true);
                anim.SetBool("isAttacking", isAttacking);
            }
        }

        if(Time.time >= lastInputTime + inputTimer)
        {
            // wait for new input
            gotInput = false;
        }
    }

    // this function gets called when we get to the impact 
    // and damage the enemy
    private void CheckAttackHitBox()
    {
        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(attack1HitBoxPos.position, attack1Radius, whatIsDamageable);

        attackDetails[0] = attack1Damage;
        attackDetails[1] = transform.position.x;

        foreach (Collider2D collider in detectedObjects)
        {
            // send mesage call a specific function on a script on an object without knowing which script it is :D
            collider.transform.parent.SendMessage("Damage", attackDetails);

            
        }
    }

    // get called end of attack anim
    private void FinishAttack1()
    {
        isAttacking = false;
        anim.SetBool("isAttacking", isAttacking);
        anim.SetBool("attack1", false);
    }



    private void Damage(float[] attackDetails)
    {
        // if player is not dashing they can get damaged
        if (!DM.GetDashStatus())
        {
            // determine what direction player is supposed to be knocked back
            int direction;

            // damage player here using attackDetails[0]
            PL.LoseLife(attackDetails[0]);

            if (attackDetails[1] < transform.position.x)
            {
                //enemy x pos is less than player x pos, enemy is to the left of player, enemy facing right
                direction = 1;
            }
            else
            {
                direction = -1;
            }

            MV.Knockback(direction);
        }


       
    }


    // used to draw hitbox
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attack1HitBoxPos.position, attack1Radius);
    }

    // gets called when the player picks up the sword item
    // at the beginning of demo as a custom event
    public void enableCombat(){
        anim = GetComponent<Animator>();
        combateEnabled = true;
        anim.SetBool("canAttack", combateEnabled);
    }

    // gets called at the beginning of demo
    // before the user has a sword
    public void disableCombat(){
        anim = GetComponent<Animator>();
        combateEnabled = false;
        anim.SetBool("canAttack", combateEnabled);
    }


    // function that checks if the player has equipment slots
    // that are being used in the equipment UI 
    // if they aren't disableCombat, if they do enableCombat
    public void CheckForEnable()
    {
        if(IS.openSlot == 2)
        {
            disableCombat();
        }
        // else there is at least one occupied slot so enable combat
        else
        {
            enableCombat();
        }
    }


}
