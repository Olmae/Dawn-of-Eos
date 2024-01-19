using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Data;
using Mono.Data.Sqlite;

public class ProfileSelection : MonoBehaviour
{
    private string connectionString;
    public Dropdown profileDropdown;
    public InputField newProfileInput;
    public int selectedProfileId;

    private void Start()
    {
        connectionString = "URI=file:" + Application.dataPath + "/database.db";
        CreateTableIfNotExists();
        LoadProfiles();
        selectedProfileId = PlayerPrefs.GetInt("CurrentProfileId", 1);
        profileDropdown.value = selectedProfileId - 1;
    }

    private void CreateTableIfNotExists()
    {
        IDbConnection dbConnection = new SqliteConnection(connectionString);
        dbConnection.Open();

        IDbCommand dbCommand = dbConnection.CreateCommand();
        dbCommand.CommandText = "CREATE TABLE IF NOT EXISTS Player (Id INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT NOT NULL UNIQUE, Coins INTEGER NOT NULL DEFAULT 0)";
        dbCommand.ExecuteNonQuery();

        dbConnection.Close();
    }

private void Update() {
    UnityEngine.Debug.Log(selectedProfileId);
}
    private void LoadProfiles()
    {
        IDbConnection dbConnection = new SqliteConnection(connectionString);
        dbConnection.Open();

        IDbCommand dbCommand = dbConnection.CreateCommand();
        dbCommand.CommandText = "SELECT Id, Name FROM Player";
        IDataReader dataReader = dbCommand.ExecuteReader();

        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
        while (dataReader.Read())
        {
            int id = dataReader.GetInt32(0);
            string name = dataReader.GetString(1);
            Dropdown.OptionData option = new Dropdown.OptionData(name);
            options.Add(option);
        }
        profileDropdown.options = options;
        profileDropdown.onValueChanged.AddListener(OnProfileSelected);

        dbConnection.Close();
    }

    public void CreateNewProfile()
    {
        string name = newProfileInput.text;
        if (string.IsNullOrEmpty(name))
        {
            Debug.Log("Name cannot be empty");
            return;
        }

        IDbConnection dbConnection = new SqliteConnection(connectionString);
        dbConnection.Open();

        IDbCommand dbCommand = dbConnection.CreateCommand();
        dbCommand.CommandText = "CREATE TABLE IF NOT EXISTS Player (Id INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT NOT NULL UNIQUE, Coins INTEGER NOT NULL DEFAULT 0)";
        dbCommand.ExecuteNonQuery();

        dbCommand = dbConnection.CreateCommand();
        dbCommand.CommandText = "INSERT INTO Player (Name, Coins) VALUES (@name, 0)";
        IDbDataParameter nameParam = dbCommand.CreateParameter();
        nameParam.ParameterName = "@name";
        nameParam.Value = name;
        dbCommand.Parameters.Add(nameParam);
        dbCommand.ExecuteNonQuery();

        dbConnection.Close();

        LoadProfiles();
    }

    public void OnProfileSelected(int profileIndex)
{
    IDbConnection dbConnection = new SqliteConnection(connectionString);
    dbConnection.Open();

    IDbCommand dbCommand = dbConnection.CreateCommand();
    dbCommand.CommandText = "SELECT Id FROM Player WHERE Name = @name";
    IDbDataParameter nameParam = dbCommand.CreateParameter();
    nameParam.ParameterName = "@name";
    nameParam.Value = profileDropdown.options[profileIndex].text;
    dbCommand.Parameters.Add(nameParam);

    IDataReader dataReader = dbCommand.ExecuteReader();
    if (dataReader.Read())
    {
        selectedProfileId = dataReader.GetInt32(0);
        PlayerPrefs.SetInt("CurrentProfileId", selectedProfileId);
    }

    dbConnection.Close();
}


    private void OnDestroy()
    {
        PlayerPrefs.SetInt("CurrentProfileId", selectedProfileId);
        PlayerPrefs.Save();
    }
}
