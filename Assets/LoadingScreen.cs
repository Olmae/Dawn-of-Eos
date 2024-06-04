using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    public GameObject panel; // Добавленный мной элемент Panel

    // Start is called before the first frame update
    public void ShowPanel()
    {
        StartCoroutine(FadeIn(panel, 0.3f)); // Запускаем корутину для плавного появления панели
    }

    IEnumerator FadeIn(GameObject obj, float duration)
    {
        obj.SetActive(true); // Включаем объект перед началом анимации

        CanvasGroup canvasGroup = obj.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = obj.AddComponent<CanvasGroup>();
        }

        float currentTime = 0f;
        float startAlpha = canvasGroup.alpha;
        canvasGroup.alpha = 0f; // Устанавливаем начальную прозрачность
        while (currentTime < duration)
        {
            currentTime += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, currentTime / duration);
            yield return null;
        }

        canvasGroup.alpha = 1f; // Устанавливаем конечную прозрачность
    }
}
