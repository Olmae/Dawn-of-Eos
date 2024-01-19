using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnInterval = 3f;
    public float spawnRadius = 5f;

    private int spawnedEnemies = 0;
    private bool isSpawning = true;

    private void Start()
    {
        InvokeRepeating("SpawnEnemy", spawnInterval, spawnInterval);
    }

    private void SpawnEnemy()
    {
        if (!isSpawning)
            return;

        if (spawnedEnemies >= 2)
            return;

        Vector2 randomPosition = Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPosition = transform.position + new Vector3(randomPosition.x, 0f, randomPosition.y);

        GameObject enemyObject = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        spawnedEnemies++;

        if (spawnedEnemies >= 2)
            isSpawning = false;
    }

    public void DecreaseSpawnedEnemies()
    {
        spawnedEnemies--;
        if (spawnedEnemies < 2)
            isSpawning = true;
    }
}
