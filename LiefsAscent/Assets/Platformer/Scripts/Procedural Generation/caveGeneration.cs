using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class caveGeneration : MonoBehaviour
{
    [SerializeField] int width;
    [SerializeField] Tilemap groundTile, foliageTile;
    [SerializeField] Tile ground, foliage, pillar;
    [SerializeField] GameObject portal;
    private bool exitGenerated;
    [Range(0, 100)] //slider
    [SerializeField] float heightValue, smoothness; //octave height/ width, controls smoothness of procedural gen
    [SerializeField] int caveLayers;
    [SerializeField] ItemGen itemGen;
    [SerializeField] bool itemsOn = true; // when this is enabled items will spawn randomly thoughout the level
    [SerializeField] GameObject enemy;
    [SerializeField] GameObject vineCollider;

    // Start is called before the first frame update
    void Start()
    {
        GenerateCave();
    }

    void GenerateCave() //generates cave terrian with stalagmites overhead
    {
        float seed = Time.deltaTime;
        int height = Mathf.RoundToInt(heightValue * Mathf.PerlinNoise(Random.Range(0, 5) / smoothness, seed)); //assigns height of cave surface (platform)
        int caveHeight = Mathf.RoundToInt((heightValue * .5f) * Mathf.PerlinNoise(Random.Range(0, 0) / smoothness, seed)); // height of stalagmite (bottom of platform)
        float holeProbability = 0; //probability of generating a hole
        float pillarProbability = 0; // ^ for pillars
        int exitMade = Random.Range(1, caveLayers - 1); //picks a floor for exit
        Vector3Int exitLoc = new Vector3Int(30, 23, 0); //exit location is initialized next to spawn
        int spawnRate = 0; //enemy spawnrate

        //load random rock sprites
        int rockTiles = 6; //number of random rocks
        Tile[] rocks = new Tile[rockTiles];
        for (int rock = 0; rock < rockTiles; rock++)
        {
            string path = "rock" + rock.ToString();
            rocks[rock] = Resources.Load<Tile>(path); //load rock sprites
        }
        //tile dimensions I picked these randomly until I learn how to find the tile width through script
        int groundTileWidth = 3;
        int groundTileHeight = 1;

        //stack cave layers to simulate dungeon
        for (int k = 0; k < caveLayers; k++) //offsets vertical placement to create cave layers
        {
            pillarProbability = 0;
            holeProbability = 0;
            spawnRate += 2;

            for (int i = 0; i < width; i += groundTileWidth) //place ground tiles on x axis
            {
                if (k == exitMade && Random.Range(0, 100) <= (2 * (k + 1)) && pillarProbability > 0) //generate a chance to place an exit on a given floor if there is a hole nearby
                {   //chance of exit appearing increases as cave leve increases
                    exitLoc = new Vector3Int(i, height - Mathf.RoundToInt((k + 1) * height) + 25, 0); //place portal with offset of 25
                    exitMade = -1;
                }

                if (holeProbability < 5) //probability of generating a hole increases further away from last generated hole
                {
                    holeProbability += .1f;
                }

               
                //randomly generate holes
                if (Random.Range(0, 100) < holeProbability & k < caveLayers - 1) //chance for hole
                {
                    holeProbability = 0; //reset hole probability 
                    pillarProbability += 5;

                    int vineLength = 6;
                    //add first vine
                    Vector3Int pos = new Vector3Int(i, height - Mathf.RoundToInt(k * height) - vineLength, 0);
                    foliageTile.SetTile(pos, foliage);
                    Instantiate(vineCollider, pos, Quaternion.identity);

                    //while vine is out of reach, add more vines
                    while (pos.y > caveHeight - k*height)
                    {
                        pos.y -= vineLength;
                        foliageTile.SetTile(pos, foliage);
                        Instantiate(vineCollider, pos, Quaternion.identity);
                    }
                    i += Random.Range(4, 6); // size of hole
                }
                else if (Random.Range(0, 100) < pillarProbability && holeProbability > .5f) //if a hole has been placed
                {
                    int pillarHeight = 2;
                    Vector3Int pos;
                    int levelOffset;

                    if (k == caveLayers - 2 && Random.Range(0, 2) == 0) //places pillar in either level above or level below hole
                    {
                        pos = new Vector3Int(i, height - Mathf.RoundToInt((k + 1) * height), 0); //adds pillar on last level
                        levelOffset = k;
                        holeProbability += 10;
                    }
                    else
                    {
                        pos = new Vector3Int(i, height - Mathf.RoundToInt(k * height), 0);
                        levelOffset = k - 1;
                    }

                    groundTile.SetTile(pos, pillar);

                    while (pos.y < caveHeight - (levelOffset) * height)
                    {
                        groundTile.SetTile(pos, pillar);
                        pos.y += pillarHeight;
                    }
                    pillarProbability = 0; //reset pillar probability
                }
                else if (Random.Range(0, 100) <= 5) //generate random rocks
                {
                    Vector3Int pos = new Vector3Int(i, height - Mathf.RoundToInt(k * height), 0);
                    groundTile.SetTile(pos, rocks[Random.Range(0, rockTiles)]);
                }

                //generate new height
                height = Mathf.RoundToInt(heightValue * Mathf.PerlinNoise(1, seed)); //assigns height fr/ noise
                caveHeight = Mathf.RoundToInt((heightValue * .5f) * Mathf.PerlinNoise(Random.Range(1, i) / smoothness, seed)); //stalagmite/ bottom of platform

                //place enemies

                if (Random.Range(0, 100) < spawnRate)
                {
                    int enemyOffset = 3;
                    Instantiate(enemy, new Vector3Int(i, height - Mathf.RoundToInt(k * height) + enemyOffset, 0), Quaternion.identity);
                    spawnRate = 0; //reset spawn rate
                }

                //place items
                if (itemsOn == true)
                {
                    itemGen.generate(new Vector3(i + 0.5f, height + 1.5f - (k * height), 0f)); // call the itemGen script
                    //not entirely sure if this is working correctly, please let me know, Logan
                }

                int wallThickness = 3;

                if (i < wallThickness || i >= width - wallThickness) //add walls on either side of platform
                {
                    for (int j = (int)heightValue; j > (-(k - 2) * heightValue); j -= groundTileHeight)
                    {
                        groundTile.SetTile(new Vector3Int(i, j, 0), ground);
                    }
                }

                for (int j = caveHeight; j < height; j += groundTileHeight) //place ground tiles on y axis
                {
                    Vector3Int pos = new Vector3Int(i, j - Mathf.RoundToInt(k * height), 0);

                    groundTile.SetTile(pos, ground);
                }
            }
        }

        //groundTile.SetTile(new Vector3Int(15, 23, 0), portal); //set spawn point
        //groundTile.SetTile(exitLoc, portal); //set exit
        //changed portal location to make sure the exit is reachable
        portal.transform.position = new Vector3Int(15, 20, 0); 
    }
}
