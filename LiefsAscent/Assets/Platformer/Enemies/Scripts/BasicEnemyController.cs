using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyController : MonoBehaviour
{
    // Enemy states, easy to track if we want to add different states to enemies
    private enum State
    {
        Walking,
        Knockback,
        Dead
    }


    // keeps track of currentState
    private State currentState;


    [SerializeField]
    private float
        groundCheckDistance,
        wallCheckDistance,
        movementSpeed,
        maxHealth,
        knockbackDuration,
        touchDamageCooldownm,
        touchDamage,
        touchDamageWidth,
        touchDamageHeight;

    [SerializeField]
    private Transform
        groundCheck,
        wallCheck,
        touchDamageCheck;

    [SerializeField]
    private LayerMask 
        whatIsGround,
        whatIsPlayer;

    [SerializeField]
    private Vector2
        knockbackSpeed; // allow x and y speed

    private float 
        currentHealth,
        lastTouchDamageTime,
        knockbackStartTime;

    private float[] attackDetails = new float[2];

    private int 
        facingDirection,
        damageDirection;

    // update values movement instead of new v2 everytime we update velocity
    private Vector2 
        movement,
        touchDamageBotLeft,
        touchDamageTopRight;


    private bool
        groundDetected,
        wallDetected;

    private GameObject alive;
    private Rigidbody2D aliveRb;
    private Animator aliveAnim;

    private void Start()
    {
        alive = transform.Find("Alive").gameObject;
        aliveRb = alive.GetComponent<Rigidbody2D>();
        aliveAnim = alive.GetComponent<Animator>();

        currentHealth = maxHealth;
        facingDirection = 1;
    }

    private void Update()
    {
        //Determine which state is active 
        // then call the update function based on state 
        switch (currentState)
        {
            case State.Walking:
                UpdateWalkingState();
                break;
            case State.Knockback:
                UpdateKnockbackState();
                break;
            case State.Dead:
                UpdateDeadState();
                break;

        }
    }

    // 3 functions for our states
    // 1. enter
    // 2. update
    // 3. exit

    //--WALKING STATE-------------------------------------

    private void EnterWalkingState()
    {

    }

    private void UpdateWalkingState()
    {
        groundDetected = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
        wallDetected = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, whatIsGround);


        CheckTouchDamage();

        if(!groundDetected || wallDetected)
        {
            //Flip enemy
            Flip();
        }
        else
        {
            // move enemy
            movement.Set(movementSpeed * facingDirection, aliveRb.velocity.y);
            aliveRb.velocity = movement;
        }
    }

    private void ExitWalkingState()
    {

    }


    // -- KNOCKBACK STATE ------------------------------------

    private void EnterKnockbackState()
    {
        // knockback start
        knockbackStartTime = Time.time;
        movement.Set(knockbackSpeed.x * damageDirection, knockbackSpeed.y);
        aliveRb.velocity = movement;
        aliveAnim.SetBool("Knockback", true);

        
    }

    private void UpdateKnockbackState()
    {
        if(Time.time >= knockbackStartTime + knockbackDuration)
        {
            SwitchState(State.Walking);
        }
    }

    private void ExitKnockbackState()
    {
        aliveAnim.SetBool("Knockback", false);
    }

    // -- DEAD STATE ---------------------------------------
    private void EnterDeadState()
    {
        PlayerStats.instance.isGoblin = true;
        Destroy(gameObject);
    }

    private void UpdateDeadState()
    {

    }

    private void ExitDeadState()
    {

    }

    // -- OTHER FUNCTIONS ------------------------------------------------

    
    private void Damage(float[] attackDetails)
    {

        Debug.Log("taking damage :0");
        // attack damage is in the first index so we subtract from health
        currentHealth -= attackDetails[0];

        // determine damage direction
        if (attackDetails[1] > alive.transform.position.x)
        {
            // xpos of player is greater than xpos of enemy
            // player should be facing enemy
            damageDirection = -1;
        }
        else
        {
            damageDirection = 1;
        }

        //Hit particles?

        //knockback state
        if(currentHealth > 0.0f)
        {
            // loads the tactical scene
            SceneLoader.instance.LoadTacticalScene(gameObject);

            // if enemy still alive -> knockback
            SwitchState(State.Knockback);
        }
        else if(currentHealth <= 0.0f)
        {
            // loads the tactical scene 
            SceneLoader.instance.LoadTacticalScene(gameObject);

            SwitchState(State.Dead);
        }

        

    }

    private void CheckTouchDamage()
    {
        //check cooldown
        if(Time.time >= lastTouchDamageTime + touchDamageCooldownm)
        {
            // determine two corners of area
            touchDamageBotLeft.Set(touchDamageCheck.position.x - (touchDamageWidth / 2), touchDamageCheck.position.y - (touchDamageHeight / 2));
            touchDamageTopRight.Set(touchDamageCheck.position.x + (touchDamageWidth / 2), touchDamageCheck.position.y + (touchDamageHeight / 2));


            // store gameoBject the overlap area detects
            Collider2D hit = Physics2D.OverlapArea(touchDamageBotLeft, touchDamageTopRight, whatIsPlayer);

            if(hit != null)
            {
                //overlap area detected player
                lastTouchDamageTime = Time.time;
                attackDetails[0] = touchDamage;
                attackDetails[1] = alive.transform.position.x; // so we can apply knockback in the right direciton
                hit.SendMessage("Damage", attackDetails);
            }
        }
    }






    private void Flip()
    {
        facingDirection *= -1;
        alive.transform.Rotate(0.0f, 180.0f, 0.0f);

    }



    // takes care of swapping between states
    private void SwitchState(State state)
    {
        // take care of calling exit function of current state
        switch (currentState)
        {
            case State.Walking:
                ExitWalkingState();
                break;
            case State.Knockback:
                ExitKnockbackState();
                break;
            case State.Dead:
                ExitDeadState();
                break;
        }

        // take care of enter function for state we want to go into
        switch (state)
        {
            case State.Walking:
                EnterWalkingState();
                break;
            case State.Knockback:
                EnterKnockbackState();
                break;
            case State.Dead:
                EnterDeadState();
                break;
        }

        currentState = state;
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, new Vector2(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(wallCheck.position, new Vector2(wallCheck.position.x + wallCheckDistance, wallCheck.position.y));

        // corners for box
        Vector2 botLeft = new Vector2(touchDamageCheck.position.x - (touchDamageWidth / 2), touchDamageCheck.position.y - (touchDamageHeight / 2));
        Vector2 botRight = new Vector2(touchDamageCheck.position.x + (touchDamageWidth / 2), touchDamageCheck.position.y - (touchDamageHeight / 2));
        Vector2 topRight = new Vector2(touchDamageCheck.position.x + (touchDamageWidth / 2), touchDamageCheck.position.y + (touchDamageHeight / 2));
        Vector2 topLeft = new Vector2(touchDamageCheck.position.x - (touchDamageWidth / 2), touchDamageCheck.position.y + (touchDamageHeight / 2));


        // draw the lines
        Gizmos.DrawLine(botLeft, botRight);
        Gizmos.DrawLine(botRight, topRight);
        Gizmos.DrawLine(topRight, topLeft);
        Gizmos.DrawLine(topLeft, botLeft);
    }

}
