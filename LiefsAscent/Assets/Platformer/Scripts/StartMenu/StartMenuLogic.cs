using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuLogic: MonoBehaviour {
    private GameObject menu;
    public Animator music;
    private CanvasGroup controlsMenu;
    public float waitTime;
    public void Start() {

        menu = GameObject.Find("ControlsMenu");
        controlsMenu = menu.GetComponent < CanvasGroup > ();
    }

    IEnumerator TransitionScene(){
        music.SetTrigger("fadeOut");
        yield return new WaitForSeconds(waitTime);
    }

    public void LaunchGame() {
        StartCoroutine(TransitionScene());
        SceneLoader.instance.LoadStartingCutscene();
    }

    public void LaunchTutorial() {
        StartCoroutine(TransitionScene());
        SceneLoader.instance.LoadDemo();
    }

    public void OpenControls() {
        controlsMenu.alpha = controlsMenu.alpha > 0 ? 0 : 1;
        controlsMenu.blocksRaycasts = controlsMenu.blocksRaycasts == true ? false : true;
        controlsMenu.interactable = controlsMenu.interactable == true ? false : true;
        Time.timeScale = Time.timeScale > 0 ? 0 : 1; // Freeze Game
        ControlManager.instance.inputsActive = ControlManager.instance.inputsActive == true ? false : true;
    }

    public void Exit() {
        Application.Quit();
    }

}