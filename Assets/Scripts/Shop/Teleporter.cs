using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public string teleportTag;
    public GameObject spawnPrefab;
    public Vector3 spawnPositionOffset;
    
    private bool prefabSpawned = false; // Флаг, который отслеживает, был ли заспавнен префаб

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameObject teleportObject = GameObject.FindGameObjectWithTag(teleportTag);
            
            if (teleportObject != null)
            {
                Vector3 teleportPosition = teleportObject.transform.position;
                teleportPosition += spawnPositionOffset; // Добавляем смещение к позиции телепортационного объекта
                spawnPositionOffset.y += 2f;
                teleportPosition.y -= 2f;

                // Телепортация игрока к новой позиции
                other.transform.position = teleportPosition;

                // Проверяем, нужно ли спавнить префаб
                if (spawnPrefab != null && !prefabSpawned)
                {
                    // Спавним префаб на указанной позиции
                    Instantiate(spawnPrefab, teleportPosition, Quaternion.identity);
                    prefabSpawned = true; // Устанавливаем флаг, что префаб заспавнен
                }
            }
            else
            {
                Debug.LogError("Не удалось найти объект с тэгом " + teleportTag + " для телепортации.");
            }
        }
    }

    // Публичный метод для установки префаба спавна
    public void SetSpawnPrefab(GameObject prefab)
    {
        spawnPrefab = prefab;
    }

    // Публичный метод для установки позиции спавна
    public void SetSpawnPosition(Vector3 position)
    {
        spawnPositionOffset = position;
    }
}
