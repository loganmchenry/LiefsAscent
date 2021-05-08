using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class TileMapScript : MonoBehaviour
{
    //!!Riz: Temporarily making this public
    [SerializeField]
    public Tilemap groundLayer;
    [SerializeField]
    private Tilemap targetLayer;
    [SerializeField]
    private Tile newTile;
    [SerializeField]
    private List<TileData> tileDatas;

    //!!Riz: Temporarily making this public
    public Dictionary<TileBase, TileData> tileInformation;

    // Start is called before the first frame update
    // !!Riz: Temporarily making this public
    //void Start()
    public void loadTileDatas()
    {
        //Make the dictionary of tiles
        tileInformation = new Dictionary<TileBase, TileData>();
        foreach (var tileData in tileDatas)
        {
            foreach (var tile in tileData.tiles)
            {
                //print($"{tile.name} : Cost:  : { tileData.movementCost}");
                tileInformation.Add(tile, tileData);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void drawMovement(int maxMovement, Vector3Int characterPosition)
    {
        //If u have movement, keep drawing tiles
        Color color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        Vector3Int gridPosition = characterPosition;
        TileBase currTile;
        if (maxMovement > 0)
        {
            //Draw a tile one up, down, left, right as long as it is walkable
            //Get the tile 1 up
            gridPosition.y = gridPosition.y + 1;
            currTile = groundLayer.GetTile(gridPosition);
            //If its a legal tile, draw it and recursively pass w/ 1 less movement
            if (currTile != null)
            {
                if (tileInformation[currTile].isWalkable)
                {
                    targetLayer.SetTile(gridPosition, newTile);
                    targetLayer.SetTileFlags(gridPosition, TileFlags.None);
                    targetLayer.SetColor(gridPosition, color);
                    drawMovement(maxMovement - 1, gridPosition);
                }
            }
            //Get the tile 1 down
            gridPosition = characterPosition;
            gridPosition.y = gridPosition.y - 1;
            currTile = groundLayer.GetTile(gridPosition);
            //If its a legal tile, draw it and recursively pass w/ 1 less movement
            if (currTile != null)
            {
                if (tileInformation[currTile].isWalkable)
                {
                    targetLayer.SetTile(gridPosition, newTile);
                    targetLayer.SetTileFlags(gridPosition, TileFlags.None);
                    targetLayer.SetColor(gridPosition, color);
                    drawMovement(maxMovement - 1, gridPosition);
                }
            }
            //Get the tile 1 left
            gridPosition = characterPosition;
            gridPosition.x = gridPosition.x - 1;
            currTile = groundLayer.GetTile(gridPosition);
            //If its a legal tile, draw it and recursively pass w/ 1 less movement
            if (currTile != null)
            {
                if (tileInformation[currTile].isWalkable)
                {
                    targetLayer.SetTile(gridPosition, newTile);
                    targetLayer.SetTileFlags(gridPosition, TileFlags.None);
                    targetLayer.SetColor(gridPosition, color);
                    drawMovement(maxMovement - 1, gridPosition);
                }
            }
            //Get the tile 1 right
            gridPosition = characterPosition;
            gridPosition.x = gridPosition.x + 1;
            currTile = groundLayer.GetTile(gridPosition);
            //If its a legal tile, draw it and recursively pass w/ 1 less movement
            if (currTile != null)
            {
                if (tileInformation[currTile].isWalkable)
                {
                    targetLayer.SetTile(gridPosition, newTile);
                    targetLayer.SetTileFlags(gridPosition, TileFlags.None);
                    targetLayer.SetColor(gridPosition, color);
                    drawMovement(maxMovement - 1, gridPosition);
                }
            }
        }
    }
    public Vector3Int GetTilePosition(Vector2 clickedPosition)
    {
        Vector3Int gridPosition = groundLayer.WorldToCell(clickedPosition);
        if (groundLayer.GetTile(gridPosition) != null)
            return gridPosition;

        //return a value with less than non-zero z-value, indicating the click was outside of map
        return new Vector3Int(0,0,-1);
    }
}
