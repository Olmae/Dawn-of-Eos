using UnityEngine;

public class Hint : MonoBehaviour
{
    public GameObject hintScreen;

    void Start()
    {
        ShowHintScreen();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (IsHintScreenVisible())
            {
                HideHintScreen();
                ResumeGame();
            }
        }
    }

    void ShowHintScreen()
    {
        Time.timeScale = 0f; // Останавливаем время в игре
        hintScreen.SetActive(true); // Включаем экран с подсказкой
    }

    void HideHintScreen()
    {
        Time.timeScale = 1f; // Возобновляем время в игре
        hintScreen.SetActive(false); // Выключаем экран с подсказкой
    }

    void ResumeGame()
    {
        // Дополнительный код для возобновления игры
    }

    bool IsHintScreenVisible()
    {
        return hintScreen.activeSelf; // Проверяем, видим ли экран с подсказкой
    }
}
