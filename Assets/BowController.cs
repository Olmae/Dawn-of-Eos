using UnityEngine;
using System.Collections;
using UnityEngine.UI; // Добавляем директиву для работы с UI

public class BowController : MonoBehaviour
{
    public Bow bow; // Ссылка на объект лука
    private bool isReloading = false; // Флаг перезарядки
    private AudioSource audioSource; // Компонент для воспроизведения звука

        [SerializeField]public int arrowCount = 10; // Количество стрел
    private Text arrowCountText; // UI элемент для отображения количества стрел

    // Метод для проверки состояния перезарядки
    public bool IsReloading
    {
        get { return isReloading; }
    }

    void Start()
    {
        UpdateArrowCountUI();
        // Инициализация аудио источника
        audioSource = GetComponent<AudioSource>();

        // Найти текстовый UI элемент по тегу
        GameObject arrowCountObject = GameObject.FindWithTag("ArrowCountUI");
        if (arrowCountObject != null)
        {
            arrowCountText = arrowCountObject.GetComponent<Text>();
            UpdateArrowCountUI();
        }
        else
        {
            Debug.LogError("Не найден объект с тегом 'ArrowCountUI'");
        }
        UpdateArrowCountUI();
    }

    // Метод для стрельбы с лука
    public void ShootArrow()
    {
        if (!isReloading && bow != null && bow.arrowPrefab != null && arrowCount > 0)
        {
            // Устанавливаем флаг перезарядки
            isReloading = true;

            // Уменьшаем количество стрел
            arrowCount--;
            UpdateArrowCountUI();

            // Получаем направление курсора от игрока
            Vector2 mouseDirection = GetMouseDirection();

            // Запускаем корутину для выстрела
            StartCoroutine(Shoot(mouseDirection));
        }
        else
        {
            if (arrowCount <= 0)
            {
                Debug.Log("Нет стрел для выстрела");
            }
            else if (isReloading)
            {
                Debug.Log("Идет перезарядка");
            }
            else if (bow == null)
            {
                Debug.LogError("Лук не назначен");
            }
            else if (bow.arrowPrefab == null)
            {
                Debug.LogError("Префаб стрелы не назначен");
            }
        }
    }

    // Корутина для выстрела
    private IEnumerator Shoot(Vector2 shootDirection)
    {
        // Проверяем наличие лука и префаба стрелы
        if (bow != null && bow.arrowPrefab != null)
        {
            // Устанавливаем флаг перезарядки
            isReloading = true;

            // Проигрываем звук выстрела
            if (bow.shootSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(bow.shootSound);
            }

            // Создаем экземпляр стрелы
            Vector3 arrowSpawnPosition = new Vector3(transform.position.x, transform.position.y, 0); // Устанавливаем Z в 0
            Quaternion arrowRotation = Quaternion.LookRotation(Vector3.forward, shootDirection);
            GameObject arrow = Instantiate(bow.arrowPrefab, arrowSpawnPosition, arrowRotation);
            Arrow arrowScript = arrow.GetComponent<Arrow>();

            // Передаем параметры стрелы
            if (arrowScript != null)
            {
                arrowScript.SetDamage(bow.arrowDamage);
                arrowScript.SetSpeed(bow.arrowSpeed);
                arrowScript.SetMaxRange(bow.maxRange);
                arrowScript.SetDirection(shootDirection);
            }

            // Ждем время перезарядки
            yield return new WaitForSeconds(bow.reloadTime);

            // Сбрасываем флаг перезарядки
            isReloading = false;
        }
        else
        {
            Debug.LogError("Проблема с луком или префабом стрелы");
            isReloading = false; // Сбрасываем флаг перезарядки в случае ошибки
        }
    }

    // Получаем направление от игрока к курсору
    private Vector2 GetMouseDirection()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePosition - (Vector2)transform.position).normalized;
        return direction;
    }

    // Метод для обновления UI с количеством стрел
    public void UpdateArrowCountUI()
    {
        if (arrowCountText != null)
        {
            arrowCountText.text = "Стрелы: " + arrowCount;
        }
    }
}
