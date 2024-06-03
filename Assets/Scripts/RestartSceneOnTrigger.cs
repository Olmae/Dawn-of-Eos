using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartSceneOnTrigger : MonoBehaviour
{
    // Получаем ссылку на объект с которым сталкивается игрок
    private GameObject playerObject;

    // Перед загрузкой сцены
    void Awake()
    {
        // Не уничтожаем этот объект при перезагрузке сцены
        DontDestroyOnLoad(gameObject);
    }

    // Перед началом столкновения
    void OnTriggerEnter2D(Collider2D other)
    {
        // Проверяем, столкнулся ли игрок
        if (other.CompareTag("Player"))
        {
            // Получаем индекс текущей сцены
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            // Запоминаем игрока
            playerObject = other.gameObject;
            // Перезапускаем сцену
            SceneManager.LoadScene(currentSceneIndex);
        }
    }

    // После загрузки сцены
    void OnLevelWasLoaded(int level)
    {
        // Проверяем, есть ли запомненный игрок
        if (playerObject != null)
        {
            // Помещаем игрока в стартовую позицию
            playerObject.transform.position = transform.position;
        }
    }
}
