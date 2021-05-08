using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiHider : MonoBehaviour
{

    [SerializeField] private GameObject theUiWindow;
    private bool isActive;


    private void Update()
    {
        CheckForActive();
        CheckForHide();
    }

    private void CheckForActive()
    {
        if (theUiWindow.activeSelf)
        {
            Debug.Log("Is active");
            isActive = true;
        }
        else
        {
            Debug.Log("Aint active chief");
            isActive = false;
        }
    }

    private void CheckForHide()
    {
        if (isActive)
        {
            // waits for two seconds then hides the UI
            StartCoroutine(WaitForTwo());
            
        }
    }

    IEnumerator WaitForTwo()
    {
        print("starting to wait...");

        yield return new WaitForSecondsRealtime(3);
        theUiWindow.SetActive(false);

        print("3 seconds have passed");
    }



}
