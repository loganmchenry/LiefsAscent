using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturntoBase : MonoBehaviour
{

    public static bool loadAtBase = false;
    private int transitionTime = 1;
    // Start is called before the first frame update
    void Start()
    {
    }

    
    // Update is called once per frame
    void Update()
    {
        if (loadAtBase)
        {
            transform.position = new Vector3(773f, -12.25f, 0f);
            Invoke("SetLoadAtBaseToFalse", transitionTime);
        }
    }

    private void SetLoadAtBaseToFalse()
    {
        loadAtBase = false;
    }
}
