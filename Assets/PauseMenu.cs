using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    private bool isPaused = false;
    public GameObject pauseMenu;
    public GameObject settingsMenu;

    public PlayerStat playerStat;

    // Менеджер сохранения
    public SaveManager saveManager;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void OpenSettings()
    {
        settingsMenu.SetActive(true);
        pauseMenu.SetActive(false);
    }

    public void CloseSettings()
    {
        settingsMenu.SetActive(false);
        pauseMenu.SetActive(true);
    }
public void QuitToMainMenu()
{
            playerStat = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStat>();

    // Сохраняем игровые данные перед выходом
    SaveGame();

    // Возвращаем масштаб времени к нормальному
    Time.timeScale = 1f;

    // Загружаем сцену главного меню
    SceneManager.LoadScene(0);
}

// Метод для сохранения данных
private void SaveGame()
{
    // Проверяем, что менеджер сохранения существует
    if (saveManager != null)
    {
        // Вызываем метод сохранения из менеджера сохранения,
        // передавая ему список достижений, количество монет и количество убитых врагов
        saveManager.SaveData(playerStat.achievements, playerStat.money, playerStat.enemiesKilled);

        // Дебаг для отображения сохраненных данных
        Debug.Log("Сохраненные данные:");
        Debug.Log("Достижения:");
        foreach (var achievement in playerStat.achievements)
        {
            Debug.Log(achievement.name + ": " + (achievement.unlocked ? "Получено" : "Не получено"));
        }
        Debug.Log("Монеты: " + playerStat.money);
        Debug.Log("Убитые враги: " + playerStat.enemiesKilled);
    }
    else
    {
        Debug.LogWarning("SaveManager не был присоединен к PauseMenu. Данные не будут сохранены при выходе из игры.");
    }
}

}
