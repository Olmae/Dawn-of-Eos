using UnityEngine;
using UnityEngine.Audio;

public class ArrowItem : MonoBehaviour
{
    public int arrowCount = 10; // Количество стрел, которое добавляется при покупке

    // Звук подбора предмета
    public AudioClip pickupSound;

    // Цена предмета
    public int price = 5;

    // Аудиомикшер и группа SFX
    public AudioMixer audioMixer;
    public AudioMixerGroup sfxGroup;

    private bool isInRange = false; // Флаг для отслеживания нахождения игрока в триггере

    private void Update()
    {
        // Если игрок в зоне триггера и нажата клавиша "Е", активируем объект
        if (isInRange && Input.GetKeyDown(KeyCode.E))
        {
            ActivateObject();
        }
    }

    // Метод для активации объекта
    void ActivateObject()
    {
        // Получаем контроллер персонажа и его статистику
        PlayerStat playerStat = FindObjectOfType<PlayerStat>();
        BowController bowController = FindObjectOfType<BowController>();

        // Проверяем, хватает ли у игрока денег на покупку
        if (playerStat != null && playerStat.money >= price)
        {
            // Уменьшаем количество денег у игрока
            playerStat.money -= price;

            // Добавляем стрелы игроку
            bowController.arrowCount += arrowCount;
            bowController.UpdateArrowCountUI();

            // Проигрываем звук подбора предмета
            if (pickupSound != null)
            {
                PlaySound(pickupSound);
            }

            // Уничтожаем объект предмета после подбора
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("Недостаточно денег для покупки этого предмета.");
        }
    }

    // Метод для воспроизведения звука с учетом аудиомикшера
    void PlaySound(AudioClip clip)
    {
        GameObject soundObject = new GameObject("PickupSound");
        AudioSource audioSource = soundObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.outputAudioMixerGroup = sfxGroup;
        audioSource.Play();

        Destroy(soundObject, clip.length);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isInRange = false;
        }
    }
}
