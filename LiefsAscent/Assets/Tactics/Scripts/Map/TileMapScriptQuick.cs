using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class TileMapScriptQuick : MonoBehaviour
{
    [SerializeField]
    private Tilemap groundLayer;
    [SerializeField]
    private Tilemap playerLayer;
    [SerializeField]
    private Tilemap targetLayer;
    [SerializeField]
    private Tile newTile;

    private bool isCharClicked;
    private TileBase currCharTile;
    private Vector3Int currCharPos;

    // Start is called before the first frame update
    void Start()
    {
        isCharClicked = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int gridPosition = groundLayer.WorldToCell(mousePosition);

            TileBase clickedTile = groundLayer.GetTile(gridPosition);
            print($"{gridPosition}: {clickedTile}");
            TileBase clickedCharacter = playerLayer.GetTile(gridPosition);
            /*
            Vector3Int testMove = gridPosition;
            testMove.y += 1;
            playerLayer.SetTile(testMove, clickedCharacter);
            */
            TileBase clickedTarget = targetLayer.GetTile(gridPosition);
            //print($"Clicked at {gridPosition} there is a {clickedTile}");
            /*
            if (clickedCharacter)
            {
                print($"Also a character here!");
                isCharClicked = true;
                //Store info on the clicked character
                currCharTile = clickedCharacter;
                currCharPos = gridPosition;
                //Make move tiles
                for (int x = -1; x < 3; x++)
                    for (int y = -1; y < 3; y++)
                    {
                        Vector3Int targetSpot = gridPosition;
                        targetSpot.x += x;
                        targetSpot.y += y;
                        targetLayer.SetTile(targetSpot, newTile);
                    }
            }
            else if (isCharClicked)
            {
                if (clickedTarget)
                {
                    Vector3Int movePos = gridPosition;
                    StartCoroutine(Move(currCharTile, currCharPos, movePos));

                }
                isCharClicked = false;
                targetLayer.ClearAllTiles();
            }
            else
            {
                isCharClicked = false;
                targetLayer.ClearAllTiles();
            }
            */
        }
    }

    /*
    IEnumerator Move(TileBase character, Vector3Int currCharPos, Vector3 movePos)
    {
        while (currCharPos.x != movePos.x)
        {
            playerLayer.SetTile(currCharPos, null);
            if (currCharPos.x < movePos.x)
                currCharPos.x += 1;
            else
                currCharPos.x -= 1;
            playerLayer.SetTile(currCharPos, character);
            yield return new WaitForSeconds(2);
        }

        while (currCharPos.y != movePos.y)
        {
            playerLayer.SetTile(currCharPos, null);
            if (currCharPos.y < movePos.y)
                currCharPos.y += 1;
            else
                currCharPos.y -= 1;
            playerLayer.SetTile(currCharPos, character);
            yield return new WaitForSeconds(2);
        }
        //yield return new WaitForSeconds(0);
    }
    */
}

