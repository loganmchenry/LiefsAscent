using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*  Implements player death
 *  - This script needs to be added to the player
 *  - And the player object needs to be dragged onto the player
 *  - Function: This script will make sure the player does not move when dying
 *      - Also restarts the scene using level manager script
 * 
 
 
 */



public class PlayerDeath : MonoBehaviour
{

    [SerializeField] GameObject player;
    Movement move;

    // Start is called before the first frame update
    void Start()
    {
        move = player.GetComponent<Movement>();
    }


    // make sure that if the player dies they cannot move
    // and restart the level
    public void Die()
    {
        move.canMove = false;
        FindObjectOfType<SoundManager>().Play("PlayerDeath");
        FindObjectOfType<LevelManager>().Restart();
    }

    public void DieProcedGen()
    {
        move.canMove = false;
        FindObjectOfType<SoundManager>().Play("PlayerDeath");
        FindObjectOfType<LevelManager>().RestartProcedGen();
    }
}
