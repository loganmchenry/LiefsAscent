using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemsAndSignsDialogManager : MonoBehaviour
{
    // Serialized Fields
    [SerializeField] GameObject dialogBox;
    [SerializeField] GameObject player;
    [SerializeField] TextMeshProUGUI dialogText;
    [SerializeField] Dialog dialog;

    // State Variables
    bool playerInRange;  // is the player within range of an object so the dialog can be triggered
    int currentLine = 0;

    // Others
    InteractionSystem interact;
    Movement move;
    Rigidbody2D myRigidBody;
    GameObject[] enemyObjects; // Will store every enemy

    private void Start()
    {
        interact = FindObjectOfType<InteractionSystem>();
        move = player.GetComponent<Movement>();
        myRigidBody = player.GetComponent<Rigidbody2D>();
        enemyObjects = GameObject.FindGameObjectsWithTag("Enemy");
    }

    // Update is called once per frame
    void Update()
    {
        if(!dialogBox.activeInHierarchy)
        {
            if ((Input.GetKeyDown(ControlManager.instance.ActionControls["PickUp"])) && playerInRange)
            {
                if (ControlManager.instance.inputsActive) // Dont do anything if we are in the middle of changing the controls
                {
                    // Set the Dialog Box to Active, and in addition
                    // disable all movement. A player should not be 
                    // able to move when the dialog box is active.
                    dialogBox.SetActive(true);
                    move.canMove = false;
                    // This is necessary to stop movement from the DashMove script
                    myRigidBody.constraints = RigidbodyConstraints2D.FreezeAll;
                    foreach (GameObject enemy in enemyObjects)
                    {
                        if (enemy != null)
                        {
                            enemy.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll; // Freezes all the enemies
                        }
                    }
                    DisplayLine(dialog.Lines[0]);
                    interact.ExamineItem(this);
                }
            }
        }
        else if (dialogBox.activeInHierarchy && playerInRange)
        {
            // When there are multiple lines to be displayed
            // keep going.
            UpdateLine();
        }
        
    }

    private void UpdateLine()
    {
        // This function is for when there are multiple
        // lines of text to be displayed. It will display
        // one line at a time.
        if ((Input.GetKeyDown(ControlManager.instance.ActionControls["PickUp"])) && playerInRange)
        {
            currentLine++;
            if (currentLine < dialog.Lines.Count)
            {
                DisplayLine(dialog.Lines[currentLine]);
            }
            else
            {
                // Once there is no more lines to be displayed
                // exit the dialog.
                interact.ExamineItem(this);
                dialogBox.SetActive(false);
                currentLine = 0;
                move.canMove = true;
                myRigidBody.constraints = RigidbodyConstraints2D.None; // Unfreezes
                myRigidBody.constraints = RigidbodyConstraints2D.FreezeRotation; // Re-freezes rotation b/c it got unchecked while frozen
                foreach (GameObject enemy in enemyObjects)
                {
                    if (enemy != null)
                    {
                        enemy.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None; // Unfreezes
                        enemy.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
                    }
                }
            }
        }
    }

    private void DisplayLine(string line)
    {
        dialogText.text = line;
    }

    // When the player is within range of a examinable object
    // or a sign then the player is within range and can 
    // activate the dialog box.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
           playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
