using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyService : MonoBehaviour
{
    //Si el proyecto se hubiera hecho desde el principio con esta arquitectura, no haría falta obtener el dungeonGenerator y el createNewFloor.
    //Solo haría falta uno de ellos
    [SerializeField]
    private DungeonGenerator dungeonGenerator;
    [SerializeField]
    private CreateNewFloor createNewFloor;
    //Tipos de enemigos a generar
    [SerializeField]
    private List<GameObject> enemies;
    private List<Vector3Int> posInsideRoom = new List<Vector3Int>();

    private float xOffset = 0.5f;
    private float yOffset = 0.5f;

    public void CreateNewFloorEnemies()
    {
        var Enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var enemy in Enemies)
        {
            Destroy(enemy);
        }
        foreach (var room in createNewFloor.roomAndType["EnemiesRoom"])
        {
            float roomSize = room.cellBounds.size.magnitude;
            int enemiesToGenerate = Mathf.RoundToInt(roomSize / 2);
            FillListOfPositions(room); 
            for (int i = 0; i < enemiesToGenerate; i++)
            {
                Vector3 pos = GetRandomPosition(posInsideRoom);
                pos.x += xOffset;
                pos.y += yOffset;
                Instantiate(enemies[Random.Range(0, enemies.Count)], pos, Quaternion.identity);
            }
            posInsideRoom.Clear();
        }
    }

    public void DungeonGeneratorEnemies()
    {
        foreach (var room in dungeonGenerator.roomAndType["EnemiesRoom"])
        {
            float roomSize = room.cellBounds.size.magnitude;
            int enemiesToGenerate = Mathf.RoundToInt(roomSize / 2);  
            FillListOfPositions(room);                 
            for (int i = 0; i < enemiesToGenerate; i++)
            {
                Vector3 pos = GetRandomPosition(posInsideRoom);
                pos.x += xOffset;
                pos.y += yOffset;
                Instantiate(enemies[Random.Range(0, enemies.Count)], pos, Quaternion.identity);
            }
            posInsideRoom.Clear();
        }
    }

    Vector3Int GetRandomPosition(List<Vector3Int> positions)
    {
        return positions[Random.Range(0, positions.Count)];
    }

    void FillListOfPositions(Tilemap room)
    {
        foreach (var pos in room.cellBounds.allPositionsWithin)
        {
            if (room.HasTile(pos))
            {
                posInsideRoom.Add(pos);
            }
        }
    }
}
