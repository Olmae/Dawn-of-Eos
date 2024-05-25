using UnityEngine;
using UnityEngine.Audio;

public class ITEMS : MonoBehaviour
{
    public enum ParameterType
    {
        MaxHealth,
        CurrentHealth,
        RollCooldown,
        MovementSpeed,
    }

    public ParameterType parameterType;
    public float improvementValue = 1.1f;
    public AudioClip pickupSound;
    public int price = 10;
    private bool isInRange = false;

    // Добавляем ссылки на аудиомикшер и группу SFX
    public AudioMixer audioMixer;
    public AudioMixerGroup sfxGroup;

    private void Update()
    {
        if (isInRange && Input.GetKeyDown(KeyCode.E))
        {
            ActivateObject();
        }
    }

    void ActivateObject()
    {
        CharacterController2D characterController = FindObjectOfType<CharacterController2D>();
        PlayerStat playerStat = FindObjectOfType<PlayerStat>();

        if (playerStat != null && playerStat.money >= price)
        {
            playerStat.money -= price;

            switch (parameterType)
            {
                case ParameterType.MaxHealth:
                    playerStat.maxHealth *= improvementValue;
                    break;
                case ParameterType.CurrentHealth:
                    playerStat.currentHealth *= improvementValue;
                    break;
                case ParameterType.RollCooldown:
                    characterController.RollCD *= improvementValue;
                    break;
                case ParameterType.MovementSpeed:
                    characterController.MoveSpeed *= improvementValue;
                    break;
            }

            if (pickupSound != null)
            {
                PlaySound(pickupSound);
            }

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
