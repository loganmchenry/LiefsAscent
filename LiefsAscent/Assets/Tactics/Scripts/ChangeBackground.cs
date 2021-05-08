using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeBackground : MonoBehaviour
{
    [SerializeField]
    private List<Image> backgroundImages;

    [SerializeField]
    private Image testImage;
    void Awake()
    {
        if (PlayerStats.instance.inValh)
            backgroundImages[3].gameObject.SetActive(true);
        else if (PlayerStats.instance.inNifl)
            backgroundImages[2].gameObject.SetActive(true);
        else if (PlayerStats.instance.inCave)
            backgroundImages[1].gameObject.SetActive(true);
        else
        {
            backgroundImages[0].gameObject.SetActive(true);
        }
    }

}
