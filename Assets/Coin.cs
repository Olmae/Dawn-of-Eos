using UnityEngine;

public class Coin : MonoBehaviour
{
    public int value = 2; // Значение монеты

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Получаем компонент PlayerStat у игрока
            PlayerStat playerStat = other.GetComponent<PlayerStat>();
            if (playerStat != null)
            {
                // Вызываем метод AddMoney, передавая значение монеты
                playerStat.AddMoney(value);
            }

            // Уничтожаем объект монеты
            Destroy(gameObject);
        }
    }
}
