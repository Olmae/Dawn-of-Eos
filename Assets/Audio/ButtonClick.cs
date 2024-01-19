using UnityEngine;
using UnityEngine.UI;

public class ButtonClick : MonoBehaviour
{
    [SerializeField] private Button button; // Ссылка на компонент Button
    [SerializeField] private AudioSource audioSource; // Ссылка на компонент AudioSource
    [SerializeField] private AudioClip clickSound; // Звук нажатия на кнопку

    void Start()
    {
        // Добавляем слушатель события нажатия на кнопку
        button.onClick.AddListener(PlayClickSound);
    }

    void PlayClickSound()
    {
        // Воспроизводим звук нажатия на кнопку
        audioSource.PlayOneShot(clickSound);
    }
}
