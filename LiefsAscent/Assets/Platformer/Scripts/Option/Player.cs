using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class that handles player information (save, load, retrieval)

public class Player : MonoBehaviour
{
    Transform playerTransform;

    //Grabs "Player" Object from Scene that is running.
    //Change to Adventurer When Applying to Demo.

    public void getPlayer()
    {   
        playerTransform = GameObject.Find("Player").GetComponent<Transform>();
    }

    //Saves Players location (atm)


    public void SavePlayer()
    {
        getPlayer();
        PlayerData playerData = new PlayerData(playerTransform.position);
        SaveGame.SavePlayer(playerData);
    }

    //Loads Players location (atm)

    public void LoadPlayer()
    {
        getPlayer();
        PlayerData data = SaveGame.LoadPlayer();
        
        Vector3 position;
        
        position.x = data.position[0];
        position.y = data.position[1];
        position.z = data.position[2];
        
        playerTransform.position = position;
    }
}
