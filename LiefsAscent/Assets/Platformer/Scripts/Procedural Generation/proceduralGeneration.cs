using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class proceduralGeneration : MonoBehaviour
{
    [SerializeField] int width;
    //[SerializeField] GameObject ground, foliage;
    [SerializeField] Tilemap groundTile, foliageTile;
    [SerializeField] Tile ground, foliage;
    [Range(0, 100)] //slider
    [SerializeField] float heightValue, smoothness; //octave height/ width, controls smoothness of procedural gen
    [SerializeField] ItemGen itemGen;
    [SerializeField] bool itemsOn = true; // when this is enabled items will spawn randomly thoughout the level
    [SerializeField] GameObject enemy;
    [SerializeField] GameObject portal;
    private bool exitGenerated = false;

    // Start is called before the first frame update
    void Start()
    {
        GenerateGround();
    }
   
    void GenerateGround() //x parameter passed marks where generation starts on horizontal axis
    {
        float seed = Time.deltaTime;
        int height = Mathf.RoundToInt(heightValue * Mathf.PerlinNoise((0) / smoothness, seed));
        int platform = 0; //platform width
        int spawnRate = 0;

        int iceItems = 2;
        Tile[] groundStuff = new Tile[iceItems];
        for (int i = 0; i < iceItems; i++)
        {
            groundStuff[i] = Resources.Load<Tile>("ice" + i.ToString()); //load dead tree
        }

        for (int i = 0;  i < width; i += 3) //place ground tiles on x axis
        {
            spawnRate += 5;

            //randomly generate holes
            if (Random.Range(0, 100) < 5) //hole probability
                i += Random.Range(3, 7); // size between 5 and 10 spaces

            if (Random.Range(0, 100) < 7)   //randomly generate platforms
                platform = Random.Range(5, 20); //platform width  

            if (platform > 0) //keep height constant
                platform--;

            else //generate new height
                height = Mathf.RoundToInt(heightValue * Mathf.PerlinNoise((i) / smoothness, seed)); //assigns height fr/ noise


            
            for (int j = 0; j < height; j += 2) //place ground tiles on y axis
            {
                Vector3Int pos = new Vector3Int(i, j, 0);
                groundTile.SetTile(pos, ground); //set ground tile

                //tints ground farther from surface
                groundTile.SetTileFlags(pos, TileFlags.None);
                float tint = (((height - j) * 1.0f) / height);
                Color newColor = groundTile.GetColor(pos);
                newColor.r -= tint;
                newColor.b -= tint;
                newColor.g -= tint;
                groundTile.SetColor(new Vector3Int(i, j, 0), newColor);

                //Make a portal at the end
                if (i > width - 50 && j > height-3 && !exitGenerated) 
                {
                    portal.transform.position = pos + new Vector3Int(0, 2, 0);
                    exitGenerated = true;
                }
            }

            foliageTile.SetTile(new Vector3Int(i, height, 0), foliage); //set foliage tile

            //place platform objects
            if (Random.Range(0, 100) < spawnRate && platform > 0)
            {
                if (Random.Range(0, 3) == 0) //spawn enemy
                {
                    int enemyOffset = 4;
                    Instantiate(enemy, new Vector3Int(i, height + enemyOffset, 0), Quaternion.identity);
                }
                spawnRate = 0; //reset spawn rate
            }

            if (Random.Range(0,100) <= 5) //ground misc. objects
            {
                if (Random.Range(0, 1) == 0 && platform > 0) //spawn background objects on ground
                {
                    int offset = 5;
                    groundTile.SetTile(new Vector3Int(i, height + offset, 0), groundStuff[0]);
                }
                else
                {
                    int offset = 2;
                    groundTile.SetTile(new Vector3Int(i, height + offset, 0), groundStuff[1]);
                }
            }

            // added to randomly spawn items throughout the level
            if (itemsOn == true) 
            {
                itemGen.generate(new Vector3(i+0.5f, height+1.5f, 0f)); // call the itemGen script
            }
        }
    }

}
