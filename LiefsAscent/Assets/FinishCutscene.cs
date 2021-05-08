using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishCutscene : MonoBehaviour
{
    public void OnEnable()
    {
        // Load Base the second the cutscene is over
        SceneLoader.instance.LoadBase();
    }
}
