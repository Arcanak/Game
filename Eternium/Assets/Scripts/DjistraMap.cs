using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DjistraMap : MonoBehaviour
{    
    [SerializeField]
    private DungeonGenerator dGen;
    private List<Tilemap> roomsMaps;    
    public Dictionary<Tilemap, double> tilemapValues = new Dictionary<Tilemap, double>();   

    public void AssignTileValues()
    {       
        roomsMaps = new List<Tilemap>(); 
        dGen.gos.ForEach(gObject =>{
            roomsMaps.Add(gObject.GetComponent<Tilemap>());
        });
        roomsMaps.ForEach(room=>{
            Dictionary<Vector3Int, int> posValue = new Dictionary<Vector3Int, int>();
            foreach (var position in room.cellBounds.allPositionsWithin){
                 if(!room.HasTile(position)){
                     continue;
                 }
                posValue.Add(position, (Mathf.Abs(position.x) + Mathf.Abs(position.y)));
             }
            List<int> values = new List<int>(posValue.Values);
            int val = 0;
            foreach(var value in values){
                val += value;
            }
            double mean = val / values.Count;
            tilemapValues.Add(room, mean);            
        });
    }
}
