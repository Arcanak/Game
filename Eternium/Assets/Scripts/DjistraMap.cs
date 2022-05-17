using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DjistraMap : MonoBehaviour
{
    [SerializeField]
    private Tile djistraTile;
    [SerializeField]
    private Tilemap roomTileMap;
    public List<Vector3Int> tileValue = new List<Vector3Int>();
    private List<Vector3Int> roomTiles = new List<Vector3Int>();
    public List<Tilemap> rooms = new List<Tilemap>();

    public void assignTileValues()
    {
        int number = 0;
        foreach (var position in roomTileMap.cellBounds.allPositionsWithin)
        {
            if (!roomTileMap.HasTile(position))
            {
                continue;
            }
            TileBase tile = roomTileMap.GetTile(position);
            if (tileValue.Count > 0)
            {
                Vector3Int lastPos = tileValue[tileValue.Count - 1];
                if ((position.x > lastPos.x + 1) && (position.y > lastPos.y - 1))
                {
                    string roomName = "room" + number;                    
                    differentiateRooms(roomName);
                    number++;
                }
                roomTiles.Add(position);
            }
            tileValue.Add(position);
            //djistraTileMap.SetTile(position, djistraTile);           
        }
    }

    private Tilemap createTilemap(string roomName)
    {
        var go = new GameObject();
        var room = go.AddComponent<Tilemap>();
        var tr = go.AddComponent<TilemapRenderer>();

        room.tileAnchor = new Vector3(0, 0, 0);
        go.transform.SetParent(roomTileMap.transform);
        tr.sortingLayerName = "Main";
        return room;
    }

    private void differentiateRooms(string roomName)
    {
        Tilemap room = createTilemap(roomName);
        foreach (var position in roomTiles)
        {
            room.SetTile(position, djistraTile);
        }
        rooms.Add(room);
        roomTiles.Clear();
    }
}
