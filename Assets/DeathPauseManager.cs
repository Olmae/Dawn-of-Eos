using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class DeathPauseManager : MonoBehaviour
{
    public GameObject pauseMenu; // Пауз-меню для отображения при смерти игрока

    private void Start()
    {
        // Пауз-меню должно быть неактивным при старте игры
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }
    }

    // Метод для отображения пауз-меню при смерти игрока
    public void ShowPauseMenu()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f; // Останавливаем время при активации пауз-меню
    }

    // Метод для перезапуска игры
    public void RestartGame()
    {
        StartCoroutine(DelayedRestartGame(0.3f));
    }

    private IEnumerator DelayedRestartGame(float delay)
    {
        yield return new WaitForSecondsRealtime(delay); // Ждём 0.3 секунды реального времени

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1f; // Возобновляем время при перезапуске игры
    }
}
