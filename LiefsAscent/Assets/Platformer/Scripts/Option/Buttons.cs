using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // For a Scene Management Script

public class Buttons : MonoBehaviour
{
    public void LoadStartScene()
    {
        SceneLoader.instance.LoadStart();
    }

    public void LoadBase()
    {
        SceneLoader.instance.LoadBase();
    }
}
