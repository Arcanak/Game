using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Linq;


public class RoomGenerator : MonoBehaviour
{
    [SerializeField]
    DungeonGenerator dungeonGenerator;  
    [SerializeField]  
    private GameObject upStairs;
    [SerializeField]
    private GameObject downStairs;





    private float xOffset = 0.5f;
    private float yOffset = 0.5f;
    

    public void GenerateStairs()
    {        
        Dictionary<string, List<Tilemap>> roomAndType = dungeonGenerator.roomAndType;
        foreach (var room in roomAndType["StairsRoom"])
        {
            Vector3 pos = GetRandomPosition(room.cellBounds);
            pos.x += xOffset; 
            pos.y += yOffset;
            Instantiate(downStairs, pos, Quaternion.identity);
        }
    }

    Vector3Int GetRandomPosition(BoundsInt bounds)
    {
        return new Vector3Int(Random.Range(bounds.xMin, bounds.xMax), Random.Range(bounds.yMin, bounds.yMax), 0);
    }
}