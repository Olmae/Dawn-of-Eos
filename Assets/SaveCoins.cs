using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;

public class SaveCoins : MonoBehaviour
{
    private string connectionString;
    private int currentProfileId;
    private int coins;

    private void Start()
    {
        connectionString = "URI=file:" + Application.dataPath + "/database.db";

        // Получаем ID текущего профиля
        currentProfileId = PlayerPrefs.GetInt("CurrentProfileId", 0);

        // Загружаем количество монет текущего профиля
        LoadCoins();
    }

    private void LoadCoins()
    {
        IDbConnection dbConnection = new SqliteConnection(connectionString);
        dbConnection.Open();

        IDbCommand dbCommand = dbConnection.CreateCommand();
        dbCommand.CommandText = "SELECT Coins FROM Player WHERE Id=@id";
        IDbDataParameter idParam = dbCommand.CreateParameter();
        idParam.ParameterName = "@id";
        idParam.Value = currentProfileId;
        dbCommand.Parameters.Add(idParam);

        object result = dbCommand.ExecuteScalar();
        if (result != null)
        {
            coins = (int)result;
        }
        else
        {
            Debug.LogError("Failed to load coins for profile with ID " + currentProfileId);
        }

        dbConnection.Close();
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        UpdateCoins();
    }

    public void RemoveCoins(int amount)
    {
        coins -= amount;
        UpdateCoins();
    }

    private void UpdateCoins()
    {
        IDbConnection dbConnection = new SqliteConnection(connectionString);
        dbConnection.Open();

        IDbCommand dbCommand = dbConnection.CreateCommand();
        dbCommand.CommandText = "UPDATE Player SET Coins=@coins WHERE Id=@id";
        IDbDataParameter coinsParam = dbCommand.CreateParameter();
        coinsParam.ParameterName = "@coins";
        coinsParam.Value = coins;
        dbCommand.Parameters.Add(coinsParam);

        IDbDataParameter idParam = dbCommand.CreateParameter();
        idParam.ParameterName = "@id";
        idParam.Value = currentProfileId;
        dbCommand.Parameters.Add(idParam);

        dbCommand.ExecuteNonQuery();

        dbConnection.Close();
    }
}
