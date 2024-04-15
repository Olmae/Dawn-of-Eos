using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomFirstDungeonGenerator : SimpleRandomWalkDungeonGenerator
{
    [SerializeField]
    private int minRoomWidth = 8, minRoomHeight = 8;
    [SerializeField]
    private int dungeonWidth = 20, dungeonHeight = 20;
    [SerializeField]
    private int offset = 1;
    [SerializeField]
    private bool randomWalkRooms = false;
    [SerializeField]
    private GameObject firstRoomPrefab;
    [SerializeField]
    private GameObject lightPrefab; // Префаб света
    [SerializeField]
    private GameObject secondRoomPrefab; // Второй префаб для появления в случайной комнате
    [SerializeField]
    private GameObject thirdRoomPrefab; // Третий префаб для появления в случайной комнате
    [SerializeField]
    private GameObject boxPrefab; // Префаб коробки

    [SerializeField]
    [Range(0f, 1f)]
    private float boxSpawnChance = 0.5f; // Вероятность появления коробки

    [SerializeField]
    private int minBoxCount = 20; // Минимальное количество коробок
    [SerializeField]
    private int maxBoxCount = 30; // Максимальное количество коробок

    [SerializeField]
    private Tilemap excludedTilemap; // Исключаемая тайлмапа для спавна

    private bool isFirstRoomCreated = false;
    private HashSet<Vector2Int> floor = new HashSet<Vector2Int>(); // Пол комнаты
    private List<Vector2Int> roomCenters = new List<Vector2Int>(); // Центры комнат

    private void Start()
    {
        // Удаляем старую локацию, если она существует
        RemoveOldLocation();
        
        // Запускаем процесс создания новой локации
        RunProceduralGeneration();
    }

    private void RemoveOldLocation()
    {
        // Удаляем все дочерние объекты текущего объекта
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    protected override void RunProceduralGeneration()
    {
        CreateRooms();
    }

    private void CreateRooms()
    {
        var roomsList = ProceduralGenerationAlgorithms.BinarySpacePartitioning(new BoundsInt((Vector3Int)startPosition, new Vector3Int(dungeonWidth, dungeonHeight, 0)), minRoomWidth, minRoomHeight);

        floor.Clear(); // Очищаем пол комнаты
        roomCenters.Clear(); // Очищаем список центров комнат

        if (randomWalkRooms)
        {
            floor = CreateRoomsRandomly(roomsList);
        }
        else
        {
            floor = CreateSimpleRooms(roomsList);
        }
        
        foreach (var room in roomsList)
        {
            roomCenters.Add((Vector2Int)Vector3Int.RoundToInt(room.center));
        }

        // Создаем световые префабы на каждой локации
        foreach (var center in roomCenters)
        {
            Instantiate(lightPrefab, new Vector3(center.x, center.y, -2), Quaternion.identity);
        }

        if (!isFirstRoomCreated && firstRoomPrefab != null && roomCenters.Count > 0)
        {
            Vector2Int firstRoomCenter = roomCenters[0];
            Instantiate(firstRoomPrefab, new Vector3(firstRoomCenter.x, firstRoomCenter.y, 0), Quaternion.identity);
            isFirstRoomCreated = true;
        }

        // Спавним второй префаб в случайной комнате, исключая первую комнату
        if (secondRoomPrefab != null && roomCenters.Count > 1)
        {
            // Выбираем случайную комнату для спавна второго префаба
            int randomRoomIndex = Random.Range(1, roomCenters.Count);
            Vector2Int randomRoomCenter = roomCenters[randomRoomIndex];
            Instantiate(secondRoomPrefab, new Vector3(randomRoomCenter.x, randomRoomCenter.y, 0), Quaternion.identity);

            // Удаляем выбранную комнату из списка, чтобы она не использовалась для третьего префаба
            roomCenters.RemoveAt(randomRoomIndex);
        }

        // Спавним третий префаб в случайной комнате, исключая первую и вторую комнаты
        if (thirdRoomPrefab != null && roomCenters.Count > 1)
        {
            Vector2Int randomRoomCenter = roomCenters[Random.Range(1, roomCenters.Count)];
            Instantiate(thirdRoomPrefab, new Vector3(randomRoomCenter.x, randomRoomCenter.y, 0), Quaternion.identity);
        }

        // Рандомный спавн коробок в случайных комнатах
        SpawnBoxesRandomly(roomCenters);

        HashSet<Vector2Int> corridors = ConnectRooms(roomCenters);
        floor.UnionWith(corridors);

        tilemapVisualizer.PaintFloorTiles(floor);
        WallGenerator.CreateWalls(floor, tilemapVisualizer);
    }

    private void SpawnBoxesRandomly(List<Vector2Int> roomCenters)
    {
        int boxCount = Random.Range(minBoxCount, maxBoxCount + 1); // Случайное количество коробок
        foreach (Vector2Int center in roomCenters)
        {
            // Проверяем, что центр комнаты не находится на исключенной тайлмапе
            if (!IsTileInExcludedTilemap(center))
            {
                // Рандомное решение о спавне коробки
                if (Random.Range(0f, 1f) < boxSpawnChance && boxCount > 0)
                {
                    Vector2 spawnPosition = center + new Vector2(Random.Range(-minRoomWidth / 2, minRoomWidth / 2), Random.Range(-minRoomHeight / 2, minRoomHeight / 2));
                    Instantiate(boxPrefab, new Vector3(spawnPosition.x, spawnPosition.y, 0), Quaternion.identity);
                    boxCount--;
                }
            }
        }
    }

    private bool IsTileInExcludedTilemap(Vector2Int tilePosition)
    {
        // Получаем тайл по позиции
        TileBase tile = excludedTilemap.GetTile((Vector3Int)tilePosition);
        // Если тайл не равен null, значит позиция находится на исключенной тайлмапе
        return tile != null;
    }

    // Остальные методы остаются без изменений

    private HashSet<Vector2Int> ConnectRooms(List<Vector2Int> roomCenters)
    {
        HashSet<Vector2Int> corridors = new HashSet<Vector2Int>();
        var currentRoomCenter = roomCenters[Random.Range(0, roomCenters.Count)];
        roomCenters.Remove(currentRoomCenter);

        while (roomCenters.Count > 0)
        {
            Vector2Int closest = FindClosestPointTo(currentRoomCenter, roomCenters);
            roomCenters.Remove(closest);
            HashSet<Vector2Int> newCorridor = CreateCorridor(currentRoomCenter, closest);

            foreach (var point in newCorridor)
            {
                corridors.Add(point);
                corridors.Add(point + new Vector2Int(0, 1));

                if (point.x == closest.x)
                {
                    corridors.Add(point + new Vector2Int(1, 0));
                    corridors.Add(point + new Vector2Int(-1, 0));
                }
            }

            // Создаем коридор к новой комнате
            HashSet<Vector2Int> corridorToRoom = CreateCorridor(currentRoomCenter, closest);
            corridors.UnionWith(corridorToRoom);

            currentRoomCenter = closest;
        }
        return corridors;
    }

    private HashSet<Vector2Int> CreateCorridor(Vector2Int currentRoomCenter, Vector2Int destination)
    {
        HashSet<Vector2Int> corridor = new HashSet<Vector2Int>();
        var position = currentRoomCenter;
        corridor.Add(position);
        while (position.y != destination.y)
        {
            if(destination.y > position.y)
            {
                position += Vector2Int.up;
            }
            else if(destination.y < position.y)
            {
                position += Vector2Int.down;
            }
            corridor.Add(position);
        }
        while (position.x != destination.x)
        {
            if (destination.x > position.x)
            {
                position += Vector2Int.right;
            }
            else if(destination.x < position.x)
            {
                position += Vector2Int.left;
            }
            corridor.Add(position);
        }
        return corridor;
    }

    private Vector2Int FindClosestPointTo(Vector2Int currentRoomCenter, List<Vector2Int> roomCenters)
    {
        Vector2Int closest = Vector2Int.zero;
        float distance = float.MaxValue;
        foreach (var position in roomCenters)
        {
            float currentDistance = Vector2.Distance(position, currentRoomCenter);
            if(currentDistance < distance)
            {
                distance = currentDistance;
                closest = position;
            }
        }
        return closest;
    }

    private HashSet<Vector2Int> CreateSimpleRooms(List<BoundsInt> roomsList)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        foreach (var room in roomsList)
        {
            for (int col = offset; col < room.size.x - offset; col++)
            {
                for (int row = offset; row < room.size.y - offset; row++)
                {
                    Vector2Int position = (Vector2Int)room.min + new Vector2Int(col, row);
                    floor.Add(position);
                }
            }
        }
        return floor;
    }

    private HashSet<Vector2Int> CreateRoomsRandomly(List<BoundsInt> roomsList)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        for (int i = 0; i < roomsList.Count; i++)
        {
            var roomBounds = roomsList[i];
            var roomCenter = new Vector2Int(Mathf.FloorToInt(roomBounds.center.x), Mathf.FloorToInt(roomBounds.center.y));
            var roomFloor = RunRandomWalk(randomWalkParameters, roomCenter);
            foreach (var position in roomFloor)
            {
                if(position.x >= (roomBounds.xMin + offset) && position.x <= (roomBounds.xMax - offset) && position.y >= (roomBounds.yMin - offset) && position.y <= (roomBounds.yMax - offset))
                {
                    floor.Add(position);
                }
            }
        }
        return floor;
    }
}
