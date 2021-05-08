using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Script that gets attched to the party members, can have them walking around and stuff.


public class PartyMovement : MonoBehaviour
{




    [SerializeField]
    private float
        groundCheckDistance,
        wallCheckDistance,
        movementSpeed;


    [SerializeField]
    private Transform
        groundCheck,
        wallCheck;

    [SerializeField]
    private LayerMask
        whatIsGround,
        whatIsPlayer;

    private bool
        isRunning;

    private int
        facingDirection;

    private Vector2
        movement;

    private bool
        groundDetected,
        wallDetected;

    [SerializeField]
    private GameObject partyMember;

    private Rigidbody2D memberRb;
    private Animator memberAnim;





    // Start is called before the first frame update
    void Start()
    {
        memberRb = partyMember.GetComponent<Rigidbody2D>();
        memberAnim = partyMember.GetComponent<Animator>();

        facingDirection = 1;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateWalkingState();
    }


    private void UpdateWalkingState()
    {
        groundDetected = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
        wallDetected = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, whatIsGround);

        if(!groundDetected || wallDetected)
        {
            // if there is no ground or if there is a wall turn around
            FlipSprite();
        }
        else
        {
            // move the party member
            movement.Set(movementSpeed * facingDirection, memberRb.velocity.y);
            memberRb.velocity = movement;
        }
    }





    private void FlipSprite()
    {
        facingDirection *= -1;
        partyMember.transform.Rotate(0.0f, 180.0f, 0.0f);
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, new Vector2(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(wallCheck.position, new Vector2(wallCheck.position.x + wallCheckDistance, wallCheck.position.y));

    }


}
