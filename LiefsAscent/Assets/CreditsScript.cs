using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsScript : MonoBehaviour
{
    public void restartGame()
    {
        SceneLoader.instance.LoadStart();
    }
}
