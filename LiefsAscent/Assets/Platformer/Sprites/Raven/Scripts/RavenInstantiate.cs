using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RavenInstantiate : MonoBehaviour
{
    // Public Fields
    public GameObject ravenPrefab;
    public float respawnTime = 2.0f;
    public bool ravenCanGenerate = true;
    [SerializeField] GameObject player;

    // For Dragon Instantiate
    public GameObject dragonPrefabRed;
    public float respawnTimeDragon = 60.0f;
    public bool dragonCanGenerate = true;

    // Private Fields
    private Vector2 screenBounds;

    private void spawnRaven()
    {
        // Instantiate Raven GameObject
        GameObject raven = Instantiate(ravenPrefab);
        raven.transform.SetParent(gameObject.transform);

        // This defines the screen boundaries on an x & y axis
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        // Generate the raven at random y value and x value way off screen to the left
        raven.transform.position = new Vector2(Random.Range(screenBounds.x - 150f, screenBounds.x - 90f), Random.Range(screenBounds.y-14f, screenBounds.y));
    }

    private void spawnDragon()
    {
        // Instantiate Raven GameObject
        GameObject dragon = Instantiate(dragonPrefabRed);
        dragon.transform.SetParent(gameObject.transform);

        // This defines the screen boundaries on an x & y axis
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        // Generate the dragon at random y value and x value way off screen to the left
        dragon.transform.position = new Vector2(Random.Range(screenBounds.x - 150f, screenBounds.x - 90f), Random.Range(screenBounds.y - 14f, screenBounds.y));
    }

    void Start()
    {
        // This defines the screen boundaries on an x & y axis
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        StartCoroutine(ravenGeneration());
        StartCoroutine(dragonGeneration());
    }

    IEnumerator ravenGeneration()
    {
        while (ravenCanGenerate)
        {
            // Generate 1 raven every respawnTime seconds
            yield return new WaitForSeconds(respawnTime);
            spawnRaven();
        }
    }

    IEnumerator dragonGeneration()
    {
        while (dragonCanGenerate)
        {
            // Generate 1 raven every respawnTime seconds
            yield return new WaitForSeconds(respawnTimeDragon);
            spawnDragon();
        }
    }

    public void restartGen()
    {
        StartCoroutine(ravenGeneration());
        StartCoroutine(dragonGeneration());
    }
}
