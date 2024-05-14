using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AchievementManager : MonoBehaviour
{
    public List<Achievement> achievements = new List<Achievement>();

    // Ссылки на другие компоненты
    public Text achievementText;
    public PlayerStat playerStat;

    private void Start()
    {
        LoadAchievements();
        UpdateAchievements(); // Обновляем ачивки при старте игры
    }

    public void UpdateAchievements()
    {
        // Проверяем ачивки по монетам
        CheckCoinAchievements();

        // Проверяем ачивки по убийствам
        CheckKillAchievements();
    }

    private void CheckCoinAchievements()
    {
        // Получаем текущее количество монет
        int coins = playerStat.money;

        // Проверяем достижения по количеству монет
        UnlockCoinAchievement(10, "Bronze Coin Collector");
        UnlockCoinAchievement(25, "Silver Coin Collector");
        UnlockCoinAchievement(50, "Gold Coin Collector");
        UnlockCoinAchievement(100, "Platinum Coin Collector");
    }

    private void UnlockCoinAchievement(int targetAmount, string achievementName)
    {
        if (!IsAchievementUnlocked(achievementName))
        {
            if (playerStat.money >= targetAmount)
            {
                UnlockAchievement(achievementName);
            }
        }
    }

    private void CheckKillAchievements()
    {
        // Получаем текущее количество убийств
        int kills = playerStat.kills;

        // Проверяем достижения по убийствам
        UnlockKillAchievement(1, "Novice Slayer");
        UnlockKillAchievement(5, "Seasoned Slayer");
        UnlockKillAchievement(10, "Veteran Slayer");
        UnlockKillAchievement(20, "Master Slayer");
    }

    private void UnlockKillAchievement(int targetAmount, string achievementName)
    {
        if (!IsAchievementUnlocked(achievementName))
        {
            if (playerStat.kills >= targetAmount)
            {
                UnlockAchievement(achievementName);
            }
        }
    }

    public void UnlockAchievement(string name)
    {
        Achievement achievement = achievements.Find(x => x.name == name);
        if (achievement != null)
        {
            achievement.unlocked = true;
            SaveAchievements();
            if (achievementText != null)
            {
                achievementText.text = "Achievement Unlocked: " + name;
            }
        }
    }

    private bool IsAchievementUnlocked(string name)
    {
        Achievement achievement = achievements.Find(x => x.name == name);
        if (achievement != null)
        {
            return achievement.unlocked;
        }
        return false;
    }

    private void SaveAchievements()
    {
        string jsonData = JsonUtility.ToJson(this);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/achievements.json", jsonData);
    }

    private void LoadAchievements()
    {
        if (System.IO.File.Exists(Application.persistentDataPath + "/achievements.json"))
        {
            string jsonData = System.IO.File.ReadAllText(Application.persistentDataPath + "/achievements.json");
            JsonUtility.FromJsonOverwrite(jsonData, this);
        }
    }
}
