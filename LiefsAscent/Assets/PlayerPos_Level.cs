using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPos_Level : MonoBehaviour
{
    public float xPos;
    public float yPos;
    public float zPos;
    Vector3 position;

    public void retrievePlayerInfoLevel()
    {
        Transform playerTransform = GameObject.Find("Player").GetComponent<Transform>();
        position.x = xPos;
        position.y = yPos;
        position.z = zPos;

        playerTransform.position = position;
    }
}
