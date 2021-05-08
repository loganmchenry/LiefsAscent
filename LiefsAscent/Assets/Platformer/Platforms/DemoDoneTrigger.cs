using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(BoxCollider2D))]
public class DemoDoneTrigger : MonoBehaviour
{

    [SerializeField]
    private GameObject PP;

    private void Reset()
    {
        GetComponent<BoxCollider2D>().isTrigger = true;
    }

    // when the user reaches the last platform before jumping off
    // indicates to playerStats that we are done and we can update
    // the playerPosition to the original start location
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            PP.GetComponent<PlayerPosition>().demoDone = true;
            Debug.Log("DEMO DONE TRIGGERED!");
            /*PP.GetComponent<PlayerPosition>().doneWithDemo = true;
            PlayerStats.instance.updatePlayerInfo();*/
        }
    }



}
