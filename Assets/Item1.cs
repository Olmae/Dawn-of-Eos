using UnityEngine;

public class Item1 : MonoBehaviour
{
    // Список параметров, которые можно улучшать
    public enum ParameterType
    {
        Damage,
        Health,
        Speed,
        RollCooldown // Новый тип параметра для улучшения Cooldown
    }

    // Типы улучшаемых параметров
    public ParameterType[] parameterTypes;

    // Значения улучшения параметров
    public float[] improvementValues;

    // Звук подбора предмета
    public AudioClip pickupSound;

    // Цена предмета
    public int price = 10;

    public Weapon weapon;

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

            // Применяем улучшение параметров в зависимости от выбранных типов
            for (int i = 0; i < parameterTypes.Length; i++)
            {
                switch (parameterTypes[i])
                {
                    case ParameterType.Damage:
                        characterController.attackDamage += improvementValues[i];
                        break;
                    case ParameterType.Health:
                        playerStat.maxHealth += improvementValues[i];
                        break;
                    case ParameterType.Speed:
                        characterController.MoveSpeed += improvementValues[i];
                        break;
                    case ParameterType.RollCooldown:
                        characterController.RollCD -= improvementValues[i]; // Уменьшаем Cooldown на значение improvementValues[i]
                        break;
                    // Добавьте здесь обработку других типов параметров, если необходимо
                }
            }

            // Проигрываем звук подбора предмета
            if (pickupSound != null)
            {
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            }

            // Устанавливаем выбранное оружие в скрипте персонажа
            if (characterController != null)
            {
                characterController.currentWeapon = weapon;

                // Обновляем спрайт оружия у персонажа
                characterController.UpdateWeaponSprite(weapon.weaponSprite);
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
