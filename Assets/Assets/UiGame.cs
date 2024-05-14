using UnityEngine;
using UnityEngine.UI;

public class UiGame : MonoBehaviour
{
    public Text coinText;
    public Text nameText;
    private int coins; // Переменная для хранения количества монет
    private string playerName; // Переменная для хранения имени игрока

    private void Start()
    {
        // Загрузка данных из PlayerPrefs при старте игры
        coins = PlayerPrefs.GetInt("Coins", 0);
        playerName = PlayerPrefs.GetString("PlayerName", "Player");

        // Обновление интерфейса
        UpdateUI();
    }

    private void UpdateUI()
    {
        // Обновление текста для количества монет и имени игрока
        coinText.text = "Монеты: " + coins;
        nameText.text = "Имя: " + playerName;
    }
}
