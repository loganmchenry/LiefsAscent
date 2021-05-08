using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // For a Scene Management Script

public class Teleporter : MonoBehaviour
{
    // Serialized Fields
    [SerializeField] float transitionTime = 1.3f;
    [SerializeField] string transitionLevel;
    public static bool unlocked;
    [SerializeField] GameObject fade;
    [SerializeField] GameObject boss;

    public static Teleporter instance;
    // Others
    public Animator transition;

    private void OnTriggerStay2D(Collider2D collision)
    {


        if (Input.GetKey(ControlManager.instance.ActionControls["Interact"]))
        {
            if (FindObjectOfType<PlayerPosition>() != null)
            {
                FindObjectOfType<PlayerPosition>().savePos();
            }

            TeleporterAvailable();
        }

    }


    public void TeleporterAvailable()
    {
        GameObject hold = GameObject.Find("GameStats");
        PlayerStats temp = hold.GetComponent<PlayerStats>();

        switch (transitionLevel)
        {
            case "01_Niflheim":
                if (temp.levelsCompleted == 0)
                    SceneLoader.instance.LoadNiflheim();
                break;
            case "02_ProceduralGenTest":
                if (temp.levelsCompleted == 1)
                    SceneLoader.instance.LoadProceduralGenTest();
                break;
            case "03_Valhalla":
                if (temp.levelsCompleted == 2)
                    SceneLoader.instance.LoadValhalla();
                break;
            case "Boss_Fight":
                GoToBoss();
                break;
        }
    }

    private void GoToBoss()
    {
        switch (name)
        {
            case "NiflheimExit":
                if (RewardsManager.bossFinished)
                {
                    GameObject hold = GameObject.Find("GameStats");
                    PlayerStats temp = hold.GetComponent<PlayerStats>();
                    temp.levelsCompleted += 1;

                    SceneLoader.instance.LoadAtTree();
                    RewardsManager.bossFinished = false;
                    
                    break;
                } else if (RewardsManager.keysCollected >= 1)
                {
                    unlocked = true;
                    PlayerStats.instance.isLoki = true;
                    RewardsManager.keysCollected--;
                    //instantiate boss enemy
                    boss.transform.position = transform.position + new Vector3Int(5, 5, 0);
                    //do a cutscene with boss where the boss triggers tactical mode
                    BossCutscene.instance.StartCutscene();
                    RewardsManager.enterBossFight = true;
                }
                break;
            case "CaveExit":
                if (RewardsManager.bossFinished)
                {
                    GameObject hold = GameObject.Find("GameStats");
                    PlayerStats temp = hold.GetComponent<PlayerStats>();
                    temp.levelsCompleted += 1;

                    SceneLoader.instance.LoadAtTree();
                    RewardsManager.bossFinished = false;
                    
                    break;
                } else if (RewardsManager.keysCollected >= 2)
                {
                    unlocked = true;
                    PlayerStats.instance.isLoki = true;
                    RewardsManager.keysCollected -= 2;
                    //instantiate boss enemy
                    boss.transform.position = transform.position + new Vector3Int(5, 5, 0);
                    //do a cutscene with boss
                    BossCutscene.instance.StartCutscene();
                    RewardsManager.enterBossFight = true;
                }
                break;
            case "ValhallaExit":
                if (RewardsManager.bossFinished)
                {
                    //Special cutscene for the end of the game // roll credits?

                    
                    GameObject hold = GameObject.Find("GameStats");
                    PlayerStats temp = hold.GetComponent<PlayerStats>();
                    temp.levelsCompleted += 1;
                    // Call to go back to base
                    // there is functionality that gets called to roll the credits
                    SceneLoader.instance.LoadAtTree();
                    RewardsManager.bossFinished = false;
                    
                    break;
                } else if (RewardsManager.keysCollected >= 3)
                {
                    unlocked = true;
                    PlayerStats.instance.isLoki = true;
                    RewardsManager.keysCollected -= 3;
                    //instantiate boss
                    boss.transform.position = transform.position + new Vector3Int(-5, 3, 0);
                    //do a cutscene with boss
                    BossCutscene.instance.StartCutscene();
                    RewardsManager.enterBossFight = true;
                }
                break;
        }
    }
}
