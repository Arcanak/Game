// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.Tilemaps;

// public class DjistraMap : MonoBehaviour
// {
//     [SerializeField]
//     private Tile djistraTile;
//     [SerializeField]
//     private Tilemap roomTileMap;
//     public List<Vector3Int> tileValue = new List<Vector3Int>();
//     private List<Vector3Int> roomTiles = new List<Vector3Int>();
//     public List<Tilemap> roomsTilemaps = new List<Tilemap>();
//     int number = 0;
//     int index = 0;
//     HashSet<Vector3Int> visited = new HashSet<Vector3Int>();
//     List<HashSet<Vector3Int>> rooms = new List<HashSet<Vector3Int>>();
    

//     public void assignTileValues()
//     {       
//         foreach (var position in roomTileMap.cellBounds.allPositionsWithin)
//         {
//             if (!roomTileMap.HasTile(position))
//             {
//                 continue;
//             }
//             if (visited.Contains(position))
//             {
//                 continue;
//             }
//             if (index == 0)
//             {
//                 rooms.Add(new HashSet<Vector3Int>());//First room
//                 rooms[0].Add(position);
//                 index ++;
//             }
//             visited.Add(position);
//             //Exists
//             //Then position neighbours added to a new list inside 'rooms' list
//             for (int xOffset = -1; xOffset <= 1; xOffset++)
//             {
//                 for (int yOffset = -1; yOffset <= 1; yOffset++)
//                 {
//                     Vector3Int neighbourPos = new Vector3Int(position.x + xOffset, position.y + yOffset, 0);
//                     if (!roomTileMap.HasTile(neighbourPos))
//                     {
//                         continue;
//                     }
//                     foreach (var room in rooms)
//                     {
//                         if (room.Contains(neighbourPos))
//                         {
//                             break;
//                         }
//                         if (!isNeighbour(position, room) && !visited.Contains(position))
//                         {
//                             rooms.Add(new HashSet<Vector3Int>());
//                             rooms[rooms.Count - 1].Add(position);
//                             assignTileValues();
//                         }
//                         room.Add(neighbourPos);
//                     }

//                 }
//             }            
//         }
//         differentiateRooms("room" + number);
//         number ++;
//     }

//     private bool isNeighbour(Vector3Int position, HashSet<Vector3Int> room)
//     {
//         foreach (var pos in room)
//         {
//             for (int xOffset = -1; xOffset <= 1; xOffset++)
//             {
//                 for (int yOffset = -1; yOffset <= 1; yOffset++)
//                 {
//                     if (pos == new Vector3Int(position.x + xOffset, position.y + yOffset, 0))
//                     {
//                         return true;
//                     }
//                 }
//             }
//         }
//         return false;
//     }

    

//     /*private void differentiateRooms(string roomName)
//     {
//         Tilemap room = createTilemap(roomName);
//         foreach (var position in roomTiles)
//         {
//             room.SetTile(position, djistraTile);
//         }
//         roomsTilemaps.Add(room);
//         roomTiles.Clear();
//     }*/
// }
