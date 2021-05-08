using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Counter : MonoBehaviour
{
    public int enemiesHit;

    public void ResetEnemies()
    {
        enemiesHit = 0;
    }
}
