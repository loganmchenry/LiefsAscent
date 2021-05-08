using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KeyUI : MonoBehaviour
{

    public Image key;
    //public Image[] keyDisplayNumber = new Image[10];
    [SerializeField] TextMeshProUGUI numberOfKeys;


    private void Start()
    {
        key.enabled = true;
    }

    private void Update()
    {
        numberOfKeys.text = " " + RewardsManager.keysCollected;
    }







}


