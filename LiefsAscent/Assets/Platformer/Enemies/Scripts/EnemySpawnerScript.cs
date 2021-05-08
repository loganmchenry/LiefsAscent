using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerScript : MonoBehaviour
{
    // Serialized Fields
    //[SerializeField] GameObject wizardEnemy;
    //[SerializeField] float spawnRate = 2f;

    // State Variable
    float randX;
    //Vector2 whereToSpawn;
    float nextSpawn = 0f;

    // Others
    Vector3 v3Left;
    Vector3 v3Right;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        v3Left = new Vector3(-0.15f, .5f, 10);
        v3Left = Camera.main.ViewportToWorldPoint(v3Left);
        v3Right = new Vector3(Screen.width, 0, 0);
        v3Right = Camera.main.ScreenToViewportPoint(v3Right);
        v3Right = new Vector3(v3Right.x, .5f, 10);
        v3Right = Camera.main.ViewportToWorldPoint(v3Right);
        if (Time.time > nextSpawn)
        {
            nextSpawn += Time.time;
            //transform.position = new Vector3(Random.Range(v3Left, v3Right + 1));
        }
    }
}
