using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawn : MonoBehaviour
{
    public GameObject[] spawnPoints; // массив точек спавна
    public GameObject[] prefabs; // массив префабов предметов

    void Start()
    {
        SpawnRandomObjects();
    }

    void SpawnRandomObjects()
    {
        // Создаем список индексов точек спавна
        List<int> spawnIndexes = new List<int>();
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            spawnIndexes.Add(i);
        }

        // Перемешиваем список индексов
        for (int i = 0; i < spawnIndexes.Count; i++)
        {
            int temp = spawnIndexes[i];
            int randomIndex = Random.Range(i, spawnIndexes.Count);
            spawnIndexes[i] = spawnIndexes[randomIndex];
            spawnIndexes[randomIndex] = temp;
        }

        // Создаем список индексов префабов
        List<int> prefabIndexes = new List<int>();
        for (int i = 0; i < prefabs.Length; i++)
        {
            prefabIndexes.Add(i);
        }

        // Перемешиваем список индексов префабов
        for (int i = 0; i < prefabIndexes.Count; i++)
        {
            int temp = prefabIndexes[i];
            int randomIndex = Random.Range(i, prefabIndexes.Count);
            prefabIndexes[i] = prefabIndexes[randomIndex];
            prefabIndexes[randomIndex] = temp;
        }

        // Спавним случайные предметы на случайных точках
        int spawnCount = Mathf.Min(spawnPoints.Length, prefabs.Length);
        for (int i = 0; i < spawnCount; i++)
        {
            int spawnIndex = spawnIndexes[i];
            GameObject spawnPoint = spawnPoints[spawnIndex];

            int prefabIndex = prefabIndexes[i];
            GameObject prefab = prefabs[prefabIndex];

            Instantiate(prefab, spawnPoint.transform.position, Quaternion.identity);
        }
    }
}
