using UnityEngine;

public class ITEMS : MonoBehaviour
{
    // Список параметров, которые можно улучшать
    public enum ParameterType
    {
        MaxHealth,
        CurrentHealth,
        RollCooldown,
        MovementSpeed,
        // Добавьте здесь любые другие параметры, которые нужно улучшать
    }

    // Тип улучшаемого параметра
    public ParameterType parameterType;

    // Значение улучшения параметра
    public float improvementValue = 1.1f;

    // Звук подбора предмета
    public AudioClip pickupSound;

    // Цена предмета
    public int price = 10;

    // Объект для включения или выключения
    private bool isInRange = false; // Флаг для отслеживания нахождения игрока в триггере

    private void Update()
    {
        // Если игрок в зоне триггера и нажата клавиша "Е", активируем объект
        if (isInRange && Input.GetKeyDown(KeyCode.E))
        {
            ActivateObject();
        }
    }

    // Метод для активации объекта
    void ActivateObject()
    {
        // Получаем контроллер персонажа
        CharacterController2D characterController = FindObjectOfType<CharacterController2D>();
        PlayerStat playerStat = FindObjectOfType<PlayerStat>();

        // Проверяем, хватает ли у игрока денег на покупку
        if (playerStat != null && playerStat.money >= price)
        {
            // Уменьшаем количество денег у игрока
            playerStat.money -= price;

            // Применяем улучшение параметра в зависимости от выбранного типа
            switch (parameterType)
            {
                case ParameterType.MaxHealth:
                    playerStat.maxHealth *= improvementValue;
                    break;
                case ParameterType.CurrentHealth:
                    playerStat.currentHealth *= improvementValue;
                    break;
                case ParameterType.RollCooldown:
                    characterController.RollCD *= improvementValue;
                    break;
                case ParameterType.MovementSpeed:
                    characterController.MoveSpeed *= improvementValue;
                    break;
                // Добавьте здесь обработку других типов параметров, если необходимо
            }

            // Проигрываем звук подбора предмета
            if (pickupSound != null)
            {
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            }

            // Уничтожаем объект предмета после подбора
            Destroy(gameObject);
        }
        else
        {
            // Если у игрока недостаточно денег, можно вывести сообщение об этом или выполнить другие действия
            Debug.Log("Недостаточно денег для покупки этого предмета.");
        }
    }

    // Обработчик события входа в триггер
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Проверяем, столкнулись ли с объектом персонажа
        if (other.CompareTag("Player"))
        {
            isInRange = true; // Устанавливаем флаг нахождения в триггере
        }
    }

    // Обработчик события выхода из триггера
    private void OnTriggerExit2D(Collider2D other)
    {
        // Проверяем, вышел ли персонаж из зоны триггера
        if (other.CompareTag("Player"))
        {
            isInRange = false; // Сбрасываем флаг нахождения в триггере
        }
    }
}
