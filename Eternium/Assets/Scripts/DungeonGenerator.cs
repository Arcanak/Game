using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Linq;

public class DungeonGenerator : MonoBehaviour
{
    //Sprites del suelo, vacío y paredes
    [SerializeField]
    private Tile groundTile;
    [SerializeField]
    private Tile pitTile;
    [SerializeField]
    private Tile topWallTile;
    [SerializeField]
    private Tile botWallTile;
    [SerializeField]
    private Tile leftWallTile;
    [SerializeField]
    private Tile rightWallTile;
    [SerializeField]
    private Tile roomTile;
    //Capas del mapa
    [SerializeField]
    private Tilemap groundMap;
    [SerializeField]
    private Tilemap pitMap;
    [SerializeField]
    private Tilemap wallMap;
    [SerializeField]
    private Tilemap roomMap;
    //Probabilidad de que el generador se desvíe por una nueva ruta
    [SerializeField]
    private int deviationRate = 10;
    //Probabilidad de que se genere una habitación
    [SerializeField]
    private int roomRate = 15;
    //Máxima longitud de una ruta
    [SerializeField]
    private int maxRouteLength;
    //Máximo número de rutas que puede hacer el generador
    [SerializeField]
    private int maxRoutes = 20;
    //[SerializeField]
    //DjistraMap djistraMap;
    public List<List<Vector3Int>> rooms = new List<List<Vector3Int>>();
    private List<Tilemap> sameRooms = new List<Tilemap>();
    private List<GameObject> gos = new List<GameObject>();

    private int finalRooms = 0;


    //Contador de las rutas creadas
    private int routeCount = 0;
    private HashSet<List<Vector3Int>> listOfFinalRooms = new HashSet<List<Vector3Int>>();

    private void Start()
    {
        //Posición 0, 0
        int x = 0;
        int y = 0;
        //Se declara la longitud de la ruta inicial
        int routeLength = 0;
        //Generar primer cuadrado con radio 1 en 0, 0
        /*
            Genera 
            [-x,1][0,1][1,1]
            [-x,0][0,0][1,0]
            [-x,-x][0,-x][1,-x]
        */
        GenerateSquare(x, y, 1);
        Vector2Int previousPos = new Vector2Int(x, y);
        y += 3;
        //Se aleja 3 tiles para generar otro cuadrado, creando una especie de pasillo inicial
        GenerateSquare(x, y, 1);
        //Empieza la generación del mundo procedural
        NewRoute(x, y, routeLength, previousPos);
        //Una vez acabada la generación, busca donde rellenar las paredes y el vacío
        FillWalls();
        //djistraMap.assignTileValues();
        DefineFinalRooms();
    }

    private void FillWalls()
    {
        BoundsInt bounds = groundMap.cellBounds;
        for (int xMap = bounds.xMin - 10; xMap <= bounds.xMax + 10; xMap++)
        {
            for (int yMap = bounds.yMin - 10; yMap <= bounds.yMax + 10; yMap++)
            {
                Vector3Int pos = new Vector3Int(xMap, yMap, 0);
                Vector3Int posBelow = new Vector3Int(xMap, yMap - 1, 0);
                Vector3Int posAbove = new Vector3Int(xMap, yMap + 1, 0);
                Vector3Int posRight = new Vector3Int(xMap + 1, yMap, 0);
                Vector3Int posLeft = new Vector3Int(xMap - 1, yMap, 0);
                TileBase tile = groundMap.GetTile(pos);
                TileBase tileBelow = groundMap.GetTile(posBelow);
                TileBase tileAbove = groundMap.GetTile(posAbove);
                TileBase tileRight = groundMap.GetTile(posRight);
                TileBase tileLeft = groundMap.GetTile(posLeft);
                if (tile == null)
                {
                    pitMap.SetTile(pos, pitTile);
                    if (tileBelow != null)
                    {
                        wallMap.SetTile(pos, topWallTile);
                    }
                    else if (tileAbove != null)
                    {
                        wallMap.SetTile(pos, botWallTile);
                    }

                    if (tileRight != null)
                    {
                        wallMap.SetTile(pos, leftWallTile);
                    }
                    else if (tileLeft != null)
                    {
                        wallMap.SetTile(pos, rightWallTile);
                    }
                }
            }
        }
    }

    private void NewRoute(int x, int y, int routeLength, Vector2Int previousPos)
    {
        if (routeCount < maxRoutes)
        {
            routeCount++;
            while (++routeLength < maxRouteLength)
            {
                //Initialize
                bool routeUsed = false;
                int xOffset = x - previousPos.x; //0
                int yOffset = y - previousPos.y; //3
                int roomSize = 1; //Hallway size
                //Probabilidad de generar una habitación
                //TODO: asignar propiedades a las habitaciones
                if (Random.Range(1, 100) <= roomRate)
                    roomSize = Random.Range(3, 6);
                previousPos = new Vector2Int(x, y);

                //Go Straight
                if (Random.Range(1, 100) <= deviationRate)
                {
                    if (routeUsed)
                    {
                        GenerateSquare(previousPos.x + xOffset, previousPos.y + yOffset, roomSize);
                        NewRoute(previousPos.x + xOffset, previousPos.y + yOffset, Random.Range(routeLength, maxRouteLength), previousPos);
                    }
                    else
                    {
                        x = previousPos.x + xOffset;
                        y = previousPos.y + yOffset;
                        GenerateSquare(x, y, roomSize);
                        routeUsed = true;
                    }
                }

                //Go left
                if (Random.Range(1, 100) <= deviationRate)
                {
                    if (routeUsed)
                    {
                        GenerateSquare(previousPos.x - yOffset, previousPos.y + xOffset, roomSize);
                        NewRoute(previousPos.x - yOffset, previousPos.y + xOffset, Random.Range(routeLength, maxRouteLength), previousPos);
                    }
                    else
                    {
                        y = previousPos.y + xOffset;
                        x = previousPos.x - yOffset;
                        GenerateSquare(x, y, roomSize);
                        routeUsed = true;
                    }
                }
                //Go right
                if (Random.Range(1, 100) <= deviationRate)
                {
                    if (routeUsed)
                    {
                        GenerateSquare(previousPos.x + yOffset, previousPos.y - xOffset, roomSize);
                        NewRoute(previousPos.x + yOffset, previousPos.y - xOffset, Random.Range(routeLength, maxRouteLength), previousPos);
                    }
                    else
                    {
                        y = previousPos.y - xOffset;
                        x = previousPos.x + yOffset;
                        GenerateSquare(x, y, roomSize);
                        routeUsed = true;
                    }
                }

                if (!routeUsed)
                {
                    x = previousPos.x + xOffset;
                    y = previousPos.y + yOffset;
                    GenerateSquare(x, y, roomSize);
                }
            }
        }
    }

    /*
        Genera 
        [-x,1][0,1][1,1]
        [-x,0][0,0][1,0]
        [-x,-x][0,-x][1,-x]
    */
    //Nueva posibilidad: Random tiles para algo de cambio en lo visual
    private void GenerateSquare(int x, int y, int radius)
    {
        //TODO if rooms share tiles, merge them in a new array that will we the final version of the map instance rooms     
        List<Vector3Int> room = new List<Vector3Int>();
        for (int tileX = x - radius; tileX <= x + radius; tileX++)
        {
            for (int tileY = y - radius; tileY <= y + radius; tileY++)
            {
                Vector3Int tilePos = new Vector3Int(tileX, tileY, 0);
                groundMap.SetTile(tilePos, groundTile);
                if (radius > 1)
                {
                    //rooms[roomNum - 1].SetTile(tilePos, roomTile); 
                    room.Add(tilePos);
                }
            }
        }
        if (radius > 1)
        {
            rooms.Add(room);
        }

    }

    private Tilemap createTilemap(string roomName)
    {
        var go = new GameObject(roomName);
        gos.Add(go);
        var room = go.AddComponent<Tilemap>();
        var tr = go.AddComponent<TilemapRenderer>();

        room.tileAnchor = new Vector3(0, 0, 0);
        go.transform.SetParent(roomMap.transform);
        tr.sortingLayerName = "Main";
        return room;
    }

    private void DefineFinalRooms()
    {
        //Dictionary<string, List<Vector3Int>> roomsWithPos = roomsAsDictionaryNamePositions();
        foreach (var posList in rooms)
        {
            foreach (var posList1 in rooms)
            {
                List<Vector3Int> posListClone = new List<Vector3Int>(posList);
                if (!(posList.Equals(posList1)))
                {
                    IEnumerable<Vector3Int> intersection = posListClone.Intersect(posList1);
                    if (intersection.Count() > 0)
                    {
                        List<Vector3Int> mergedRooms = new List<Vector3Int>(posListClone);
                        mergedRooms.AddRange(posList1);
                        bool sameRoom = false;
                        foreach (var room in listOfFinalRooms)
                        {
                            room.Sort((a, b) => a.x.CompareTo(b.x));
                            mergedRooms.Sort((a, b) => a.x.CompareTo(b.x));
                            if (room.SequenceEqual(mergedRooms))
                            {
                                sameRoom = true;
                                break;
                            }
                        }
                        if (!sameRoom)
                        {
                            listOfFinalRooms.Add(mergedRooms);
                        }
                        break;
                    }
                }
            }
        }
        rooms.Clear();
        rooms.AddRange(listOfFinalRooms);
        if (listOfFinalRooms.Count == finalRooms)
        {
            for (int i = 0; i < rooms.Count; i++)
            {
                Tilemap room = createTilemap("room" + i);
                foreach (var pos in rooms[i])
                {
                    room.SetTile(pos, roomTile);
                }
            }
            return;
        }
        finalRooms = listOfFinalRooms.Count;
        DefineFinalRooms();
    }

    // Dictionary<string, List<Vector3Int>> roomsAsDictionaryNamePositions(){
    //     Dictionary<string, List<Vector3Int>> roomsAndPositions = new Dictionary<string, List<Vector3Int>>();        
    //     int roomNumber = 0;
    //     string roomName = "room" + roomNumber;
    //     rooms.ForEach(room =>{
    //         List<Vector3Int> posRom = new List<Vector3Int>();
    //         foreach (var position in room.cellBounds.allPositionsWithin){
    //             if(room.HasTile(position)){
    //                 posRom.Add(position);
    //             }
    //         }
    //         roomsAndPositions.Add(roomName, posRom);
    //         roomNumber++;
    //     });
    //     return roomsAndPositions;
    // }

    // List<Vector3Int> posVisited = new List<Vector3Int>();    
    // bool sameRoom;
    // bool anyRoomMerged = false;
    // rooms.ForEach(room =>
    // {
    //     rooms.ForEach(room1 =>
    //     {
    //         List<Vector3Int> newRoom = new List<Vector3Int>();
    //         sameRoom = false;
    //         if (!room.Equals(room1) && !(sameRooms.Contains(room) || sameRooms.Contains(room1)))
    //         {
    //             //Iterate over all positions
    //             foreach (var position in room.cellBounds.allPositionsWithin)
    //             {
    //                 if (room1.HasTile(position))
    //                 {
    //                     sameRoom = true;
    //                     anyRoomMerged = true;
    //                     foreach (var position1 in room1.cellBounds.allPositionsWithin)
    //                     {
    //                         newRoom.Add(position1);
    //                     }
    //                 }
    //                 posVisited.Add(position);
    //             }
    //         }

    //         if (sameRoom)
    //         {
    //             posVisited.ForEach(pos =>
    //             {
    //                 newRoom.Add(pos);
    //             });                    
    //             listOfRoomsCreated.Add(newRoom);
    //             sameRooms.Add(room);
    //             sameRooms.Add(room1);
    //             newRoom.Clear();
    //         }
    //         posVisited.Clear();
    //     });

    // });
    // rooms.ForEach(room =>
    // {
    //     posVisited.Clear();
    //     foreach (var position in room.cellBounds.allPositionsWithin)
    //     {
    //         posVisited.Add(position);
    //     }

    //     listOfFinalRooms.Add(posVisited);
    // });

    // if (anyRoomMerged)
    // {
    //     roomNum = 0;
    //     for(int i = gos.Count - 1; i >=0; i--){
    //         GameObject.Destroy(gos[i]);
    //     }
    //     gos.Clear();
    //     rooms.Clear();
    //     listOfRoomsCreated.ForEach(room =>
    //     {
    //         rooms.Add(createTilemap("room" + roomNum));
    //         roomNum++;
    //         foreach (var position in room)
    //         {
    //             rooms[roomNum - 1].SetTile(position, roomTile);
    //         }
    //     });
    //     sameRooms.Clear();
    //     DefineFinalRooms();
    // }
    // else
    // {
    //     roomNum = 0;
    //     for(int i = gos.Count - 1; i >=0; i--){
    //         GameObject.Destroy(gos[i]);
    //     }
    //     gos.Clear();
    //     rooms.Clear();
    //     listOfFinalRooms.ForEach(room =>
    //     {
    //         rooms.Add(createTilemap("room" + roomNum));
    //         roomNum++;
    //         foreach (var position in room)
    //         {
    //             rooms[roomNum - 1].SetTile(position, roomTile);
    //         }
    //     });

    // }


}
