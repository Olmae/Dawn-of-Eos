using UnityEngine;
using UnityEngine.Audio;

public class BowItem : MonoBehaviour
{
    // Типы улучшаемых параметров
    public enum ParameterType
    {
        Damage,
        ReloadSpeed // Новый тип параметра для улучшения скорости перезарядки
    }

    // Параметры улучшения
    public ParameterType[] parameterTypes;

    // Значения улучшения параметров
    public float[] improvementValues;

    // Звук подбора предмета
    public AudioClip pickupSound;

    // Цена предмета
    public int price = 10;

    public Bow bow;

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
        // Получаем контроллер персонажа
        CharacterController2D characterController = FindObjectOfType<CharacterController2D>();
        PlayerStat playerStat = FindObjectOfType<PlayerStat>();

        // Проверяем, хватает ли у игрока денег на покупку
        if (playerStat != null && playerStat.money >= price)
        {
            // Уменьшаем количество денег у игрока
            playerStat.money -= price;

            // Применяем улучшение параметров в зависимости от выбранных типов
            for (int i = 0; i < parameterTypes.Length; i++)
            {
                switch (parameterTypes[i])
                {
                    case ParameterType.Damage:
                        bow.arrowDamage += improvementValues[i];
                        break;
                    case ParameterType.ReloadSpeed:
                        bow.reloadTime -= improvementValues[i]; // Уменьшаем время перезарядки на значение improvementValues[i]
                        break;
                }
            }

            // Проигрываем звук подбора предмета
            if (pickupSound != null)
            {
                PlaySound(pickupSound);
            }

            // Устанавливаем выбранный лук в скрипте персонажа
            if (characterController != null)
            {
                characterController.currentBow = bow;
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
