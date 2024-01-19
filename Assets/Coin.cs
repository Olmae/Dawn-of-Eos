using System;
using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using static ProfileSelection;

public class Coin : MonoBehaviour
{
    public int value = 2;
    public static event Action CoinUpdated;
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

            // Обновляем количество монет для профиля
            int newCoins = currentCoins + value;
            command = connection.CreateCommand();
            command.CommandText = "UPDATE PLAYER SET COINS = " + newCoins + " WHERE ID = " + selectedProfileId;
            command.ExecuteNonQuery();

            // Закрываем соединение с базой данных

            // Вызываем событие обновления монет
            CoinUpdated?.Invoke();

            // Уничтожаем объект монетки
            Destroy(gameObject);
        }
    }
}
