using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

[System.Serializable]
public class Achievement
{
    public string name;
    public bool unlocked;

    public Achievement(string _name)
    {
        name = _name;
        unlocked = false;
    }
}

[System.Serializable]
public class PlayerStat : MonoBehaviour
{
    [Header("Health")]
       [SerializeField] public float maxHealth = 6f;
    [SerializeField]     public float currentHealth;
    public Text healthText;

    [Header("Money")]
       [SerializeField] public int money = 0;
    public Text moneyText;

    [Header("Kills")]
    public int kills = 0;
    public int enemiesKilled = 0;
    private bool isHealing = false;
    private float healingCooldown = 1f;
    private float healingCooldownTimer = 0f;

    [Header("Achievements")]
    public List<Achievement> achievements = new List<Achievement>(); // Список всех достижений
    public List<string> allAchievementNames = new List<string>();

    public Text achievementsText;

    [Header("UI")]
    public DeathPauseManager deathPauseManager; // Ссылка на менеджер смерти и пауз-меню
    public string achievementTextTag = "AchievementUnlock"; // Тег для поиска текстового поля с достижениями

private void Start()
{
    GameObject deathPauseManagerObject = GameObject.FindGameObjectWithTag("Die");
        if (deathPauseManagerObject != null)
        {
            deathPauseManager = deathPauseManagerObject.GetComponent<DeathPauseManager>();
        }
        else
        {
            Debug.LogError("Death Pause Manager object not found with tag 'Die'");
        }

    currentHealth = maxHealth;
    healthText = GameObject.FindGameObjectWithTag("Health").GetComponent<Text>();
    moneyText = GameObject.FindGameObjectWithTag("CoinUI").GetComponent<Text>(); 

    achievementsText = GameObject.FindGameObjectWithTag(achievementTextTag).GetComponent<Text>(); // Находим текст по тегу
    LoadSavedData();
}




private void LoadSavedData()
    {
        // Создаем экземпляр SaveManager
        SaveManager saveManager = new SaveManager();
        
        // Вызываем метод LoadData через экземпляр класса
        SaveData savedData = saveManager.LoadData();

        if (savedData != null)
        {
            // Устанавливаем загруженные данные
            achievements = savedData.achievements;
            money = savedData.money;
            enemiesKilled = savedData.enemiesKilled;
            // Обновляем отображение данных
            UpdateAchievementsText();
            UpdateMoneyText();
            UpdateHealthText();
        }
        else
        {
            
            Debug.Log("Сохраненные данные не найдены.");
        }
    }

    private void FixedUpdate()
    {
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
        UpdateHealthText();

        if (isHealing)
        {
            healingCooldownTimer -= Time.deltaTime;
            if (healingCooldownTimer <= 0f)
            {
                isHealing = false;
            }
        }

        // Проверяем условия для получения ачивок
        CheckCoinAchievements();
        CheckLevelAchievements();
    }

    public void TakeDamage(float damage)
    {
        if (!isHealing)
        {
            currentHealth -= damage;

            if (currentHealth <= 0)
            {
                Die();
            }

            isHealing = true;
            healingCooldownTimer = healingCooldown;

            UpdateHealthText();
                                Debug.LogError("kills:" + kills + "enemies" + enemiesKilled);

        }
    }

    public void Update(){
        CheckKillAchievements();
            

    }

 private void Die()
    {
            deathPauseManager.ShowPauseMenu(); // Вызываем пауз-меню при смерти
    }






    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1f;
    }

    public void AddMoney(int amount)
    {
        money += amount;
        UpdateMoneyText();
    }

    public void AddKill()
    {
        enemiesKilled++;
    }


    public void UpdateHealthText()
    {
        healthText.text = "ОЗ: " + currentHealth.ToString();
    }

    public void UpdateMoneyText()
    {
        moneyText.text = "Монеты: " + money.ToString(); 
    }

    // Метод для обновления текста достижений
    public void UpdateAchievementsText()
    {
        string achievementsInfo = "Достижения:\n";
        foreach (Achievement achievement in achievements)
        {
            string status = achievement.unlocked ? " (Получено)" : " (Не получено)";
            achievementsInfo += achievement.name + status + "\n";
        }
        achievementsText.text = achievementsInfo;
    }
    public void CheckAllAchievements()
    {
        CheckCoinAchievements();
        CheckKillAchievements();
        CheckLevelAchievements();
        // Добавь сюда вызов других методов проверки достижений, если они есть
    }
    // Метод для добавления достижения в список
// Метод для добавления достижения в список
public void AddAchievement(string achievementName)
{
    // Проверяем, есть ли уже такое достижение в списке
    if (!IsAchievementUnlocked(achievementName))
    {
        // Добавляем новое достижение
        achievements.Add(new Achievement(achievementName) { unlocked = true });
        UpdateAchievementsText();
    }
}



    // Метод для проверки условий получения ачивок за монетки
// Метод для проверки условий получения ачивок за монетки
public void CheckCoinAchievements()
{
    // Проверка для ачивок по монетам
    if (money >= 1 && !IsAchievementUnlocked("Первое золото!"))
    {
        AddAchievement("Первое золото!");
    }
    if (money >= 5 && !IsAchievementUnlocked("Полная копилка"))
    {
        AddAchievement("Полная копилка");
    }
    if (money >= 10 && !IsAchievementUnlocked("Золотой сундук"))
    {
        AddAchievement("Золотой сундук");
    }
    if (money >= 20 && !IsAchievementUnlocked("Кладоискатель"))
    {
        AddAchievement("Кладоискатель");
    }
    if (money >= 50 && !IsAchievementUnlocked("Богатей!"))
    {
        AddAchievement("Миллионер");
    }
}

public void CheckKillAchievements()
{
    // Проверка для ачивок по убийствам
    if (enemiesKilled >= 1 && !IsAchievementUnlocked("Первое... Убийство..."))
    {
        AddAchievement("Первое... Убийство...");
    }
    if (enemiesKilled >= 3 && !IsAchievementUnlocked("Следопыт"))
    {
        AddAchievement("Следопыт");
    }
    if (enemiesKilled >= 5 && !IsAchievementUnlocked("Охотник на монстров"))
    {
        AddAchievement("Охотник на монстров");
    }
    if (enemiesKilled >= 10 && !IsAchievementUnlocked("Мастер боя"))
    {
        AddAchievement("Мастер боя");
    }
    if (enemiesKilled >= 20 && !IsAchievementUnlocked("Бог войны"))
    {
        AddAchievement("Бог войны");
    }
}

public void CheckLevelAchievements()
{
    // Проверка для ачивок за прохождение уровней
    if (SceneManager.GetActiveScene().name == "Level10" && !IsAchievementUnlocked("Пройти 10 уровней"))
    {
        AddAchievement("Пройти 10 уровней");
    }
    // Добавь здесь проверки для других ачивок за прохождение уровней, если необходимо
}

    // Метод для проверки, разблокировано ли достижение
    public bool IsAchievementUnlocked(string achievementName)
    {
        foreach (Achievement achievement in achievements)
        {
            if (achievement.name == achievementName && achievement.unlocked)
            {
                return true;
            }
        }
        return false;
    }
}
