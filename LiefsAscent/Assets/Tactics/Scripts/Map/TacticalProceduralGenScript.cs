/*  Documentation:
 *  Intended use
 *      Manage the procedural generation of the tactics map
 *      and unit spawning
 *      
 *  Last Documentation Update: 5 May 2021
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TacticalProceduralGenScript : MonoBehaviour
{
    [SerializeField] Tilemap groundLayer;
    [SerializeField] Tile grassTile, grassTile1, stoneTile, waterTile, snowTile, snowTile1, iceTile, holeTile, groundTile, groundTile1, castleTile, castleTile1;
    [SerializeField] GameObject PlayerUnit, EvilWizard0;
    [SerializeField] GameObject Thief, Archer, Healer, Mage, Warrior;       //The player party units
    [SerializeField] GameObject LokiBoss, EvilWizard1, EvilWizard2, EvilWizard3, Goblin, Draugr0, Draugr1, Draugr2, Draugr3, Fiel0, Fiel1, Fiel2, Fiel3, Fiel4; //Enemy units

    [SerializeField]
    public List<TileData> tileDatas;

    public Dictionary<TileBase, TileData> dataFromTiles;
    public Tilemap Map { get { return groundLayer; } }
    public Dictionary<TileBase, TileData> mapData { get { return dataFromTiles; } }

    public static int enemyCtr;                                           //# of enemy units, to be randomized at start of battle (for battles before Valhalla)
    public static int enemyValhalla;                                      //# of enemy units fought in Valhalla battles, to be randomized at start of battle
    private GameObject gameStats;
    private int currentHealth;
    private int maxHealth;
    private int speed;
    private int attackPower;
    private string charName;
    private int movement;
    private int abilityPoints;
    private int currentY;
    private int currentX;

    // Start is called before the first frame update
    void Start()
    {
        dataFromTiles = new Dictionary<TileBase, TileData>();
        gameStats = GameObject.Find("GameStats");
        currentHealth = PlayerStats.instance.currentHealth;
        maxHealth = PlayerStats.instance.maxHealth;
        speed = PlayerStats.instance.speed;
        attackPower = PlayerStats.instance.attackPower;
        charName = PlayerStats.instance.characterName;
        movement = PlayerStats.instance.maxMove;
        abilityPoints = PlayerStats.instance.currentAP;
        currentY = PlayerStats.instance.yPos;
        currentX = PlayerStats.instance.xPos;
        enemyCtr = Random.Range(1, 5);
        enemyValhalla = Random.Range(1, 6);

        foreach (var tileData in tileDatas)
        {
            foreach (var tile in tileData.tiles)
            {
                dataFromTiles.Add(tile, tileData);
            }
        }

        //while map NOT solvable
        MakeBoard();                //Generate the board
        SpawnPlayerParty();         //Spawn player party
        if (PlayerStats.instance.isLoki == true)
        {
            PlayerStats.instance.isLoki = false;
            TacticalManager.isLokiFight = true;
            SetUpBoss();                //Set Loki Stats
            SetEnemyPos();              //Spawn enemy units
        }
        else
        {
            SetEnemyPos();
        }




        Debug.Log("currX: " + currentX);
        Debug.Log("currY: " + currentY);
        //PlayerUnit.tag = "Unit";
        //EnemyUnit.tag = "Unit";
    }
    private void Update()
    {

    }

    /*  Function Name:  Make Board
     *  Input:          None, called at Start()
     *  Output:         Generate a 16x16 board of (1-cost), (2-cost), (impassible) tiles.           */
    void MakeBoard() {
        //Code for walkable tiles
        if (PlayerStats.instance.inNifl == true){
            for (int i = -8; i < 8; i++)
            {
                for (int j = -8; j < 8; j++)
                {
                    Vector3Int pos = new Vector3Int(i, j, 0);
                    var rng = Random.Range(1, 9);
                    switch (rng)
                    {
                        case 1:
                            groundLayer.SetTile(pos, snowTile1);
                            break;
                        case 2:
                        case 3:
                        case 4:
                        case 5:
                        case 6:
                            groundLayer.SetTile(pos, snowTile);
                            break;
                        case 7:
                            groundLayer.SetTile(pos, iceTile);
                            break;

                        case 8:
                            groundLayer.SetTile(pos, waterTile);
                            break;

                    }
                }
            }
        }
        else if (PlayerStats.instance.inCave == true)
        {
            for (int i = -8; i < 8; i++)
            {
                for (int j = -8; j < 8; j++)
                {
                    Vector3Int pos = new Vector3Int(i, j, 0);
                    var rng = Random.Range(1, 9);
                    switch (rng)
                    {
                        case 1:
                        case 2:
                        case 3:
                            groundLayer.SetTile(pos, groundTile1);
                            break;
                        case 4:
                        case 5:
                        case 6:
                        case 7:
                            groundLayer.SetTile(pos, groundTile);
                            break;
                        case 8:
                            groundLayer.SetTile(pos, holeTile);
                            break;

                    }
                }
            }
        }
        else if(PlayerStats.instance.inValh == true)
        {
            for (int i = -8; i < 8; i++)
            {
                for (int j = -8; j < 8; j++)
                {
                    Vector3Int pos = new Vector3Int(i, j, 0);
                    var rng = Random.Range(1, 9);
                    switch (rng)
                    {
                        case 1:
                        case 2:
                        case 3:
                            groundLayer.SetTile(pos, castleTile);
                            break;
                        case 4:
                        case 5:
                        case 6:
                            groundLayer.SetTile(pos, castleTile1);
                            break;
                        case 7:
                        case 8:
                            groundLayer.SetTile(pos, stoneTile);
                            break;

                    }
                }
            }
        }
        else {
            for (int i = -8; i < 8; i++)
            {
                for (int j = -8; j < 8; j++)
                {
                    Vector3Int pos = new Vector3Int(i, j, 0);
                    var rng = Random.Range(1, 9);
                    switch (rng)
                    {
                        case 1:
                        case 2:
                            groundLayer.SetTile(pos, grassTile1);
                            break;
                        case 3:
                        case 4:
                        case 5:
                            groundLayer.SetTile(pos, grassTile);
                            break;
                        case 6:
                        case 7:
                            groundLayer.SetTile(pos, stoneTile);
                            break;

                        case 8:
                            groundLayer.SetTile(pos, waterTile);
                            break;

                    }
                }
            }
        }
    }

    void SetPlayerPos(GameObject unit)
    {
        int xPos = Random.Range(-8, 0);
        int yPos = Random.Range(-8, 8);
        Vector3Int pos = new Vector3Int(xPos, yPos, 0);
        TileBase playerTile = groundLayer.GetTile(pos);
        while(dataFromTiles[playerTile].isWalkable == false)
        {
            xPos = Random.Range(-8, 0);
            yPos = Random.Range(-8, 8);
            pos = new Vector3Int(xPos, yPos, 0);
            playerTile = groundLayer.GetTile(pos);
        }
        //Convert the grid position to a world position (2 is offset because it puts it on the corner of the cell)
        Vector3 charPos = groundLayer.CellToWorld(pos);
        
        charPos.x += 2;
        charPos.y += 2;
        unit.transform.position = charPos;
        //Update the units info
        unit.GetComponent<CharacterScript>().SetPos(xPos, yPos);
        Debug.Log(PlayerUnit.GetComponent<CharacterScript>().XPos);
        Debug.Log(PlayerUnit.GetComponent<CharacterScript>().YPos);
    }
    void SetEnemyPos()
    {
        if (PlayerStats.instance.isGoblin == true)
        {
            enemyCtr = 1;
            PlayerStats.instance.isGoblin = false;
            int xPosG = Random.Range(0, 8);
            int yPosG = Random.Range(-8, 8);
            //Check if the pos is on a walkable tile
            Vector3Int pos = new Vector3Int(xPosG, yPosG, 0);
            TileBase playerTile = groundLayer.GetTile(pos);
            while (dataFromTiles[playerTile].isWalkable == false)
            {
                xPosG = Random.Range(0, 8);
                yPosG = Random.Range(-8, 8);
                pos = new Vector3Int(xPosG, yPosG, 0);
                playerTile = groundLayer.GetTile(pos);
            }
            Goblin.SetActive(true);
            Goblin.gameObject.transform.GetComponent<CharacterScript>().ForceStats();
            //pos = new Vector3Int(xPosG, yPosG, 0);
            //Convert the grid position to a world position (2 is offset because it puts it on the corner of the cell)
            Vector3 charPos = groundLayer.CellToWorld(pos);
            charPos.x += 2;
            charPos.y += 2;
            Goblin.transform.position = charPos;
            //Update the units info
            Goblin.GetComponent<CharacterScript>().SetPos(xPosG, yPosG);
        }
        else if (PlayerStats.instance.inNifl == false && PlayerStats.instance.inCave == false && PlayerStats.instance.inValh == false)
        {
            for (int i = 0; i < enemyCtr; i++)
            {
                if (i == 0)
                {
                    int xPos = Random.Range(0, 8);
                    int yPos = Random.Range(-8, 8);
                    //Check if the pos is on a walkable tile
                    Vector3Int pos = new Vector3Int(xPos, yPos, 0);
                    TileBase playerTile = groundLayer.GetTile(pos);
                    while (dataFromTiles[playerTile].isWalkable == false)
                    {
                        xPos = Random.Range(0, 8);
                        yPos = Random.Range(-8, 8);
                        pos = new Vector3Int(xPos, yPos, 0);
                        playerTile = groundLayer.GetTile(pos);
                    }
                    EvilWizard0.SetActive(true);
                    EvilWizard0.gameObject.transform.GetComponent<CharacterScript>().ForceStats();
                    //pos = new Vector3Int(xPos, yPos, 0);
                    //Convert the grid position to a world position (2 is offset because it puts it on the corner of the cell)
                    Vector3 charPos = groundLayer.CellToWorld(pos);
                    charPos.x += 2;
                    charPos.y += 2;
                    EvilWizard0.transform.position = charPos;
                    //Update the units info
                    EvilWizard0.GetComponent<CharacterScript>().SetPos(xPos, yPos);
                }
                else if (i == 1)
                {
                    int xPos1 = Random.Range(0, 8);
                    int yPos1 = Random.Range(-8, 8);
                    //Check if the pos is on a walkable tile
                    Vector3Int pos = new Vector3Int(xPos1, yPos1, 0);
                    TileBase playerTile = groundLayer.GetTile(pos);
                    while (dataFromTiles[playerTile].isWalkable == false)
                    {
                        xPos1 = Random.Range(0, 8);
                        yPos1 = Random.Range(-8, 8);
                        pos = new Vector3Int(xPos1, yPos1, 0);
                        playerTile = groundLayer.GetTile(pos);
                    }
                    Goblin.SetActive(true);
                    Goblin.gameObject.transform.GetComponent<CharacterScript>().ForceStats();
                    //pos = new Vector3Int(xPos1, yPos1, 0);
                    //Convert the grid position to a world position (2 is offset because it puts it on the corner of the cell)
                    Vector3 charPos = groundLayer.CellToWorld(pos);
                    charPos.x += 2;
                    charPos.y += 2;
                    Goblin.transform.position = charPos;
                    //Update the units info
                    Goblin.GetComponent<CharacterScript>().SetPos(xPos1, yPos1);
                }
                else if (i == 2)
                {
                    int xPos2 = Random.Range(0, 8);
                    int yPos2 = Random.Range(-8, 8);
                    //Check if the pos is on a walkable tile
                    Vector3Int pos = new Vector3Int(xPos2, yPos2, 0);
                    TileBase playerTile = groundLayer.GetTile(pos);
                    while (dataFromTiles[playerTile].isWalkable == false)
                    {
                        xPos2 = Random.Range(0, 8);
                        yPos2 = Random.Range(-8, 8);
                        pos = new Vector3Int(xPos2, yPos2, 0);
                        playerTile = groundLayer.GetTile(pos);
                    }
                    Draugr0.SetActive(true);
                    Draugr0.gameObject.transform.GetComponent<CharacterScript>().ForceStats();
                    //pos = new Vector3Int(xPos2, yPos2, 0);
                    //Convert the grid position to a world position (2 is offset because it puts it on the corner of the cell)
                    Vector3 charPos = groundLayer.CellToWorld(pos);
                    charPos.x += 2;
                    charPos.y += 2;
                    Draugr0.transform.position = charPos;
                    //Update the units info
                    Draugr0.GetComponent<CharacterScript>().SetPos(xPos2, yPos2);
                }
                else
                {
                    int xPos3 = Random.Range(0, 8);
                    int yPos3 = Random.Range(-8, 8);
                    //Check if the pos is on a walkable tile
                    Vector3Int pos = new Vector3Int(xPos3, yPos3, 0);
                    TileBase playerTile = groundLayer.GetTile(pos);
                    while (dataFromTiles[playerTile].isWalkable == false)
                    {
                        xPos3 = Random.Range(0, 8);
                        yPos3 = Random.Range(-8, 8);
                        pos = new Vector3Int(xPos3, yPos3, 0);
                        playerTile = groundLayer.GetTile(pos);
                    }
                    Draugr1.SetActive(true);
                    Draugr1.gameObject.transform.GetComponent<CharacterScript>().ForceStats();
                    //pos = new Vector3Int(xPos3, yPos3, 0);
                    //Convert the grid position to a world position (2 is offset because it puts it on the corner of the cell)
                    Vector3 charPos = groundLayer.CellToWorld(pos);
                    charPos.x += 2;
                    charPos.y += 2;
                    Draugr1.transform.position = charPos;
                    //Update the units info
                    Draugr1.GetComponent<CharacterScript>().SetPos(xPos3, yPos3);
                }
            }
        }

        //1st and 2nd region: 4 enemies max, Valhalla only region with 5 enemies
        else if (PlayerStats.instance.inNifl == true)
        {
            for (int i = 0; i < enemyCtr; i++)
            {
                if (i == 0)
                {
                    int xPos = Random.Range(0, 8);
                    int yPos = Random.Range(-8, 8);
                    //Check if the pos is on a walkable tile
                    Vector3Int pos = new Vector3Int(xPos, yPos, 0);
                    TileBase playerTile = groundLayer.GetTile(pos);
                    while (dataFromTiles[playerTile].isWalkable == false)
                    {
                        xPos = Random.Range(0, 8);
                        yPos = Random.Range(-8, 8);
                        pos = new Vector3Int(xPos, yPos, 0);
                        playerTile = groundLayer.GetTile(pos);
                    }
                    Draugr0.SetActive(true);
                    Draugr0.gameObject.transform.GetComponent<CharacterScript>().ForceStats();
                   // Vector3Int pos = new Vector3Int(xPos, yPos, 0);
                    //Convert the grid position to a world position (2 is offset because it puts it on the corner of the cell)
                    Vector3 charPos = groundLayer.CellToWorld(pos);
                    charPos.x += 2;
                    charPos.y += 2;
                    Draugr0.transform.position = charPos;
                    //Update the units info
                    Draugr0.GetComponent<CharacterScript>().SetPos(xPos, yPos);
                }
                else if (i == 1)
                {
                    int xPos1 = Random.Range(0, 8);
                    int yPos1 = Random.Range(-8, 8);
                    //Check if the pos is on a walkable tile
                    Vector3Int pos = new Vector3Int(xPos1, yPos1, 0);
                    TileBase playerTile = groundLayer.GetTile(pos);
                    while (dataFromTiles[playerTile].isWalkable == false)
                    {
                        xPos1 = Random.Range(0, 8);
                        yPos1 = Random.Range(-8, 8);
                        pos = new Vector3Int(xPos1, yPos1, 0);
                        playerTile = groundLayer.GetTile(pos);
                    }
                    Draugr1.SetActive(true);
                    Draugr1.gameObject.transform.GetComponent<CharacterScript>().ForceStats();
                   // Vector3Int pos = new Vector3Int(xPos1, yPos1, 0);
                    //Convert the grid position to a world position (2 is offset because it puts it on the corner of the cell)
                    Vector3 charPos = groundLayer.CellToWorld(pos);
                    charPos.x += 2;
                    charPos.y += 2;
                    Draugr1.transform.position = charPos;
                    //Update the units info
                    Draugr1.GetComponent<CharacterScript>().SetPos(xPos1, yPos1);
                }
                else if (i == 2)
                {
                    int xPos2 = Random.Range(0, 8);
                    int yPos2 = Random.Range(-8, 8);
                    //Check if the pos is on a walkable tile
                    Vector3Int pos = new Vector3Int(xPos2, yPos2, 0);
                    TileBase playerTile = groundLayer.GetTile(pos);
                    while (dataFromTiles[playerTile].isWalkable == false)
                    {
                        xPos2 = Random.Range(0, 8);
                        yPos2 = Random.Range(-8, 8);
                        pos = new Vector3Int(xPos2, yPos2, 0);
                        playerTile = groundLayer.GetTile(pos);
                    }
                    Draugr2.SetActive(true);
                    Draugr2.gameObject.transform.GetComponent<CharacterScript>().ForceStats();
                    //Vector3Int pos = new Vector3Int(xPos2, yPos2, 0);
                    //Convert the grid position to a world position (2 is offset because it puts it on the corner of the cell)
                    Vector3 charPos = groundLayer.CellToWorld(pos);
                    charPos.x += 2;
                    charPos.y += 2;
                    Draugr2.transform.position = charPos;
                    //Update the units info
                    Draugr2.GetComponent<CharacterScript>().SetPos(xPos2, yPos2);
                }
                else
                {
                    int xPos3 = Random.Range(0, 8);
                    int yPos3 = Random.Range(-8, 8);
                    //Check if the pos is on a walkable tile
                    Vector3Int pos = new Vector3Int(xPos3, yPos3, 0);
                    TileBase playerTile = groundLayer.GetTile(pos);
                    while (dataFromTiles[playerTile].isWalkable == false)
                    {
                        xPos3 = Random.Range(0, 8);
                        yPos3 = Random.Range(-8, 8);
                        pos = new Vector3Int(xPos3, yPos3, 0);
                        playerTile = groundLayer.GetTile(pos);
                    }
                    Draugr3.SetActive(true);
                    Draugr3.gameObject.transform.GetComponent<CharacterScript>().ForceStats();
                    //Vector3Int pos = new Vector3Int(xPos3, yPos3, 0);
                    //Convert the grid position to a world position (2 is offset because it puts it on the corner of the cell)
                    Vector3 charPos = groundLayer.CellToWorld(pos);
                    charPos.x += 2;
                    charPos.y += 2;
                    Draugr3.transform.position = charPos;
                    //Update the units info
                    Draugr3.GetComponent<CharacterScript>().SetPos(xPos3, yPos3);
                }
            }
        }
        else if (PlayerStats.instance.inCave == true)
        {
            for (int i = 0; i < enemyCtr; i++)
            {
                if (i == 0)
                {
                    int xPos = Random.Range(0, 8);
                    int yPos = Random.Range(-8, 8);
                    //Check if the pos is on a walkable tile
                    Vector3Int pos = new Vector3Int(xPos, yPos, 0);
                    TileBase playerTile = groundLayer.GetTile(pos);
                    while (dataFromTiles[playerTile].isWalkable == false)
                    {
                        xPos = Random.Range(0, 8);
                        yPos = Random.Range(-8, 8);
                        pos = new Vector3Int(xPos, yPos, 0);
                        playerTile = groundLayer.GetTile(pos);
                    }
                    EvilWizard0.SetActive(true);
                    EvilWizard0.gameObject.transform.GetComponent<CharacterScript>().ForceStats();
                    //Vector3Int pos = new Vector3Int(xPos, yPos, 0);
                    //Convert the grid position to a world position (2 is offset because it puts it on the corner of the cell)
                    Vector3 charPos = groundLayer.CellToWorld(pos);
                    charPos.x += 2;
                    charPos.y += 2;
                    EvilWizard0.transform.position = charPos;
                    //Update the units info
                    EvilWizard0.GetComponent<CharacterScript>().SetPos(xPos, yPos);
                }
                else if (i == 1)
                {
                    int xPos1 = Random.Range(0, 8);
                    int yPos1 = Random.Range(-8, 8);
                    //Check if the pos is on a walkable tile
                    Vector3Int pos = new Vector3Int(xPos1, yPos1, 0);
                    TileBase playerTile = groundLayer.GetTile(pos);
                    while (dataFromTiles[playerTile].isWalkable == false)
                    {
                        xPos1 = Random.Range(0, 8);
                        yPos1 = Random.Range(-8, 8);
                        pos = new Vector3Int(xPos1, yPos1, 0);
                        playerTile = groundLayer.GetTile(pos);
                    }
                    EvilWizard1.SetActive(true);
                    EvilWizard1.gameObject.transform.GetComponent<CharacterScript>().ForceStats();
                   // Vector3Int pos = new Vector3Int(xPos1, yPos1, 0);
                    //Convert the grid position to a world position (2 is offset because it puts it on the corner of the cell)
                    Vector3 charPos = groundLayer.CellToWorld(pos);
                    charPos.x += 2;
                    charPos.y += 2;
                    EvilWizard1.transform.position = charPos;
                    //Update the units info
                    EvilWizard1.GetComponent<CharacterScript>().SetPos(xPos1, yPos1);
                }
                else if (i == 2)
                {
                    int xPos2 = Random.Range(0, 8);
                    int yPos2 = Random.Range(-8, 8);
                    //Check if the pos is on a walkable tile
                    Vector3Int pos = new Vector3Int(xPos2, yPos2, 0);
                    TileBase playerTile = groundLayer.GetTile(pos);
                    while (dataFromTiles[playerTile].isWalkable == false)
                    {
                        xPos2 = Random.Range(0, 8);
                        yPos2 = Random.Range(-8, 8);
                        pos = new Vector3Int(xPos2, yPos2, 0);
                        playerTile = groundLayer.GetTile(pos);
                    }
                    EvilWizard2.SetActive(true);
                    EvilWizard2.gameObject.transform.GetComponent<CharacterScript>().ForceStats();
                    //Vector3Int pos = new Vector3Int(xPos2, yPos2, 0);
                    //Convert the grid position to a world position (2 is offset because it puts it on the corner of the cell)
                    Vector3 charPos = groundLayer.CellToWorld(pos);
                    charPos.x += 2;
                    charPos.y += 2;
                    EvilWizard2.transform.position = charPos;
                    //Update the units info
                    EvilWizard2.GetComponent<CharacterScript>().SetPos(xPos2, yPos2);
                }
                else
                {
                    int xPos3 = Random.Range(0, 8);
                    int yPos3 = Random.Range(-8, 8);
                    //Check if the pos is on a walkable tile
                    Vector3Int pos = new Vector3Int(xPos3, yPos3, 0);
                    TileBase playerTile = groundLayer.GetTile(pos);
                    while (dataFromTiles[playerTile].isWalkable == false)
                    {
                        xPos3 = Random.Range(0, 8);
                        yPos3 = Random.Range(-8, 8);
                        pos = new Vector3Int(xPos3, yPos3, 0);
                        playerTile = groundLayer.GetTile(pos);
                    }
                    EvilWizard3.SetActive(true);
                    EvilWizard3.gameObject.transform.GetComponent<CharacterScript>().ForceStats();
                    //Vector3Int pos = new Vector3Int(xPos3, yPos3, 0);
                    //Convert the grid position to a world position (2 is offset because it puts it on the corner of the cell)
                    Vector3 charPos = groundLayer.CellToWorld(pos);
                    charPos.x += 2;
                    charPos.y += 2;
                    EvilWizard3.transform.position = charPos;
                    //Update the units info
                    EvilWizard3.GetComponent<CharacterScript>().SetPos(xPos3, yPos3);
                }
            }
        }
        else
        {
            for (int i = 0; i < enemyValhalla; i++)
            {
                if (i == 0)
                {
                    int xPos = Random.Range(0, 8);
                    int yPos = Random.Range(-8, 8);
                    //Check if the pos is on a walkable tile
                    Vector3Int pos = new Vector3Int(xPos, yPos, 0);
                    TileBase playerTile = groundLayer.GetTile(pos);
                    while (dataFromTiles[playerTile].isWalkable == false)
                    {
                        xPos = Random.Range(0, 8);
                        yPos = Random.Range(-8, 8);
                        pos = new Vector3Int(xPos, yPos, 0);
                        playerTile = groundLayer.GetTile(pos);
                    }
                    Fiel0.SetActive(true);
                    Fiel0.gameObject.transform.GetComponent<CharacterScript>().ForceStats();
                   // Vector3Int pos = new Vector3Int(xPos, yPos, 0);
                    //Convert the grid position to a world position (2 is offset because it puts it on the corner of the cell)
                    Vector3 charPos = groundLayer.CellToWorld(pos);
                    charPos.x += 2;
                    charPos.y += 2;
                    Fiel0.transform.position = charPos;
                    //Update the units info
                    Fiel0.GetComponent<CharacterScript>().SetPos(xPos, yPos);
                }
                else if (i == 1)
                {
                    int xPos1 = Random.Range(0, 8);
                    int yPos1 = Random.Range(-8, 8);
                    //Check if the pos is on a walkable tile
                    Vector3Int pos = new Vector3Int(xPos1, yPos1, 0);
                    TileBase playerTile = groundLayer.GetTile(pos);
                    while (dataFromTiles[playerTile].isWalkable == false)
                    {
                        xPos1 = Random.Range(0, 8);
                        yPos1 = Random.Range(-8, 8);
                        pos = new Vector3Int(xPos1, yPos1, 0);
                        playerTile = groundLayer.GetTile(pos);
                    }
                    Fiel1.SetActive(true);
                    Fiel1.gameObject.transform.GetComponent<CharacterScript>().ForceStats();
                    //Vector3Int pos = new Vector3Int(xPos1, yPos1, 0);
                    //Convert the grid position to a world position (2 is offset because it puts it on the corner of the cell)
                    Vector3 charPos = groundLayer.CellToWorld(pos);
                    charPos.x += 2;
                    charPos.y += 2;
                    Fiel1.transform.position = charPos;
                    //Update the units info
                    Fiel1.GetComponent<CharacterScript>().SetPos(xPos1, yPos1);
                }
                else if (i == 2)
                {
                    int xPos2 = Random.Range(0, 8);
                    int yPos2 = Random.Range(-8, 8);
                    //Check if the pos is on a walkable tile
                    Vector3Int pos = new Vector3Int(xPos2, yPos2, 0);
                    TileBase playerTile = groundLayer.GetTile(pos);
                    while (dataFromTiles[playerTile].isWalkable == false)
                    {
                        xPos2 = Random.Range(0, 8);
                        yPos2 = Random.Range(-8, 8);
                        pos = new Vector3Int(xPos2, yPos2, 0);
                        playerTile = groundLayer.GetTile(pos);
                    }
                    Fiel2.SetActive(true);
                    Fiel2.gameObject.transform.GetComponent<CharacterScript>().ForceStats();
                    //Vector3Int pos = new Vector3Int(xPos2, yPos2, 0);
                    //Convert the grid position to a world position (2 is offset because it puts it on the corner of the cell)
                    Vector3 charPos = groundLayer.CellToWorld(pos);
                    charPos.x += 2;
                    charPos.y += 2;
                    Fiel2.transform.position = charPos;
                    //Update the units info
                    Fiel2.GetComponent<CharacterScript>().SetPos(xPos2, yPos2);
                }
                else if (i == 3)
                {
                    int xPos3 = Random.Range(0, 8);
                    int yPos3 = Random.Range(-8, 8);
                    //Check if the pos is on a walkable tile
                    Vector3Int pos = new Vector3Int(xPos3, yPos3, 0);
                    TileBase playerTile = groundLayer.GetTile(pos);
                    while (dataFromTiles[playerTile].isWalkable == false)
                    {
                        xPos3 = Random.Range(0, 8);
                        yPos3 = Random.Range(-8, 8);
                        pos = new Vector3Int(xPos3, yPos3, 0);
                        playerTile = groundLayer.GetTile(pos);
                    }
                    Fiel3.SetActive(true);
                    Fiel3.gameObject.transform.GetComponent<CharacterScript>().ForceStats();
                    //Vector3Int pos = new Vector3Int(xPos3, yPos3, 0);
                    //Convert the grid position to a world position (2 is offset because it puts it on the corner of the cell)
                    Vector3 charPos = groundLayer.CellToWorld(pos);
                    charPos.x += 2;
                    charPos.y += 2;
                    Fiel3.transform.position = charPos;
                    //Update the units info
                    Fiel3.GetComponent<CharacterScript>().SetPos(xPos3, yPos3);
                }
                else
                {
                    int xPos4 = Random.Range(0, 8);
                    int yPos4 = Random.Range(-8, 8);
                    //Check if the pos is on a walkable tile
                    Vector3Int pos = new Vector3Int(xPos4, yPos4, 0);
                    TileBase playerTile = groundLayer.GetTile(pos);
                    while (dataFromTiles[playerTile].isWalkable == false)
                    {
                        xPos4 = Random.Range(0, 8);
                        yPos4 = Random.Range(-8, 8);
                        pos = new Vector3Int(xPos4, yPos4, 0);
                        playerTile = groundLayer.GetTile(pos);
                    }
                    Fiel4.SetActive(true);
                    Fiel4.gameObject.transform.GetComponent<CharacterScript>().ForceStats();
                   // Vector3Int pos = new Vector3Int(xPos4, yPos4, 0);
                    //Convert the grid position to a world position (2 is offset because it puts it on the corner of the cell)
                    Vector3 charPos = groundLayer.CellToWorld(pos);
                    charPos.x += 2;
                    charPos.y += 2;
                    Fiel4.transform.position = charPos;
                    //Update the units info
                    Fiel4.GetComponent<CharacterScript>().SetPos(xPos4, yPos4);
                }
            }
        }
    }

    void SpawnPlayerParty()
    {
        //Lief is always a part of the party.
        PlayerUnit.gameObject.transform.GetComponent<CharacterScript>().TrueClass = PlayerStats.instance.Lief;
        SetPlayerPos(PlayerUnit);
        PlayerUnit.SetActive(true);
        //What happens if Lief has no health?

        /*  Temporary Solution: 
         *  Reset his health to 50%, reduce his stats by 5.
         * 
         */

        if (PlayerStats.instance.Lief.CurrHP == 0)           //If Lief was dead
        {
            int tempHealth = PlayerStats.instance.Lief.MaxHP/2;
            PlayerUnit.gameObject.transform.GetComponent<CharacterScript>().CurrHP = tempHealth;    //Reset to half
        }

        for (int i = 0; i < PlayerStats.instance.activeMembers.Count; i++)
        {
            switch (PlayerStats.instance.activeMembers[i])
            {
                case PartyMember.Thief:
                    Thief.SetActive(true);
                    Thief.gameObject.transform.GetComponent<CharacterScript>().TrueClass = PlayerStats.instance.Thief;
                    SetPlayerPos(Thief);
                    if (PlayerStats.instance.Thief.CurrHP == 0)           //If Thief was dead
                    {
                        int tempHealth = PlayerStats.instance.Thief.MaxHP / 4;
                        Thief.gameObject.transform.GetComponent<CharacterScript>().CurrHP = tempHealth;    //Reset to half
                    }
                    break;
                case PartyMember.Archer:
                    Archer.SetActive(true);
                    Archer.gameObject.transform.GetComponent<CharacterScript>().TrueClass = PlayerStats.instance.Archer;
                    SetPlayerPos(Archer);
                    if (PlayerStats.instance.Archer.CurrHP == 0)           //If Archer was dead
                    {
                        int tempHealth = PlayerStats.instance.Archer.MaxHP / 4;
                        Archer.gameObject.transform.GetComponent<CharacterScript>().CurrHP = tempHealth;    //Reset to half
                    }
                    break;
                case PartyMember.Healer:
                    Healer.SetActive(true);
                    Healer.gameObject.transform.GetComponent<CharacterScript>().TrueClass = PlayerStats.instance.Healer;
                    SetPlayerPos(Healer);
                    if (PlayerStats.instance.Healer.CurrHP == 0)           //If Healer was dead
                    {
                        int tempHealth = PlayerStats.instance.Healer.MaxHP / 4;
                        Healer.gameObject.transform.GetComponent<CharacterScript>().CurrHP = tempHealth;    //Reset to half
                    }
                    break;
                case PartyMember.Mage:
                    Mage.SetActive(true);
                    Mage.gameObject.transform.GetComponent<CharacterScript>().TrueClass = PlayerStats.instance.Mage;
                    SetPlayerPos(Mage);
                    if (PlayerStats.instance.Mage.CurrHP == 0)           //If Mage was dead
                    {
                        int tempHealth = PlayerStats.instance.Mage.MaxHP / 4;
                        Mage.gameObject.transform.GetComponent<CharacterScript>().CurrHP = tempHealth;    //Reset to half
                    }
                    break;
                case PartyMember.Warrior:
                    Warrior.SetActive(true);
                    Warrior.gameObject.transform.GetComponent<CharacterScript>().TrueClass = PlayerStats.instance.Warrior;
                    SetPlayerPos(Warrior);
                    if (PlayerStats.instance.Warrior.CurrHP == 0)           //If Warrior was dead
                    {
                        int tempHealth = PlayerStats.instance.Warrior.MaxHP / 4;
                        Warrior.gameObject.transform.GetComponent<CharacterScript>().CurrHP = tempHealth;    //Reset to half
                    }
                    break;
                default: break;
            }
        }
    }

    /*  Function Name:  Set Up Boss
     *  Input:          None, called during boss fights
     *  Output:         Get Loki and activate him and stuff.             */
    void SetUpBoss()
    {
        int xPosL = Random.Range(0, 8);
        int yPosL = Random.Range(-8, 8);
        LokiBoss.SetActive(true);
        LokiBoss.gameObject.transform.GetComponent<CharacterScript>().TrueClass = PlayerStats.instance.Loki;
        LokiBoss.gameObject.transform.GetComponent<CharacterScript>().ForceStats();
        Vector3Int posL = new Vector3Int(xPosL, yPosL, 0);
        TileBase playerTile = groundLayer.GetTile(posL);
        while (dataFromTiles[playerTile].isWalkable == false)
        {
            xPosL = Random.Range(0, 8);
            yPosL = Random.Range(-8, 8);
            posL = new Vector3Int(xPosL, yPosL, 0);
            playerTile = groundLayer.GetTile(posL);
        }
        Vector3 charPosL = groundLayer.CellToWorld(posL);
        charPosL.x += 2;
        charPosL.y += 2;
        LokiBoss.transform.position = charPosL;
        LokiBoss.GetComponent<CharacterScript>().SetPos(xPosL,yPosL);
    }
}
