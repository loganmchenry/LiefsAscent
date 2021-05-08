using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/* 
    TO USE:
    When on the platform press "I" for Instructions.
    Press # Level (0, 1, 2, 3) you want to go to.
    When on Floor you can press "0" to return the platform to base.
*/ 


public class MovingPlatform : MonoBehaviour
{
    // Grabs all points of the different levels on the teee. 
    // The Platform Acts as an Elevator going up to the level
    // of the tree pressed.

    // To Trigger Instructions for Platform, press "I"

    // Serialized Fields
    [SerializeField] float speed;
    [SerializeField] bool onMovingPlatform;
    [SerializeField] Transform childTransform;
    [SerializeField] Transform transformB;
    [SerializeField] Transform transformC;
    [SerializeField] Transform transformD;
    [SerializeField] GameObject platformInstruction;
    [SerializeField] TextMeshProUGUI floatingText;

    // Private & State Variables
    private Vector3 level0;
    private Vector3 level1;
    private Vector3 level2;
    private Vector3 level3;
    private Vector3 nextPos;
    private bool userEnteredInput = false;
    private bool isDoneMove = true;
    private int levelPressed = 0;
    private int currentLevel = 0;
    //private bool firstTime = true;
    private bool instructionsOpen = false;

    private void Start()
    {
        level0 = childTransform.localPosition;
        level1 = transformB.localPosition;
        level2 = transformC.localPosition;
        level3 = transformD.localPosition;
    }

    private void Update()
    {
        if (onMovingPlatform)
        {
            // Check for user input only if instructions not open
            if (!instructionsOpen)
            {
                CheckUserInput();
            }

            // Close or Open Instructions
            if ((Input.GetKeyDown(ControlManager.instance.ActionControls["Interact"])) && isDoneMove)
            {
                if (ControlManager.instance.inputsActive) // Dont do anything if we are in the middle of changing the controls
                {
                    ShowInstructions();
                }
            }

            // Move the Platform
            if (userEnteredInput)
            {
                if (currentLevel != levelPressed)
                {
                    Move();
                }
                else
                {
                    isDoneMove = true;
                }
            }
        }
        else
        {
            callBackToBase();
        }
    }

    private void callBackToBase()
    {
        // Player can request elevator to go back to base (bottom)
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            levelPressed = 0;
            currentLevel = 1;
            nextPos = level0;
            userEnteredInput = true;
            isDoneMove = false;
        }

        // Move the Platform
        if (userEnteredInput)
        {
            if (currentLevel != levelPressed)
            {
                Move();
            }
            else
            {
                isDoneMove = true;
            }
        }
    }

    private void CheckUserInput()
    { 
        if (onMovingPlatform && isDoneMove)
        {
            // Grab User Key Press
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                levelPressed = 0;
                nextPos = level0;
                userEnteredInput = true;
                isDoneMove = false;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                levelPressed = 1;
                nextPos = level1;
                userEnteredInput = true;
                isDoneMove = false;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                levelPressed = 2;
                nextPos = level2;
                userEnteredInput = true;
                isDoneMove = false;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                levelPressed = 3;
                nextPos = level3;
                userEnteredInput = true;
                isDoneMove = false;
            }
        }
    }

    private void Move()
    {
        // Move the Platform
        childTransform.localPosition = Vector3.MoveTowards(childTransform.localPosition, nextPos, speed * Time.deltaTime);
        // Checks if the level has finished moving
        if (Vector3.Distance(childTransform.localPosition, nextPos) <= 0.1f)
        {
            ChangeDestination();
        }
    }

    private void ChangeDestination()
    {
        currentLevel = levelPressed;
        isDoneMove = true;
    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.gameObject.CompareTag("Player"))
    //    {
    //        onMovingPlatform = true;
    //        floatingText.gameObject.SetActive(true);
    //        collision.collider.transform.SetParent(childTransform);
    //    }
    //}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            onMovingPlatform = true;
            floatingText.gameObject.SetActive(true);
            collision.transform.SetParent(childTransform);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            onMovingPlatform = false;
            floatingText.gameObject.SetActive(false);
            collision.transform.SetParent(null);
        }
    }

    //private void OnCollisionExit2D(Collision2D collision)
    //{
    //    if (collision.gameObject.CompareTag("Player"))
    //    {
    //        onMovingPlatform = false;
    //        floatingText.gameObject.SetActive(false);
    //        collision.collider.transform.SetParent(null);
    //    }
    //}

    private void ShowInstructions()
    {
        if (!instructionsOpen)
        {
            platformInstruction.SetActive(true);
            instructionsOpen = true;
        }
        else
        {
            platformInstruction.SetActive(false);
            instructionsOpen = false;
        }
    }
}
