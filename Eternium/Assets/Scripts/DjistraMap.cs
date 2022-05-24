using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DjistraMap : MonoBehaviour
{
    [SerializeField]
    private Tile djistraTile;
    [SerializeField]
    private DungeonGenerator dGen;
    private List<Tilemap> roomsMaps;
    private Dictionary<Tilemap, double> tilemapValues;
    
    
    

    public void assignTileValues()
    {       
        roomsMaps = new List<Tilemap>(dGen.gos); 
        roomsMaps.ForEach(room=>{
            BoundsInt bounds = room.cellBounds.allPositionsWithin;

            foreach (var position in bounds){
                
            }
        });
        
    }
}
