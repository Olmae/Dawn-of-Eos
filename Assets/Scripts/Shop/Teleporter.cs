using UnityEngine;

public class Teleporter : MonoBehaviour
{
    // Тэг объекта, к которому нужно телепортироваться
    public string teleportTag;

    // Метод, вызываемый при входе объекта в триггер
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Проверяем, что объект, вошедший в триггер, имеет тэг "Player"
        if (other.CompareTag("Player"))
        {
            // Находим объект с указанным тэгом
            GameObject teleportObject = GameObject.FindGameObjectWithTag(teleportTag);
            
            // Проверяем, найден ли объект с указанным тэгом
            if (teleportObject != null)
            {
                // Получаем позицию телепортационного объекта
                Vector3 teleportPosition = teleportObject.transform.position;
                
                // Смещаем позицию на 10 единиц вниз
                teleportPosition.y -= 2f;
                
                // Телепортируем игрока к новой позиции
                other.transform.position = teleportPosition;
            }
            else
            {
                // Если объект не найден, выводим сообщение об ошибке
                Debug.LogError("Не удалось найти объект с тэгом " + teleportTag + " для телепортации.");
            }
        }
    }
}
