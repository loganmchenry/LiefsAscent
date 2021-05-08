using System.Collections;
using System.Collections.Generic;
using UnityEngine;


 
/*  This script gets added to an object (spikes, etc.) that you wish to damage/kill the player
 *      - automatically adds boxCollider2D
 *      - Detects when the player interacts with this object and outputs to debugger
 
 */

[RequireComponent(typeof(BoxCollider2D))]
public class TrapObject : MonoBehaviour
{
    private void Reset()
    {
        // make sure the box collider has isTrigger set to true
        GetComponent<BoxCollider2D>().isTrigger = true;
    }


    // So when colliding with the player character will it trigger losing lives from the playerLives Script
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {

            Debug.Log($"{name} Triggered");
            FindObjectOfType<PlayerLives>().LoseLife(1);
        }
    }


}
