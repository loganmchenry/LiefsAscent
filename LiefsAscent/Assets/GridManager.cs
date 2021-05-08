using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
            GameObject player = GameObject.Find("Player");
           // Debug.Log(player.transform.position)
            if (Input.GetMouseButtonDown(0))
            {
                // get mouse click's position in 2d plane
                Vector3 pz = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                pz.z = 0;

                // convert mouse click's position to Grid position
                Tilemap tilemap = GetComponent<Tilemap>();
                Vector3Int cellPosition = tilemap.LocalToCell(pz);
                Debug.Log(tilemap.ToString());
                Vector3 cellCenter = tilemap.GetCellCenterLocal(cellPosition);
                
                // set selectedUnit to clicked location on grid
                player.transform.position = new Vector3(cellCenter.x, cellCenter.y, cellCenter.z);
                Debug.Log(player.transform.position);
                Debug.Log(cellPosition);
            }        
    }

}
