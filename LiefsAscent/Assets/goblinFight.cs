using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class goblinFight : MonoBehaviour
{
    private bool isGoblin;
    public bool passedTut = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    if(transform.position.x == -6 && !passedTut || PlayerStats.instance.isGoblin == true)
        {
            passedTut = true;
        }
    }
}
