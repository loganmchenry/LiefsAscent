using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossCutscene : MonoBehaviour
{
    // Serialized Variables
    [SerializeField] GameObject player;
    public static bool cutsceneDone;
    public static BossCutscene instance;

    // Serialized Fields
    [SerializeField] GameObject dialogBox;
    [SerializeField] TextMeshProUGUI dialogText;
    [SerializeField] int lettersPerSecond;
    [SerializeField] Dialog dialog;

    // State Variables
    bool isTyping;       // if a line of dialogue is being printed out, do not move on to the next line
    int currentLine = 0;

    // Others
    Movement move;
    Rigidbody2D myRigidBody;
    //This is needed to essentially "Freeze Game" while the dialog is in play
    GameObject[] enemyObjects; // Will store every enemy

    private void Start()
    {
        move = player.GetComponent<Movement>();
        myRigidBody = player.GetComponent<Rigidbody2D>();

       
    }

    

    void Awake()
    {
        // Singleton
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

    }



    public void StartCutscene()
    {
        if (!cutsceneDone)
        {
            cutsceneDone = true;

            // Freeze Player & Disable Dash Movement
            move.canMove = false;
            myRigidBody.constraints = RigidbodyConstraints2D.FreezeAll;


            if (!dialogBox.activeInHierarchy)
            {

                // Set the Dialog Box to Active, and in addition
                // disable all movement. A player should not be 
                // able to move when the dialog box is active.


                if (ControlManager.instance.inputsActive) // Dont do anything if we are in the middle of changing the controls
                {

                    dialogBox.SetActive(true);
                    move.canMove = false;
                    // This is necessary to stop movement from the DashMove script
                    myRigidBody.constraints = RigidbodyConstraints2D.FreezeAll;
                    /*
                    foreach (GameObject enemy in enemyObjects)
                    {
                        if (enemy != null)
                        {
                            enemy.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll; // Freezes all the enemies
                        }
                    }
                    */
                    TypeDialog(dialog.Lines[0]);
                }


            }
            



            // Stop the cutscene after 5 seconds
            Invoke(nameof(StopCutscene), 5f);
        }
        
    }

    private void StopCutscene()
    {
        // Unfreeze the Player & Enable Dash Movement
        move.canMove = true;
        myRigidBody.constraints = RigidbodyConstraints2D.None; // Unfreezes
        myRigidBody.constraints = RigidbodyConstraints2D.FreezeRotation; // Re-freezes rotation b/c it got unchecked while frozen
        dialogBox.SetActive(false);
        //reset for next level
        cutsceneDone = false;

        // Remove Cutscene GameObject, this should only be enabled once
        Destroy(gameObject);

        SceneLoader.instance.LoadTacticalScene(gameObject);
    }

  

    // This co-routine displays the lines with a typing effect
    // displaying letters at the speed set by the user
    private IEnumerator TypeDialog(string line)
    {
        isTyping = true;
        dialogText.text = "";
        foreach (var letter in line.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }
        isTyping = false;
    }

}


