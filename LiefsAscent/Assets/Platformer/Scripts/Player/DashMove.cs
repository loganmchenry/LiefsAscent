using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Script that can be added to the player component and set things like dash speed and dash distance  */
/* Update 2.0: 3/17/2021
 * - added afterImages to the dash so that it looks nicer
 * - made it so that the player has to push "C" in order to dash (this was set in the unity editor, Edit -> Project Settings -> Input Manager -> should be the last one named "dash")
 * - implemented the dash cooldown that I wanted to do
 
 */
public class DashMove : MonoBehaviour
{


    [Header("Private Dash Fields")]
    // dashing or nah
    private bool isDashing = false;

    // keeps track how much longer the dash should be happening
    private float dashTimeLeft;

    // keep track of the last x coordinate where we placed an afterimage
    private float lastImageXPos;

    // keep track of the last time we started a dash (used for cooldown)
    private float lastDash = -100f;

    
    [Header("Public Dash Fields")]
    // how long the dash takes
    public float dashTime;

    // how fast the character should move when dashing
    public float dashSpeed;

    // how far the afterImages should be placed when dashing
    public float distanceBetweenImages;

    // how long we have to wait to dash again
    public float dashCooldown;




    [Header("Player Fields")]
    private Rigidbody2D rb;
    private float movementInputDirection;
    [SerializeField] GameObject player;

    [Header("Movement Fields")]
    Movement move;
    
    // this is just normal movement speed
    public float speed = 16f;
    
    

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        move = player.GetComponent<Movement>();

    }

    // This is so that our player doesn't dash/ slide forever
    private void FixedUpdate()
    {
        if (!isDashing)
            rb.velocity = new Vector2(movementInputDirection * speed, rb.velocity.y);
    }


    // Update is called once per frame
    void Update()
    {

        CheckInput();
        CheckDash();
    }

    private void CheckInput()
    {
        movementInputDirection = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(ControlManager.instance.MoveControls["Dash"]))
        {
            if (ControlManager.instance.inputsActive) // Dont do anything if we are in the middle of changing the controls
            {
                if (Time.time >= (lastDash + dashCooldown))
                {
                    AttemptToDash();
                }
            }
        }
    }


    private void AttemptToDash()
    {
        isDashing = true;
        dashTimeLeft = dashTime;
        lastDash = Time.time;

        // get gameobject from pool
        AfterImagePool.Instance.GetFromPool();

        lastImageXPos = transform.position.x;
        FindObjectOfType<SoundManager>().Play("PlayerDash");
    }


    // responsible with setting dash velocity
    // also checks if we should be dashing or if we should stop
    private void CheckDash()
    {
        if (isDashing)
        {
            if(dashTimeLeft > 0)
            {
                move.canMove = false;
                

                rb.velocity = new Vector2(dashSpeed * move.facingDirection, rb.velocity.y);
                dashTimeLeft -= Time.deltaTime;

                // need to check if enough distance has passed to place another afterimage
                if (Mathf.Abs(transform.position.x - lastImageXPos) > distanceBetweenImages)
                {
                    AfterImagePool.Instance.GetFromPool();
                    lastImageXPos = transform.position.x;
                }
            }

            if(dashTimeLeft <= 0)
            {
                isDashing = false;
                move.canMove = true;
                
            }
            
        }
    }

    public int GetFacingDirection()
    {
        return move.facingDirection;
    }

    public bool GetDashStatus()
    {
        return isDashing;
    }



}
