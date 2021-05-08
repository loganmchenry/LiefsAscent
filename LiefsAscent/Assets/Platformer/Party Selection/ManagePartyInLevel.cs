using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagePartyInLevel : MonoBehaviour
{
    public int levelsCompleted;
    public GameObject level1;
    public GameObject level2;
    public GameObject level3;

    public GameObject credits;

    // Every time the Base scene loads, set the players that are unlocked
    // Try #55
    public void Start()
    {
        // Start -> Level 0
        level1 = GameObject.FindGameObjectWithTag("Party_Level_1");
        level2 = GameObject.FindGameObjectWithTag("Party_Level_2");
        level3 = GameObject.FindGameObjectWithTag("Party_Level_3");
        Transform[] c_level1 = level1.GetComponentsInChildren<Transform>();
        Transform[] c_level2 = level2.GetComponentsInChildren<Transform>();
        Transform[] c_level3 = level3.GetComponentsInChildren<Transform>();

        levelsCompleted = PlayerStats.instance.levelsCompleted;

        switch (levelsCompleted)
        {
            case 0:
                foreach (Transform party in c_level1)
                {
                    party.gameObject.SetActive(true);
                }
                foreach (Transform party in c_level2)
                {
                    party.gameObject.SetActive(false);
                }
                foreach (Transform party in c_level3)
                {
                    party.gameObject.SetActive(false);
                }
                break;
            case 1:
                foreach (Transform party in c_level1)
                {
                    party.gameObject.SetActive(false);
                }
                foreach (Transform party in c_level2)
                {
                    party.gameObject.SetActive(true);
                }
                foreach (Transform party in c_level3)
                {
                    party.gameObject.SetActive(false);
                }
                break;
            case 2:
                foreach (Transform party in c_level1)
                {
                    party.gameObject.SetActive(false);
                }
                foreach (Transform party in c_level2)
                {
                    party.gameObject.SetActive(false);
                }
                foreach (Transform party in c_level3)
                {
                    party.gameObject.SetActive(true);
                }
                break;
            case 3:
                credits.SetActive(true);
                break;
        }
    }
}
