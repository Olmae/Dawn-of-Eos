using System;
using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using static ProfileSelection;
using static CharacterController2D;
using static PlayerStat;

public class ShopItem : MonoBehaviour
{
    public int price = 10;
    public int healthBuff = 10;
    public int damageBuff = 5;
    public static event Action ShopItemPurchased;
    private int selectedProfileId;
    private string connectionString;

    private void Start()
    {
        selectedProfileId = PlayerPrefs.GetInt("CurrentProfileId", 1);
        connectionString = "URI=file:" + Application.dataPath + "/database.db";
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Создаем подключение к базе данных
            IDbConnection connection = new SqliteConnection(connectionString);
            connection.Open();

            // Получаем текущее количество монет для профиля
            IDbCommand command = connection.CreateCommand();
            command.CommandText = "SELECT COINS FROM PLAYER WHERE ID = " + selectedProfileId;
            IDataReader reader = command.ExecuteReader();
            int currentCoins = 0;
            if (reader.Read())
            {
                currentCoins = reader.GetInt32(0);
            }
            reader.Close();

            // Проверяем, достаточно ли монет для покупки предмета
            if (currentCoins >= price)
            {
                // Обновляем количество монет для профиля
                int newCoins = currentCoins - price;
                command = connection.CreateCommand();
                command.CommandText = "UPDATE PLAYER SET COINS = " + newCoins + " WHERE ID = " + selectedProfileId;
                command.ExecuteNonQuery();

                // Применяем баффы от предмета
                CharacterController2D playerController = other.GetComponent<CharacterController2D>();
                PlayerStat playerHP = other.GetComponent<PlayerStat>();

                if (playerController != null && playerHP != null)
                {
                    playerHP.currentHealth += healthBuff;
                    playerController.attackDamage += damageBuff;
                    playerHP.UpdateHealthText();
                }

                // Закрываем соединение с базой данных
                connection.Close();

                // Вызываем событие покупки предмета
                ShopItemPurchased?.Invoke();

                // Уничтожаем объект предмета магазина
                Destroy(gameObject);
            }
            else
            {
                // Закрываем соединение с базой данных
                connection.Close();

                // Не хватает монет для покупки предмета
                Debug.Log("Не хватает монет для покупки предмета");
            }
        }
    }
}
