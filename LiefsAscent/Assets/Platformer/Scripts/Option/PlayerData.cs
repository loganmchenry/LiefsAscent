using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

//Class that holds the information regarding the player.
//Need to include other information later.


public class PlayerData
{
    //public int level;                 //What level Player is on
    public float[] position;            //Coordinates of Player

    public PlayerData(Vector3 playerPosition)
    {
        position = new float[3];
        position[0] = playerPosition.x;
        position[1] = playerPosition.y;
        position[2] = playerPosition.z;
    }
}
