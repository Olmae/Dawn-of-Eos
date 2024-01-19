using System;
using Mono.Data.Sqlite;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    private string connectionString;

    private void Start()
    {
        // Set up the connection string
        connectionString = "URI=file:" + Application.dataPath + "/database.db";

        // Open a connection to the database
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            // Create the table for storing the coins if it doesn't already exist
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "CREATE TABLE IF NOT EXISTS Coins (id INTEGER PRIMARY KEY AUTOINCREMENT, amount INTEGER)";
                command.ExecuteNonQuery();
            }
        }
    }
 // добавить монету в бд
    public void AddCoin(int amount)
    {
        // Open a connection to the database
        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            // Insert the coin into the table
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO Coins (amount) VALUES (@amount)";
                command.Parameters.AddWithValue("@amount", amount);
                command.ExecuteNonQuery();
            }
        }
    }
}
