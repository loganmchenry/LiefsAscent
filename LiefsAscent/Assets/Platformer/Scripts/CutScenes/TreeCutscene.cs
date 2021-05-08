using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeCutscene : MonoBehaviour
{
    // Serialized Variables
    [SerializeField] Animator camAnim;
    [SerializeField] GameObject background_image1;
    [SerializeField] GameObject background_image2;
    [SerializeField] GameObject background_image3;
    [SerializeField] GameObject player;
    public bool isCutsceneActive;

    // Others
    Movement move;
    Rigidbody2D myRigidBody;

    private void Start()
    {
        move = player.GetComponent<Movement>();
        myRigidBody = player.GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Freeze Player & Disable Dash Movement
            move.canMove = false;
            myRigidBody.constraints = RigidbodyConstraints2D.FreezeAll;

            // Initiate the Tree Cutscene
            camAnim.SetBool("TreeExit", false);
            isCutsceneActive = true;
            camAnim.SetBool("TreeCutscene", true);

            // Scale the background so it fits with the large tree
            background_image1.transform.localScale = new Vector3(9, 9, 1);
            background_image2.transform.localScale = new Vector3(9, 9, 1);
            background_image3.transform.localScale = new Vector3(9, 9, 1);

            // Stop the cutscene after 3 seconds
            Invoke(nameof(StopCutscene), 3f);
        }
    }

    private void StopCutscene()
    {
        // Unfreeze the Player & Enable Dash Movement
        move.canMove = true;
        myRigidBody.constraints = RigidbodyConstraints2D.None; // Unfreezes
        myRigidBody.constraints = RigidbodyConstraints2D.FreezeRotation; // Re-freezes rotation b/c it got unchecked while frozen

        // Re-Scale the background so it fits back normally
        background_image1.transform.localScale = new Vector3(1, 1, 1);
        background_image2.transform.localScale = new Vector3(1, 1, 1);
        background_image3.transform.localScale = new Vector3(1, 1, 1);

        // Turn off cutscene camera, and go back to the regular one
        camAnim.SetBool("TreeExit", true);
        isCutsceneActive = false;

        //the Tree cutscene finishes
        camAnim.SetBool("TreeCutscene", false);

        // Remove Tree Cutscene GameObject, this should only be enabled once
        Destroy(gameObject);
    }
}
