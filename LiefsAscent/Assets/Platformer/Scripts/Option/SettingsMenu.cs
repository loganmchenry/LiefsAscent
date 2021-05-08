using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

//Class that handles UI functionality of the Options/Settings Menu.

public class SettingsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;               //Temporary Audio Mixer to Test out Volume Slider.
    public Button ResumeButton;
    public Button ExitGameButton;
    public GameObject mainMenu;

    public Vector2 pos;


    //Initialize Resume/Exit Game Buttons at Options Load
    //Disable Settings Menu

    void Start()                                
    {
        Button ExitBtn = ExitGameButton.GetComponent<Button>();
        ExitBtn.onClick.AddListener(ExitGameClick);

        Button ResumeBtn = ResumeButton.GetComponent<Button>();
        ResumeBtn.onClick.AddListener(ResumeGameClick); 
    }


    public void ResumeGameClick()
    {
        SceneManager.UnloadSceneAsync("Options Menu");
        Time.timeScale = 1;
    }


    public void ControlClick()
    { 
        ControlManager Controls = GameObject.FindObjectOfType<ControlManager>();
        Controls.OpenControls();
    }

    //If back button from settings is clicked, disable settings menu and enable main menu

    public void backGameClick()
    {
        mainMenu.SetActive(true);
    }

    public void ExitGameClick()
    {
        Application.Quit();
    }

    /*
    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("volume", volume);
    }

    public void SetFullScreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }*/
}

