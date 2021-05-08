using System;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]

public class Movement : MonoBehaviour
{
    // Kinematic equations initial velocity == 0
    // to solve for gravity
    // deltaMovement = velocityInitial * time + (acceleration * time**2 )/ 2
    // jumpHeight = gravity * timeToJumpApex**2 / 2 --> gravity =  2 * jumpHeight / timeToJumpApex**2

    // to solve for jumpVelocity
    // velocityFinal = velocityInitial + acceleration * time
    // jumpVelocity = gravity * timeToJumpApex

    // Public Fields
    public bool canMove = true;
    public int facingDirection = 1; // which direction we are facing, gets changed when we flipped
    public bool canFlip = true;

    // Serialized Fields
    [SerializeField] float jumpHeight = 4; //moves up x units
    [SerializeField] float timeToJumpApex = 0.5f; //half a sec
    [SerializeField] bool doubleJumpEnabled = true; // has the player unlocked double jump?
    [SerializeField] bool hasDoubleJump = true; // false when the double jump has been used, before the player returns to the ground
    [SerializeField] float moveSpeed = 12; // horizontal speed
    [SerializeField] bool canJump;

    // State Variables
    float jumpVelocity; //jumpVelocity and gravity are based off jumpHeight and timeToJumpApex
    float gravity;
    Vector3 velocity;
    // float accelerationAirborne = 0.2f; //slows upSpeed
    // float accelerationGrounded = 0.1f; //stops player from going 0 to fullspeed
    // float xsmoothing; //variable to gradually increase speed


    // Wall Sliding and Wall Jumping
    public bool isTouchingFront;
    public Transform frontCheck;
    bool wallSliding;
    public float wallSlidingSpeed;
    bool wallJumping;
    private float xWallForce;
    private float yWallForce;
    private float wallJumpTime;

    // Vine Climbing
    private int touchingVine = 0;
    private float inputVertical;
    
    

    // Knockback
    private float knockbackStartTime;
    [SerializeField]
    private float knockbackDuration;

    private bool knockback;

    [SerializeField]
    private Vector2 knockbackSpeed;

 



    // Cached References
    Controller2D controller;
    Animator animator;
    Rigidbody2D myRigidBody;

    ///<summary>
    /// calculates gravity and jumpVelocity based on jumpheight and timeToJumpApex
    ///</summary>
  
    private void Start() {
        controller = GetComponent<Controller2D>();
        animator = GetComponent<Animator>();
        myRigidBody = GetComponent<Rigidbody2D>();
        gravity = -(2* jumpHeight / Mathf.Pow(timeToJumpApex,2));
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        //print("Gravity= " +gravity + "jumpVelocity= " +jumpVelocity );
    }

    ///<summary>
    /// SmoothDamp https://docs.unity3d.com/ScriptReference/Mathf.SmoothDamp.html
    /// Gradually changes a value towards a desired goal over time
    ///</summary>
    private void Update()
    {
        if (controller.collisions.below)
        {
            velocity.y = 0; //prevent gravity from accumulating on object by reseting velocity
            hasDoubleJump = true; //resets ability
            canJump = true;
        }
        if (controller.collisions.above){
            velocity.y = 0;
        }

         // so that we don't move when we are examining an item
         /*if (CanMove() == false)
             return;*/

        // wall jump and slide
        isTouchingFront = (controller.collisions.left || controller.collisions.right);
        if (isTouchingFront == true && !controller.collisions.below) {
            wallSliding = true;
        } else {
            wallSliding = false;
        }

        if (wallSliding) 
        {
            velocity = new Vector2(velocity.x, Mathf.Clamp(velocity.y, -wallSlidingSpeed, float.MaxValue));
        }


        if (canMove)
        {
            RunAndJump();
            FlipSprite();
            CheckKnockback();
            Climb();
           
        }
        else // Player should be frozen so go straight to idle animation
        {
            animator.SetBool("Running", false);
            
        }


        

        
    }

    private void RunAndJump()
    {
        // FUTURE WORK: Needs to be modified to allow any changes to whatever key for movement
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")); //get all UP/DOWN ARROW commands
        
        if ((Input.GetKeyDown(ControlManager.instance.MoveControls["Jump"])) && (controller.collisions.below || hasDoubleJump) && !knockback)
        {
            if (ControlManager.instance.inputsActive) // Dont do anything if we are in the middle of changing the controls
            {
                
                if (canJump == true)
                {
                    velocity.y = jumpVelocity; //if something collides with bottom of object allow jump
                    FindObjectOfType<SoundManager>().Play("PlayerJump");
                    canJump = false;
                }
                else
                {
                    
                    DoubleJump(); // possibly double jump
                }



                WallJump();
            }
        }

        float targetVelocityX = input.x * moveSpeed;

        // Note: As of now this causes issues with animation transitioning, will have to update later
        // velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref xsmoothing, (controller.collisions.below) ? accelerationGrounded : accelerationAirborne);
        // velocity.y += gravity * Time.deltaTime;

        velocity.x = targetVelocityX;

        if (touchingVine == 0)
        {
            velocity.y += gravity * Time.deltaTime;
            gravity = -(2 * jumpHeight / Mathf.Pow(timeToJumpApex, 2));
        }
        else if (!(Input.GetKeyDown(ControlManager.instance.MoveControls["Jump"])))
        {
            //gravity = 0;
            velocity.y = 0;
        }
        else velocity.y = jumpVelocity;

        controller.Move(velocity * Time.deltaTime);
        


        AnimationCheck();
    }

    void DoubleJump()
    {
        if(hasDoubleJump && doubleJumpEnabled && !controller.collisions.below)
            {
                velocity.y = jumpVelocity;
                hasDoubleJump =false;
            }
    }

    void WallJump()
    {
        // wall jump
            if(wallSliding == true && isTouchingFront == true) {
                wallJumping = true;
                Invoke("SetWallJumpingToFalse", wallJumpTime); //sets to false after wallJumpTime
            }
            if (wallJumping == true) 
            {
                velocity.y = jumpVelocity;
            }
    }

    void SetWallJumpingToFalse() 
    {
        wallJumping = false;
    }


    private void AnimationCheck()
    {
        // Transitioning from Idle to Run State & Vice Versa
        bool playersHorizontalSpeed = (Mathf.Abs(velocity.x) > Mathf.Epsilon) && (myRigidBody.velocity.y == 0);
        animator.SetBool("Running", playersHorizontalSpeed);

        // Transitioning to Jumping, Falling, and Idle/Run States
        // Notes: The velocity is never constant, however the 
        // y-velocity of RigidBody does stay constant at 0 on the 
        // ground making it reliable. However, it only ever goes
        // negative so using a combination of both velocitys we 
        // can ensure smooth transitioning -> jumping & falling.
        if (myRigidBody.velocity.y == 0f)
        {
            animator.SetBool("Jumping", false);
            animator.SetBool("Falling", false);
        }
        else
        {
            if (velocity.y > 0f)
            {
                animator.SetBool("Jumping", true);
                animator.SetBool("Falling", false);
            }
            if (velocity.y < 0f)
            {
                animator.SetBool("Falling", true);
                animator.SetBool("Jumping", false);
            }
        }
    }

    private void FlipSprite()
    {
        // Check to see if the Player is running -- if they are then we check
        // to see if we should flip the sprite
        bool playersHorizontalSpeed = Mathf.Abs(velocity.x) > Mathf.Epsilon;
        bool hasChangedDirection = Mathf.Sign(velocity.x) != transform.localScale.x;
        if (playersHorizontalSpeed && hasChangedDirection && canFlip && !knockback)
        {
            // Depending on the Sign of the movement (left or right) it will flip
            // the sprite on its x-axis accordingly. 
            transform.localScale = new Vector2(Mathf.Sign(velocity.x), 1f);
            facingDirection *= -1;
        }
    }

    public void DisableFlip()
    {
        canFlip = false;
    }

    public void EnableFlip()
    {
        canFlip = true;
    }


    public void Knockback(int direction)
    {
        knockback = true;
        knockbackStartTime = Time.time;
        myRigidBody.velocity = new Vector2(knockbackSpeed.x * direction, knockbackSpeed.y);
    }

    private void CheckKnockback()
    {
        if(Time.time >= knockbackStartTime + knockbackDuration && knockback)
        {
            knockback = false;
            myRigidBody.velocity = new Vector2(0.0f, myRigidBody.velocity.y);
        }
    }


    public void Climb()
    {
        animator.SetBool("canClimb", false);
        animator.SetFloat("isClimbing", 0);
        if (touchingVine > 0)
        {
            animator.SetBool("canClimb", true);
            FindObjectOfType<SoundManager>().Play("PlayerClimb");
            inputVertical = Input.GetAxisRaw("Vertical");
            animator.SetFloat("isClimbing", inputVertical);
            myRigidBody.velocity = new Vector2(myRigidBody.position.x, inputVertical * moveSpeed);
            gravity = 0;
            if ((Input.GetKeyDown(ControlManager.instance.MoveControls["Jump"])))
            {

                Vector3 pos = GameObject.FindGameObjectWithTag("Player").transform.position;
                for (int i = 1; i < jumpHeight; i += 2)
                {
                    
                    GameObject.FindGameObjectWithTag("Player").transform.position = new Vector3(pos.x, pos.y + i, pos.z);
                }
                gravity = (2 * jumpHeight / Mathf.Pow(timeToJumpApex, 2));
                
            }
        }
       
    }
    // set touchingVine to true when the player collides with vines
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Vines")
        {
            touchingVine += 1;

        }
    }

    // set touchingVine to false when player stops colliding with vines
    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.tag == "Vines")
        {
            animator.SetBool("canClimb", false);
            FindObjectOfType<SoundManager>().Stop("PlayerClimb");
            touchingVine -= 1;          
        }
    }


    // Function that checks if the player can move
    // Right now only if they have the examine window open they can't move
    // Can add on differnt conditions later one like: in a dialogue etc
    /*bool CanMove()
    {
        bool can = true;
        // If the examine window is open stop moving
        if (FindObjectOfType<InteractionSystem>().isExamining)
            can = false;
        // If the inventory window is open stop moving
        if (FindObjectOfType<InventorySystem>().isOpen)
            can = false;
        return can;
    }*/

}
