using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterBlocker : MonoBehaviour
{
    public static TeleporterBlocker instance;

    public void SetBlockers()
    {
        GameObject hold = GameObject.Find("GameStats");
        PlayerStats temp = hold.GetComponent<PlayerStats>();
        GameObject blocker;
        switch (temp.levelsCompleted)
        {
            case 0:
                blocker = GameObject.Find("Teleporter 1 Block");
                blocker.SetActive(false);
                break;
            case 1:
                blocker = GameObject.Find("Teleporter 2 Block");
                blocker.SetActive(false);
                break;
            case 2:
                blocker = GameObject.Find("Teleporter 3 Block");
                blocker.SetActive(false);
                break;
        }
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
}
