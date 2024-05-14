using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour
{
    public Text achievementsText;
    public GameObject playerStatPrefab; // Ссылка на префаб PlayerStat
    private PlayerStat playerStatInstance; // Экземпляр PlayerStat

    private void Start()
    {
        // Создаем экземпляр PlayerStat из префаба
        playerStatInstance = Instantiate(playerStatPrefab).GetComponent<PlayerStat>();
        // Скрываем префаб
        playerStatInstance.gameObject.SetActive(false);
        // Загружаем сохраненные данные
        LoadSavedData();
        // Обновляем отображение ачивок
        UpdateAchievementsDisplay();
    }

    // Метод для загрузки сохраненных данных
    private void LoadSavedData()
    {
        // Создаем экземпляр SaveManager
        SaveManager saveManager = new SaveManager();
        
        // Вызываем метод LoadData через экземпляр класса
        SaveData savedData = saveManager.LoadData();

        if (savedData != null)
        {
            // Устанавливаем загруженные данные
            playerStatInstance.achievements = savedData.achievements;
        }
        else
        {
            Debug.Log("Сохраненные данные не найдены.");
        }
    }

    // Метод для обновления отображения ачивок
    public void UpdateAchievementsDisplay()
    {
        if (playerStatInstance == null || achievementsText == null)
        {
            Debug.LogError("PlayerStat или текстовое поле с ачивками не установлены!");
            return;
        }

        string achievementsInfo = "Достижения:\n";
        foreach (Achievement achievement in playerStatInstance.achievements)
        {
            string status = achievement.unlocked ? " (Получено)" : " (Не получено)";
            achievementsInfo += achievement.name + status + "\n";
        }
        achievementsText.text = achievementsInfo;
    }
}
