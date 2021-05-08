using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Player Dialog Is Activated With "T" for "Talk"

public class DialogManager : MonoBehaviour
{
    // Serialized Fields
    [SerializeField] GameObject dialogBox;
    [SerializeField] GameObject player;
    [SerializeField] TextMeshProUGUI dialogText;
    [SerializeField] int lettersPerSecond;
    [SerializeField] Dialog dialog;

    // Serialized Fields just for Party Members
    [SerializeField] bool partyMember = false;
    [SerializeField] string partyName = "";

    // Serialized Fields just for exit teleporters
    [SerializeField] bool exitPortal = false;

    // Pop-Up when Player in Range
    [SerializeField] GameObject popUp;
    [SerializeField] Image imageForPopup;
    [SerializeField] TextMeshProUGUI textforPopup;
    [SerializeField] Sprite theImage; // Sprites should be around 1024 x 512

    // State Variables
    bool playerInRange;  // is the player within range of an object so the dialog can be triggered
    bool isTyping;       // if a line of dialogue is being printed out, do not move on to the next line
    int currentLine = 0;

    // Others - This is needed to essentially "Freeze Game" while the dialog is in play
    Movement move;
    Rigidbody2D myRigidBody;
    GameObject[] enemyObjects; // Will store every enemy

    // Start is called before the first frame update
    void Start()
    {
        move = player.GetComponent<Movement>();
        myRigidBody = player.GetComponent<Rigidbody2D>();
        enemyObjects = GameObject.FindGameObjectsWithTag("Enemy");

        // Set Up Pop-Up Canvas
        imageForPopup.sprite = theImage;
        string getAction = (ControlManager.instance.ActionControls["Interact"]).ToString();
        getAction = getAction.Replace("Arrow", "");
        textforPopup.text = "Press '" + getAction + "' to Interact";
    }

    // Update is called once per frame
    void Update()
    {
        if (!dialogBox.activeInHierarchy)
        {
            if ((Input.GetKeyDown(ControlManager.instance.ActionControls["Interact"])) && playerInRange)
            {
                // Set the Dialog Box to Active, and in addition
                // disable all movement. A player should not be 
                // able to move when the dialog box is active.

                
                if (ControlManager.instance.inputsActive) // Dont do anything if we are in the middle of changing the controls
                {
                    // Close Pop Up
                    popUp.GetComponent<CanvasGroup>().alpha = 0;
                    popUp.GetComponent<CanvasGroup>().blocksRaycasts = false;
                    popUp.GetComponent<CanvasGroup>().interactable = false;

                    if (!Teleporter.unlocked) // added to get rid of dialog once the player has enough keys
                    {
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
                        StartCoroutine(TypeDialog(dialog.Lines[0]));
                    }
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
        if ((Input.GetKeyDown(ControlManager.instance.ActionControls["Interact"])) && playerInRange && !isTyping)
        {
            currentLine++;
            if (currentLine < dialog.Lines.Count)
            {
                StartCoroutine(TypeDialog(dialog.Lines[currentLine]));
            }
            else
            {
                // Once there is no more lines to be displayed
                // exit the dialog.
                dialogBox.SetActive(false);
                checkIfCharacterMetUpdate();
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

    private void checkIfCharacterMetUpdate()
    {
        // If the characte is a party member & you just met them in game
        // now make them available in the partyMemberUI
        if (partyMember)
        {
            PartyMembersInstance.instance.metInGame(partyName);
            partyMember = false;
        }
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

    // When the player is within range of a NPC character
    // whom they can interact with they are able to 
    // trigger dialogue
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;

            // Open Pop Up
            imageForPopup.sprite = theImage;
            UpdateTextPopUp(); // Just check if text needs to be updated (aka controls were changed)
            popUp.GetComponent<CanvasGroup>().alpha = 1;
            popUp.GetComponent<CanvasGroup>().blocksRaycasts = true;
            popUp.GetComponent<CanvasGroup>().interactable = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;

            // Close Pop Up
            popUp.GetComponent<CanvasGroup>().alpha = 0;
            popUp.GetComponent<CanvasGroup>().blocksRaycasts = false;
            popUp.GetComponent<CanvasGroup>().interactable = false;
        }
    }

    private void UpdateTextPopUp()
    {
        string getAction = (ControlManager.instance.ActionControls["Interact"]).ToString();
        getAction = getAction.Replace("Arrow", "");
        textforPopup.text = "Press '" + getAction + "' to Interact";
    }
}
