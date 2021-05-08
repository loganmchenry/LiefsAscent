using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDontDestroy : MonoBehaviour
{
    public static ItemDontDestroy instance;
    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
