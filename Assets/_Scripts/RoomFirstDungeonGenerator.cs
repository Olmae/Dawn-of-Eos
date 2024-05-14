using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomFirstDungeonGenerator : SimpleRandomWalkDungeonGenerator
{
    [Header("Room Settings")]
    [SerializeField] private int minRoomWidth = 8;
    [SerializeField] private int minRoomHeight = 8;
    [SerializeField] private int dungeonWidth = 20;
    [SerializeField] private int dungeonHeight = 20;
    [SerializeField] private int offset = 1;
    [SerializeField] private bool randomWalkRooms = false;

    [Header("Prefabs")]
    [SerializeField] private GameObject firstRoomPrefab;
    [SerializeField] private GameObject lightPrefab;
    [SerializeField] private GameObject secondRoomPrefab;
    [SerializeField] private GameObject thirdRoomPrefab;
    [SerializeField] private GameObject boxPrefab;
    [SerializeField] private List<GameObject> enemyPrefabs;
    [SerializeField] private int enemyCount = 10;
    private Vector2Int playerPosition; // Добавь эту переменную для хранения позиции игрока
    private float enemySpawnChance = 0.5f; // Пример значения вероятности спавна врагов\
    public int desiredEnemyCount = 10;
    

    [Header("Spawn Settings")]
    [SerializeField] [Range(0f, 1f)] private float boxSpawnChance = 0.5f;
    [SerializeField] private int minBoxCount = 20;
    [SerializeField] private int maxBoxCount = 30;

    [Header("Tilemaps")]
    [SerializeField] private Tilemap excludedTilemap;

    private bool isFirstRoomCreated = false;
    private HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
    private List<Vector2Int> roomCenters = new List<Vector2Int>();
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


    SetPlayerPosition(playerPosition);
    SetEnemySpawnChance(enemySpawnChance);
    // Создаем пустой объект для световых префабов
    GameObject lightsContainer = new GameObject("Lights");
    lightsContainer.transform.SetParent(transform);

    // Создаем пустой объект для коробок
    GameObject boxesContainer = new GameObject("Boxes");
    boxesContainer.transform.SetParent(transform);

    // Создаем пустой объект для врагов
    GameObject enemiesContainer = new GameObject("Enemies");
    enemiesContainer.transform.SetParent(transform);
    SpawnEnemiesRandomly(roomCenters, enemiesContainer, desiredEnemyCount);

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

    // Создаем световые префабы на каждой локации и устанавливаем их родительский объект
    foreach (var center in roomCenters)
    {
        GameObject light = Instantiate(lightPrefab, new Vector3(center.x, center.y, -2), Quaternion.identity);
        light.transform.SetParent(lightsContainer.transform);
    }

    if (!isFirstRoomCreated && firstRoomPrefab != null && roomCenters.Count > 0)
    {
        Vector2Int firstRoomCenter = roomCenters[0];
        Instantiate(firstRoomPrefab, new Vector3(firstRoomCenter.x, firstRoomCenter.y, 0), Quaternion.identity, transform);
        isFirstRoomCreated = true;
    }

    // Спавним второй префаб в случайной комнате, исключая первую комнату
    if (secondRoomPrefab != null && roomCenters.Count > 1)
    {
        // Выбираем случайную комнату для спавна второго префаба
        int randomRoomIndex = Random.Range(1, roomCenters.Count);
        Vector2Int randomRoomCenter = roomCenters[randomRoomIndex];
        Instantiate(secondRoomPrefab, new Vector3(randomRoomCenter.x, randomRoomCenter.y, 0), Quaternion.identity, transform);

        // Удаляем выбранную комнату из списка, чтобы она не использовалась для третьего префаба
        roomCenters.RemoveAt(randomRoomIndex);
    }

    // Спавним третий префаб в случайной комнате, исключая первую и вторую комнаты
    if (thirdRoomPrefab != null && roomCenters.Count > 1)
    {
        Vector2Int randomRoomCenter = roomCenters[Random.Range(1, roomCenters.Count)];
        Instantiate(thirdRoomPrefab, new Vector3(randomRoomCenter.x, randomRoomCenter.y, 0), Quaternion.identity, transform);
    }

    // Рандомный спавн коробок в случайных комнатах и устанавливаем их родительский объект
    SpawnBoxesRandomly(roomCenters, boxesContainer);

    // Рандомный спавн врагов в случайных комнатах и устанавливаем их родительский объект
SpawnEnemiesRandomly(roomCenters, enemiesContainer, desiredEnemyCount);

    HashSet<Vector2Int> corridors = ConnectRooms(roomCenters);
    floor.UnionWith(corridors);

    tilemapVisualizer.PaintFloorTiles(floor);
    WallGenerator.CreateWalls(floor, tilemapVisualizer);
}



    // Метод для установки позиции игрока
    public void SetPlayerPosition(Vector2Int position)
    {
        playerPosition = position;
    }

    // Метод для установки вероятности спавна врагов
    public void SetEnemySpawnChance(float chance)
    {
        enemySpawnChance = chance;
    }
private Vector2Int GetRandomRoomCenter(Vector2Int originalCenter, int maxOffset)
{
    // Генерируем случайное смещение в пределах maxOffset
    int offsetX = Random.Range(-maxOffset, maxOffset + 1);
    int offsetY = Random.Range(-maxOffset, maxOffset + 1);

    // Добавляем смещение к оригинальному центру комнаты
    Vector2Int newCenter = originalCenter + new Vector2Int(offsetX, offsetY);

    return newCenter;
}

private void SpawnEnemiesRandomly(List<Vector2Int> roomCenters, GameObject enemiesContainer, int desiredEnemyCount)
{
    foreach (Vector2Int center in roomCenters)
    {
        // Проверяем, что центр комнаты не находится на исключенной тайлмапе
        if (!IsTileInExcludedTilemap(center))
        {
            // Проверяем, что центр комнаты не совпадает с позицией игрока
            if (center != playerPosition)
            {
                // Проверяем, что центр комнаты не совпадает с центром уже созданной комнаты
                bool isCenterValid = true;
                foreach (Vector2Int existingCenter in roomCenters)
                {
                    if (center == existingCenter && center != existingCenter) // Тут ошибка, нужно убрать второе условие
                    {
                        isCenterValid = false;
                        break;
                    }
                }

                if (isCenterValid)
                {
                    // Рандомное решение о спавне врага
                    if (Random.Range(0f, 1f) < enemySpawnChance)
                    {
                        // Генерируем позицию для спавна врага в пределах комнаты
                        Vector2 spawnPosition = GetRandomSpawnPosition(center);
                        if (spawnPosition != Vector2.zero)
                        {
                            GameObject enemyPrefab = GetRandomEnemyPrefab();
                            if (enemyPrefab != null)
                            {
                                GameObject enemy = Instantiate(enemyPrefab, new Vector3(spawnPosition.x, spawnPosition.y, 0), Quaternion.identity);
                                enemy.transform.SetParent(enemiesContainer.transform);
                            }
                        }
                        else
                        {
                            Debug.LogError("Unable to find valid spawn position for enemy in room at: " + center);
                        }
                    }
                }
                else
                {
                    Debug.LogError("Center of room at: " + center + " is not valid for enemy spawn due to existing room.");
                }
            }
            else
            {
                Debug.LogError("Player position matches center of room at: " + center + ". Skipping enemy spawn in this room.");
            }
        }
        else
        {
            Debug.LogError("Center of room at: " + center + " is on excluded tilemap. Skipping enemy spawn in this room.");
        }
    }
}

private Vector2 GetRandomSpawnPosition(Vector2Int roomCenter)
{
    Vector2 spawnPosition;
    bool isValidPosition = false;
    int maxAttempts = 10;
    int attemptCount = 0;

    do
    {
        // Генерируем случайные координаты в пределах комнаты
        spawnPosition = roomCenter + new Vector2(Random.Range(-minRoomWidth / 2, minRoomWidth / 2), Random.Range(-minRoomHeight / 2, minRoomHeight / 2));

        // Проверяем, что позиция не находится в центре комнаты
        if (Vector2Int.RoundToInt(spawnPosition) != roomCenter)
        {
            isValidPosition = true;
        }

        attemptCount++;
    } while (!isValidPosition && attemptCount < maxAttempts);

    if (!isValidPosition)
    {
        return Vector2.zero;
    }

    return spawnPosition;
}

private GameObject GetRandomEnemyPrefab()
{
    if (enemyPrefabs != null && enemyPrefabs.Count > 0)
    {
        return enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
    }
    return null;
}
private void SpawnBoxesRandomly(List<Vector2Int> roomCenters, GameObject boxesContainer)
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
                GameObject box = Instantiate(boxPrefab, new Vector3(spawnPosition.x, spawnPosition.y, 0), Quaternion.identity);
                box.transform.SetParent(boxesContainer.transform);
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
    Vector2Int position = currentRoomCenter;

    // Продвигаемся на один тайл в направлении Y
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

    // Продвигаемся на один тайл в направлении X
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
                var roomCenter = GetRandomRoomCenter(new Vector2Int(Mathf.FloorToInt(room.center.x), Mathf.FloorToInt(room.center.y)), 2);

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
