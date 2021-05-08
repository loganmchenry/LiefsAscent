using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class towerGeneration : MonoBehaviour
{
    [SerializeField] int width;
    [SerializeField] int height;
    [SerializeField] int floors;
    [SerializeField] Tilemap groundTile, foliageTile;
    [SerializeField] Tile ground, rope;//, foliage, pillar, portal;
    [SerializeField] GameObject portal;
    [SerializeField] GameObject ropeCollider;
    [SerializeField] GameObject enemy;
    [SerializeField] GameObject sign;
    private bool exitGenerated;

    //[Range(0, 100)] //slider
    //[SerializeField] float heightValue, smoothness; //octave height/ width, controls smoothness of procedural gen

    //[SerializeField] ItemGen itemGen;
    //[SerializeField] bool itemsOn = true; // when this is enabled items will spawn randomly thoughout the level
    //SerializeField] GameObject enemy;


    // Start is called before the first frame update
    void Start()
    {
        GenerateTower();


        xCenter = 0;
        yCenter = 0;
        buildWall(currHeight, -width);
        buildWall(currHeight, width);
    }

    int xCenter = 0;
    int yCenter = 0;
    int currHeight = 0;

    void buildWall(int height, int x, int wallThickness = 3)
    {
        for (int i = yCenter; i < height + yCenter; i++)
        {
            Vector3Int pos = new Vector3Int(x, i, 0);
            for (int w = 0; w < wallThickness; w++)
            {
                groundTile.SetTile(pos, ground);
                pos.x++;
            }
        }

    }

    void buildFloor(int width, int y, int floorThickness = 3)
    {
        int spawnRate = 0;
        for (int j = xCenter; j < width + xCenter; j++)
        {
            Vector3Int pos = new Vector3Int(j, y, 0);
            for (int w = 0; w < floorThickness; w++)
            {
                if (w == floorThickness - 1) //spawn on top level
                {
                    spawnRate+= 2;
                    if (Random.Range(0, 100) < spawnRate) //spawn enemy
                    {
                        int enemyOffset = 4;
                        Vector3Int enemyPos = pos;
                        enemyPos.y += enemyOffset;

                        Instantiate(enemy, enemyPos, Quaternion.identity);
                    }
                    spawnRate = 0; //reset spawn rate
                }

                groundTile.SetTile(pos, ground);
                pos.y++;
            }
        }
    }
    void GenerateTower() //generates cave terrian with stalagmites overhead
    {
        //build start point
        xCenter = -width;
        buildFloor(2 * width, 0);
        xCenter = 0;

        //floor one
        buildSpiral();
        //floor two
        trickLadder(0, currHeight, Random.Range(20, 40));
        //floor three
        buildMaze(Random.Range(5, 7));
    }

    void cascade(int width, int height, int yGap = 3) //builds random distribution of ascending blocks
    {
        int xFinal;
        for (int y = 0; y < height; y += yGap)
        {
            xFinal = Random.Range(-width,width);

            Vector3Int pos = new Vector3Int(xCenter + xFinal, currHeight + y, 0);
        }

        currHeight = currHeight + height + 10;
        buildFloor(width - 7, currHeight - 3, 1);
    }
    void buildMaze(int floors)
    {
        int floorHeight = 22;

        currHeight += floorHeight;
        xCenter = -width;
        buildFloor(width - 5, currHeight);
        buildRope(-5, currHeight);
        xCenter = 0;
        buildFloor(width, currHeight);
        int holeProbability = 5;
        int pillarProbability = 0;
        int spawnRate = 0;

        for (int floor = 0; floor < floors; floor++)
        {
            spawnRate++;
            currHeight += floorHeight;
            pillarProbability = 0;
            holeProbability = 5;

            for (int x = -width + 2; x < width - 2; x ++)
            {
                if (holeProbability < 5)
                {
                    holeProbability++;
                }
                if (Random.Range(0, 100) < holeProbability) //make holes
                {
                    buildRope(x, currHeight);
                    x += Random.Range(7, 10);

                    holeProbability = 0;
                    pillarProbability += 5;
                }
                else if (Random.Range(0, 100) < pillarProbability && floor > 0)
                {
                    yCenter = currHeight - floorHeight;
                    buildWall(floorHeight, x, 1);
                    x += 3;
                    pillarProbability = 0;
                }        

                Vector3Int pos = new Vector3Int(x, currHeight, 0);

                if (Random.Range(0, 100) < spawnRate) //spawn enemy
                {
                    int enemyOffset = 4;
                    Vector3Int enemyPos = pos;
                    enemyPos.y += enemyOffset;

                    Instantiate(enemy, enemyPos, Quaternion.identity);
                    spawnRate = 0; //reset spawn rate
                }


                groundTile.SetTile(pos, ground);

                //maybe add a chance to drop some cool loot here, maybe keys or something

                //Make a portal at the end
                if (floor >= floors -1 && !exitGenerated && x > width / 2)
                {
                    portal.transform.position = pos + new Vector3Int(0, 1, 0);
                    exitGenerated = true;
                }

            }
      
        }
        //currHeight += floorHeight;
    }

    void buildRope(int x, int y)
    {
        //add first vine
        int ropeOffset = 6;
        Vector3Int pos = new Vector3Int(x, y - ropeOffset, 0);
        foliageTile.SetTile(pos, rope);
        Instantiate(ropeCollider, pos, Quaternion.identity);
    }

    void trickLadder(int x, int y, int height, int xOffset = 5, int mode = 0) //load trick ladder chunk
    {
        for (int h = 0; h < height; h += 5)
        {

            Vector3Int pos = new Vector3Int(x + xOffset, y + h, 0);
            if (h % 2 == 0)
            {
                pos = new Vector3Int(x - xOffset, y + h, 0);
            }
            groundTile.SetTile(pos, ground);
        }

        groundTile.SetTile(new Vector3Int(x - xOffset, y + height, 0), ground);
        currHeight = y + height + 10;

        buildFloor(width - 7, currHeight - 3, 1);
        Instantiate(sign, new Vector3Int(5, currHeight, 0), Quaternion.identity);
    }

    void buildSpiral() //load spiral chunk
    {
        currHeight = buildStaircase(width, 0);
        buildFloor(width - 16, currHeight - 3, 1);
        currHeight = buildStaircase(width, currHeight, -1);
        xCenter = -width + 16;
        buildFloor(width - 16, currHeight - 3, 1);
        xCenter = 0;
        currHeight = buildStaircase(width, currHeight);
        buildFloor(width - 16, currHeight - 3, 1);
    }

    int buildStaircase(int width, int y, int direction = 1, int xgap = 10, int ygap = 7, int stairSize = 1)
    {
        int height = y;
        for (int x = xCenter; x < width + xCenter; x += xgap)
        {
            Vector3Int pos = new Vector3Int(xCenter + (x * direction), y, 0);
            for (int w = 0; w < stairSize; w++)
            {
                groundTile.SetTile(pos, ground);
                pos.x++;             
            }
            if (ygap == -1)
            {
                y += Random.Range(-5, 5); //random platform placement
            }
            else
            {
                y += ygap; //creates upward stairs
                height += ygap;
            }
        }
        return height;
    }

}
