using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/* 
    When using this script -> attach this script to the player character
    Then under "lives" change the size to whatever amount of lives you wish to use: we are using 4 right now
    then move in the images of the hearts/masks/etc that indicate the health
    We assume the 1st health starts from the right and then moves to the left:
        - Life4, Life3, Life2, Life1
    You also want to set Lives Remaining to whatever value you want, in Lief we use 4
    
    For debugging purposes we use Update so that we press enter to simulate losing lives
        - this can be commented out later... 
 
 */


public class PlayerLives : MonoBehaviour
{

    public Image[] lives;
    public int livesRemaining;

    private int ourLives = 4; // while livesRemaining is set in the editor, this is set in code so that we can reset them later


    // 4 Lives  and 4 images (0,1,2,3)
    // 3 lives and 3 visible (0,1,2,[3])
    // 2 lives and 2 visible (0,1,[2], [3])
    //etc


    public void LoseLife(float amount)
    {

        // If no life, do nothing, much like real life
        if (livesRemaining == 0)
        {
            return;
        }



        // decrease the value of livesRemaining
        livesRemaining -= (int)amount;
        // Hide one of the of the hearts
        lives[livesRemaining].enabled = false;
        // If we run out of lives reset
        if(livesRemaining == 0)
        {
            Debug.Log("You Lost");
            FindObjectOfType<PlayerDeath>().Die();
        }
    }

    /* 
        Gain's life based on the input amount
        to keep simple should only really increase once
        also this shouldbe called as a consume event on items
        need to drag the player object onto the consume event and select this funciton
    */
    public void GainLife(float amount){
        
        // if max health, don't do anything
        if(livesRemaining == ourLives){
            Debug.Log("max health dude wyd?");
            return;
        }
        Debug.Log(livesRemaining);
        // increase the value of livesRemaining
        livesRemaining += (int)amount;

        // un-hide one of the hearts, minus 1 because its a 0-3 you dummy
        lives[livesRemaining-1].enabled = true;




    }



    // Just for debugging, when user presses enter, loses a life
    private void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.Return))
        {
            LoseLife();
        }*/
    }

    public void enableLives(){
        for(int i = 0; i < ourLives; i++){
            lives[i].enabled = true;
        }
        livesRemaining = ourLives;
    }

}
