using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RavenMovement : MonoBehaviour
{
    // Serialized Fields
    [SerializeField] float moveSpeed = 6f; // horizontal speed

    // Cached Fields
    Rigidbody2D myRigidBody;
    private Vector2 screenBounds;

    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        myRigidBody.velocity = new Vector2(moveSpeed, myRigidBody.velocity.y);
        
        // This defines the screen boundaries on an x & y axis
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        
        // Check if the position of the raven has exceeded the right side of the screen (plus a little padding)
        if (transform.position.x > screenBounds.x + 40f)
        {
            Destroy(gameObject); // Don't want a million ravens flying around
        }
    }
}
