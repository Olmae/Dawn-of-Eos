using UnityEngine;
using System.Collections;

public class Hint : MonoBehaviour
{
    public GameObject hintScreen;
    public GameObject panel; // Добавленный мной элемент Panel

    private bool hintVisible = true; // Флаг, указывающий, видима ли подсказка

    void Start()
    {
        Invoke("ShowHintScreen", 1f); // Вызываем показ подсказки через 1 секунду
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (hintVisible)
            {
                HidePanel(); // Скрываем панель
                ResumeGame(); // Возобновляем игру
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape) && hintVisible)
        {
            // Блокируем нажатие ESCAPE, если подсказка видима
            return;
        }
    }

    void ShowHintScreen()
    {
        Time.timeScale = 0f; // Останавливаем время в игре
        hintScreen.SetActive(true); // Включаем экран с подсказкой
        StartCoroutine(FadeOut(panel, 1f)); // Запускаем корутину для плавного исчезновения элемента
    }

    void HidePanel()
    {
        StartCoroutine(FadeOut(panel, 1f)); // Запускаем корутину для плавного исчезновения панели
        hintVisible = false; // Устанавливаем флаг, что подсказка больше не видна
                hintScreen.SetActive(false); // выключаем экран с подсказкой

    }

    void ResumeGame()
    {
        // Дополнительный код для возобновления игры
        Time.timeScale = 1f; // Возобновляем время в игре
    }

    IEnumerator FadeOut(GameObject obj, float duration)
    {
        CanvasGroup canvasGroup = obj.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = obj.AddComponent<CanvasGroup>();
        }

        float currentTime = 0f;
        float startAlpha = canvasGroup.alpha;
        while (currentTime < duration)
        {
            currentTime += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, currentTime / duration);
            yield return null;
        }

        obj.SetActive(false); // Выключаем объект после завершения анимации
    }
}
