using UnityEngine;
using UnityEngine.UI;
using System.Data;
using Mono.Data.Sqlite;
using static Coin;

public class UiGame : MonoBehaviour
{
    public Text coinText;
    public Text nameText;
    private int selectedProfileId;
    private string connectionString;

    private void Start()
    {
        selectedProfileId = PlayerPrefs.GetInt("CurrentProfileId", 1);
        connectionString = "URI=file:" + Application.dataPath + "/database.db";

        Coin.CoinUpdated += UpdateUI;

        UpdateUI();
    }

    private void UpdateUI()
    {
        IDbConnection connection = new SqliteConnection(connectionString);
        connection.Open();

        IDbCommand command = connection.CreateCommand();
        command.CommandText = "SELECT NAME, COINS FROM PLAYER WHERE ID = " + selectedProfileId;
        IDataReader reader = command.ExecuteReader();
        if (reader.Read())
        {
            string name = reader.GetString(0);
            int coins = reader.GetInt32(1);

            nameText.text = "Имя: " + name;

            if (coinText != null) {
                coinText.text = "Монеты: " + coins;
            }
        }
        reader.Close();

        connection.Close();
    }
}
