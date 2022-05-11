using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DjistraMap : MonoBehaviour
{
    [SerializeField]
    private Tilemap groundTileMap;
    private Dictionary<TileBase, int> tileValue = new Dictionary<TileBase, int>();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void assignTileValues()
    {        
        for (int x = groundTileMap.cellBounds.min.x; x < groundTileMap.cellBounds.max.x; x++)
        {
            for (int y = groundTileMap.cellBounds.min.y; y < groundTileMap.cellBounds.max.y; y++)
            {
                TileBase tile =  groundTileMap.GetTile(new Vector3Int(x, y, 0));                

                tileValue.Add(tile, Mathf.Abs(x) + Mathf.Abs(y));               
            }
        }
    }
}
