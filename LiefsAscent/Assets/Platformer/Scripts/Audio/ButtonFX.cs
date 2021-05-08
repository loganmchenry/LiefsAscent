using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonFX : MonoBehaviour
{
    public AudioSource myFx;
    public AudioClip hover;
    public AudioClip click;

    public void Hover(){
        myFx.PlayOneShot(hover);
    }

    public void Click(){
        myFx.PlayOneShot(click);
    }
}
