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
    [SerializeField]
    DjistraMap djistraMap;
    private List<List<Vector3Int>> rooms = new List<List<Vector3Int>>();
    private HashSet<List<Vector3Int>> sameRooms = new HashSet<List<Vector3Int>>();
    public List<GameObject> gos = new List<GameObject>();
    public Dictionary<string, List<Tilemap>> roomAndType = new Dictionary<string, List<Tilemap>>();

    private int finalRooms = 0;

    int count = 0;


    //Contador de las rutas creadas
    private int routeCount = 0;
    private HashSet<List<Vector3Int>> listOfSameRooms = new HashSet<List<Vector3Int>>();
    private HashSet<List<Vector3Int>> listOfMergedRooms = new HashSet<List<Vector3Int>>();

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
        DefineFinalRooms();
        djistraMap.AssignTileValues();
        ClassifyRooms();
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

    //Crea el objeto que va a ser la ahbitación en el juego
    private Tilemap CreateTilemap(string roomName)
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

    /*
        El método recorre todas las posiciones que tienen las habitaciones y, las habitaciones que tengan posiciones conjuntas,
        las une primero en una lista que será alterada después por el método Merge().
        El cual devuelve una lista de habitaciones unidas y después esa lista es añadida al conjunto de habitaciones total.
        Este conjunto de habitaciones total, es recorrido después de que el método se llame de forma recusiva para asegurar
        la correcta unión de las habitaciones.
    */
    private void DefineFinalRooms()
    {
        //Dictionary<string, List<Vector3Int>> roomsWithPos = roomsAsDictionaryNamePositions();

        //Recorrer todas las posiciones en las habitaciones
        foreach (var posList in rooms)
        {
            //Compararlas con las otras posiciones de la lista
            foreach (var posList1 in rooms)
            {
                //Clon para no modificar la referencia
                List<Vector3Int> posListClone = new List<Vector3Int>(posList);
                if (!(posList.All(posList1.Contains) && posList.Count == posList1.Count))
                {
                    //Hace la intersección de donde se juntan las posiciones
                    IEnumerable<Vector3Int> intersection = posListClone.Intersect(posList1);
                    //Si ha encontrado una posición compartida => intersection.Count() > 0
                    if (intersection.Count() > 0)
                    {
                        HashSet<Vector3Int> mergedRooms = new HashSet<Vector3Int>(posListClone);
                        //Une las habitaciones
                        mergedRooms.UnionWith(posList1);
                        bool sameRoom = false;
                        //Recorre la lista de habitaciones unidas para ver si ya está dentro la que se acaba de crear
                        foreach (var room in listOfMergedRooms)
                        {
                            if (room.All(mergedRooms.Contains) && room.Count == mergedRooms.Count)
                            {
                                sameRoom = true;
                                //listOfSameRooms.Add(room);
                                break;
                            }
                        }
                        //Si no está, la añade
                        if (!sameRoom)
                        {
                            listOfMergedRooms.Add(mergedRooms.ToList());
                            //sameRooms.Add(mergedRooms);
                        }
                        break;
                    }
                }
            }
        }

        List<List<Vector3Int>> roomsClone = new List<List<Vector3Int>>(rooms);
        //Conteo de habitaciones antes de mergear
        int roomCount1 = rooms.Count;
        //Recorre la lista de habiaciones unidas para eliminar las habitaciones simples
        //que comparten espacio con las unidas
        foreach (var room in rooms)
        {
            foreach (var mergedRoom in listOfMergedRooms)
            {
                IEnumerable<Vector3Int> intersection = room.Intersect(mergedRoom);
                //&& !(room.All(mergedRoom.Contains) && room.Count == mergedRoom.Count)
                if (intersection.Count() > 0)
                {
                    roomsClone.Remove(room);
                }
            }
        }
        //Junta las habitaciones de la lista listOfMergedRooms
        Merge();
        //Añade las habitaciones a la lista de habitaciones simples
        roomsClone.AddRange(listOfMergedRooms);

        rooms = new List<List<Vector3Int>>(roomsClone);
        //Conteo después del merge
        int roomCount2 = rooms.Count;

        //Si son iguales, significa que no hay cambios
        if (roomCount1 == roomCount2)
        {
            //Crea los GameObjects tilemaps y se les añade unas tiles para delimitar la zona
            for (int i = 0; i < rooms.Count; i++)
            {
                Tilemap room = CreateTilemap("room" + i);
                foreach (var pos in rooms[i])
                {
                    room.SetTile(pos, roomTile);
                }
                //Elimina el espacio en exceso fuera de donde están pintadas las casillas
                room.CompressBounds();
            }
            return;
        }
        //Vuelve a comprobar
        DefineFinalRooms();



    }

    /*
        Junta de forma recursiva las habitaciones de la lista de habitaciones para asegurarse de que no haya
        habitaciones superpuestas
        @Returns listOfMergedRooms lista de habitaciones sin superponer.
    */
    HashSet<List<Vector3Int>> Merge()
    {
        HashSet<List<Vector3Int>> roomsToMergeClone = new HashSet<List<Vector3Int>>();
        Dictionary<List<Vector3Int>, bool> roomHasMerged = new Dictionary<List<Vector3Int>, bool>();
        foreach (var room in listOfMergedRooms)
        {
            roomHasMerged.Add(room, false);
        }
        foreach (var room in listOfMergedRooms)
        {
            foreach (var room1 in listOfMergedRooms)
            {
                if (!(room.All(room1.Contains) && room.Count == room1.Count))
                {
                    if (roomHasMerged[room] == false && roomHasMerged[room1] == false)
                    {
                        IEnumerable<Vector3Int> intersection = room.Intersect(room1);
                        if (intersection.Count() > 0)
                        {
                            HashSet<Vector3Int> newRoom = new HashSet<Vector3Int>(room);
                            newRoom.UnionWith(room1);
                            roomsToMergeClone.Add(newRoom.ToList());
                            roomHasMerged[room] = true;
                            roomHasMerged[room1] = true;
                            break;
                        }
                    }
                }
            }
        }       

        foreach(var room in listOfMergedRooms){
            if(roomHasMerged[room] == false){
                roomsToMergeClone.Add(room);
            }
        }    

        listOfMergedRooms = new HashSet<List<Vector3Int>>(roomsToMergeClone);
        
        

        if (!(listOfMergedRooms.Count == count))
        {
            count = listOfMergedRooms.Count;
            Merge();
        }
        return listOfMergedRooms;
    }

    void ClassifyRooms(){
        string[] roomTypes = new string[] {"SafeRoom", "StairsRoom", "EnemiesRoom", "TreasureRoom"};
        for(int i = 0; i < roomTypes.Length;i++){
            roomAndType.Add(roomTypes[i], new List<Tilemap>());
        }       
        //Linq query
        var sortedDict = from entry in djistraMap.tilemapValues orderby entry.Value ascending select entry;
        Dictionary<Tilemap, double> tilemapValues = new Dictionary<Tilemap, double>(sortedDict);
        int index = 0;
        bool trasureGen = false;        
        foreach(var tuple in tilemapValues){
            if(index == 0){
                roomAndType["SafeRoom"].Add(tuple.Key);
                index++;
                continue;
            }
            if(index == tilemapValues.Count - 1){
                roomAndType["StairsRoom"].Add(tuple.Key);
                index++;
                continue;
            }
            if(!trasureGen){
                if(Random.Range(0, 100) <= 15){
                    roomAndType["TreasureRoom"].Add(tuple.Key);
                    trasureGen = true;
                    index++;
                    continue;
                }
            }
            roomAndType["EnemiesRoom"].Add(tuple.Key);
            index++;
        }
        foreach(var tuple in roomAndType){
            string rooms = "";
            tuple.Value.ForEach(tilemap=>{
                rooms += tilemap.ToString() + " ";
            });
            Debug.Log("Roomtype: " + tuple.Key + ", Values: " + rooms.Trim());
        }

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

    //     listOfMergedRooms.Add(posVisited);
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
    //     listOfMergedRooms.ForEach(room =>
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
