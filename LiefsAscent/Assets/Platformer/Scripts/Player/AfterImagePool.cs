using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterImagePool : MonoBehaviour
{
    [SerializeField]
    //reference of the prefab for the afterImage
    private GameObject afterImagePrefab;

    // stores all the objects that have been made but are not currently active
    private Queue<GameObject> availableObjects = new Queue<GameObject>();

    // singleton to access this script from other scripts
    public static AfterImagePool Instance { get; private set; }


    private void Awake()
    {
        // sets the reference
        Instance = this;
        // we want to have a pool at the start of our game
        GrowPool();
    }

    // Create more gameObjects for the pool
    private void GrowPool()
    {
        for(int i = 0; i < 10; i++)
        {
            // var: figure out what dataType (should be gameObject)
            var instanceToAdd = Instantiate(afterImagePrefab);
            // make gameObject recreate a child of the gameobject this script is attatched to
            instanceToAdd.transform.SetParent(transform);

            AddToPool(instanceToAdd);

        }
    }


    // called from other scripts via singleton
    public void AddToPool(GameObject instance)
    {
        instance.SetActive(false);
        availableObjects.Enqueue(instance);
    }

    // Function that gets an object from a pool
    public GameObject GetFromPool()
    {
        // if we are trying to get an afterimage to spawn
        // and there are none available make some more
        if(availableObjects.Count == 0)
        {
            GrowPool();
        }

        // take object from queue
        var instace = availableObjects.Dequeue();
        // onEnable function gets called in afterImage.cs
        instace.SetActive(true);
        return instace;
    }

}
