using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// A temporary fix for removing enemies once they are 
// defeated in tactics.

public class EnemyManage : MonoBehaviour
{
    public static EnemyManage instance;

    // State Variables
    public bool enemy1Exist = true;
    public bool enemy2Exist = true;
    public bool enemy3Exist = true;

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


}
