using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OptionsMenuOpen : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        //Hit Escape Key from Gameplay To Pause Game and Go to Options Menu

        if (Input.GetKeyDown(KeyCode.Escape) && !SceneManager.GetSceneByName("Options Menu").isLoaded)
        {
            Time.timeScale = 0;
            Debug.Log("test");
            SceneLoader.instance.LoadOptions();
        }
    }
}
