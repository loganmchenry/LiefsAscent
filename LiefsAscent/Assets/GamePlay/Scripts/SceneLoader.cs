using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // For a Scene Management Script

// Note: Before Using the SceneLoader to Transition Scenes, the 
// Build Index of each of the Scenes must be set in the Unity
// Build Settings. This Ensures that We Do Not Rely on The
// Physical Name of the Scene in order to load an transition,
// But The Actual Order of GamePlay.

public class SceneLoader : MonoBehaviour
{
    // Serialized Params
    public static SceneLoader instance;
    [SerializeField] float transitionTime = 1.3f;
    [SerializeField] GameObject fade;
    [SerializeField] int currentSceneIndex;
    [SerializeField] GameObject player;
    

    Scene CurrentScene;
    GameObject gameObjectVal;
    public List<GameObject> SceneObjects;
    public List<String> doNotAdd;

    // Others
    public Animator transition;

    public void LoadStart()
    {
        StartCoroutine(LoadNextSceneAsync("StartMenu"));
    }

    //Load Demo Level

    public void LoadDemo()
    {
        StartCoroutine(LoadNextSceneAsync("Demo"));
    }

    // Load Initial Cutscene
    public void LoadStartingCutscene()
    {
        StartCoroutine(LoadNextSceneAsync("99_Cutscene"));
    }

    //Load Base Level

    public void LoadBase()
    {
        PlayerStats.instance.inNifl = false;
        PlayerStats.instance.inCave = false;
        PlayerStats.instance.inValh = false;
        StartCoroutine(LoadNextSceneAsync("00_Base"));
    }

    //Load Base Level with player at the elevator 
    public void LoadAtTree()
    {
        PlayerStats.instance.inNifl = false;
        PlayerStats.instance.inCave = false;
        PlayerStats.instance.inValh = false;
        PlayerStats.instance.items = new List<GameObject>();
        StartCoroutine(LoadNextSceneAsync("00_Base"));
        ReturntoBase.loadAtBase = true;
    }


    //Load Niflheim Level

    public void LoadNiflheim()
    {
        PlayerStats.instance.inNifl = true;
        PlayerStats.instance.items = new List<GameObject>();
        StartCoroutine(LoadNextSceneAsync("01_Niflheim"));
    }

    //Load Procedural Level

    public void LoadProceduralGenTest()
    {
        PlayerStats.instance.inCave = true;
        PlayerStats.instance.items = new List<GameObject>();
        StartCoroutine(LoadNextSceneAsync("02_ProceduralGenTest"));
    }

    public void LoadValhalla()
    {
        PlayerStats.instance.inValh = true;
        PlayerStats.instance.items = new List<GameObject>();
        StartCoroutine(LoadNextSceneAsync("03_Valhalla"));
    }

    //Load Scene as By Name

    public void LoadNextScene(string loadScene)                     //Non-iterative Loading of Scene
    {
        StartCoroutine(LoadNextSceneAsync(loadScene));
    }

    //Load Options Menu Additively

    public void LoadOptions()
    {
        Time.timeScale = 0;
        Debug.Log("Test");
        SceneManager.LoadScene("Options Menu", LoadSceneMode.Additive);
    }

    //Unload Options Menu 

    public void UnLoadOptions()
    {
        SceneManager.UnloadSceneAsync("Options Menu");
        Time.timeScale = 1;
    }

    //Load Tactics Scene Additively 

    public void LoadTacticalScene(GameObject toBeRemoved)
    {
        //StartCoroutine(LoadSceneAdditively("tacticalDemo"));
        gameObjectVal = toBeRemoved;
        StartCoroutine(LoadSceneAdditively("tacticalDemo"));
    }

    //Unload Tactics Scenes
    public void UnloadTacticalScene()
    {
        StartCoroutine(UnLoadSceneAdditively("tacticalDemo"));
    }

    //This is used to load Scenes additvely so we do not have to store information between platformers and tactics.

    IEnumerator LoadSceneAdditively(string transitionLevel)
    {
        // Play Scene Transition Animation
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        // Load Scene in the Background while the current Scene runs.
        // This is to ensure smooth transitions between scenes.

        AsyncOperation asyncLoadScene = SceneManager.LoadSceneAsync(transitionLevel, LoadSceneMode.Additive);

        SetSceneInactive();
        RemoveGameObject(gameObjectVal);

        // Wait until the scene fully loads
        while (!asyncLoadScene.isDone)
        {
            yield return null;
        }


        //Disable all game objects in current scene so that it does not interfere with tactics.


        fade.SetActive(false);  //Temporary Fix to solve animation issues with loading Proced Gen Scene
        fade.SetActive(true);
    }

    //This is used to unload additive scenes.
    //This is only done for the tactics scene.

    IEnumerator UnLoadSceneAdditively(string transitionLevel)
    {
        // Play Scene Transition Animation
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        // Load Scene in the Background while the current Scene runs.
        // This is to ensure smooth transitions between scenes.

        AsyncOperation asyncLoadScene = SceneManager.UnloadSceneAsync(transitionLevel);
        // Wait until the scene fully loads
        while (!asyncLoadScene.isDone)
        {
            yield return null;
        }

        //Enable all game objects in current level after tactics level finishes.
        SetSceneActive();

        ReEnableDisruptedLoops();
        PartyMembersInstance.instance.updatePartyMemberUIStats();

        fade.SetActive(false);  //Temporary Fix to solve animation issues with loading Proced Gen Scene
        fade.SetActive(true);
    }

    //Used to load different levels that are not additive (all levels other than tactics and options)

    IEnumerator LoadNextSceneAsync(string transitionLevel)
    {
        // Play Scene Transition Animation
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        // Load Scene in the Background while the current Scene runs.
        // This is to ensure smooth transitions between scenes.
        AsyncOperation asyncLoadScene = SceneManager.LoadSceneAsync(transitionLevel);

        // Wait until the scene fully loads
        while (!asyncLoadScene.isDone)
        {
            yield return null;
        }

        CurrentScene = SceneManager.GetSceneByName(transitionLevel);
        GrabSceneObjects();


        if (transitionLevel == "00_Base")
        {
            TeleporterBlocker.instance.SetBlockers();
            Teleporter.unlocked = false; // for the teleporters OUT of the levels
        }

        fade.SetActive(false);  //Temporary Fix to solve animation issues with loading Proced Gen Scene
        fade.SetActive(true);
    }

    //When initially loading a level (that is not tactics), call this method to store all managed gameobjects for loading/unloading the tactics scene.

    public void GrabSceneObjects()
    {
        SceneObjects.Clear();
        doNotAdd.Clear();
        doNotAdd.Add("Rain");
        doNotAdd.Add("Level_Manager");
        doNotAdd.Add("Game_Manager");
        doNotAdd.Add("Teleporter 1 Block");
        doNotAdd.Add("Teleporter 2 Block");
        doNotAdd.Add("Teleporter 3 Block");
        foreach (GameObject g in SceneManager.GetSceneByName(CurrentScene.name).GetRootGameObjects())
        {
            if(!doNotAdd.Contains(g.name))
            {
                SceneObjects.Add(g);
            }    
        }
    }

    //Goes through list of Managed GameObjects for a scene and disables them one by one to transition to tactics scene.

    public void SetSceneInactive()
    {
        if (SceneObjects.Count == 0)
        {
            CurrentScene = SceneManager.GetActiveScene();
            GrabSceneObjects();
            if (CurrentScene.name == "00_Base")
                TeleporterBlocker.instance.SetBlockers();
        }

        foreach (GameObject g in SceneObjects)
        {
            if(g != null)
                g.SetActive(false);
        }
    }


    //Goes through list of Managed GameObjects for a scene and re-enables them one by one to transition back to the level.

    public void SetSceneActive()
    {
        foreach (GameObject g in SceneObjects)
        {
            if(g != null)
                g.SetActive(true);
        }
    }

    //Removes a GameObject from the managed list so it won't be setactive after the scene is reloaded.

    public void RemoveGameObject(GameObject toBeRemoved)
    {
        if (SceneObjects.Contains(toBeRemoved))
            SceneObjects.Remove(toBeRemoved);
    }

    private void ReEnableDisruptedLoops()
    {
        if (GameObject.Find("Counter") != null)
        {
            FindObjectOfType<Counter>().ResetEnemies();
        }

        // Restarts Raven Instantiation
        if (GameObject.Find("RavenController") != null)
        {
            GameObject.Find("RavenController").GetComponent<RavenInstantiate>().restartGen();
        }
    }

    // This works for PC standalone builds.
    // There will be no affect when testing 
    // in the Unity Editor. 
    public void QuitGame()
    {
        Application.Quit();
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
