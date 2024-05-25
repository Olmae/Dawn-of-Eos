using UnityEngine;

public class Arrow : MonoBehaviour
{
    private float damage; // Урон стрелы
    private float speed; // Скорость полета стрелы
    private float maxRange; // Максимальная дальность стрельбы
    private float distanceTraveled; // Пройденное расстояние

    private Vector2 direction; // Направление полета стрелы

    private void Update()
    {
        // Двигаем стрелу вперед по заданному направлению
        transform.Translate(direction * speed * Time.deltaTime, Space.World);

        // Обновляем пройденное расстояние
        distanceTraveled += speed * Time.deltaTime;

        // Проверяем, достигла ли стрела максимальной дальности
        if (distanceTraveled >= maxRange)
        {
            Destroy(gameObject); // Уничтожаем стрелу
        }
    }

    // Метод для установки направления полета стрелы
    public void SetDirection(Vector2 arrowDirection)
    {
        direction = arrowDirection.normalized;
        transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);

    }

    // Устанавливаем урон стрелы
    public void SetDamage(float damageAmount)
    {
        damage = damageAmount;
    }

    // Устанавливаем скорость стрелы
    public void SetSpeed(float arrowSpeed)
    {
        speed = arrowSpeed;
    }

    // Устанавливаем максимальную дальность стрельбы
    public void SetMaxRange(float range)
    {
        maxRange = range;
    }

   private void OnTriggerEnter2D(Collider2D other)
{
    // Проверяем коллизию со врагом
    if (other.CompareTag("Enemy"))
    {
        // Получаем компонент Enemy у врага
        Enemy enemy = other.GetComponent<Enemy>();

        // Наносим урон врагу
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }

        // Уничтожаем стрелу при попадании во врага
        Destroy(gameObject);
    }
    // Проверяем коллизию со стеной
    else if (other.gameObject.layer == LayerMask.NameToLayer("Wall"))
    {
        // Уничтожаем стрелу при попадании в стену
        Destroy(gameObject);
    }
}

}
