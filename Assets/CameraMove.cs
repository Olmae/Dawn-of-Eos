using UnityEngine;

public class CameraMove : MonoBehaviour
{
    // Параметры плавности следования камеры
    public float damping = 0.1f;
    public float horizontalDamping = 0.2f; // Плавность движения влево и вправо

    // Отступы камеры от игрока
    public Vector2 offset = new Vector2(2f, 1f);

    // Приватные переменные
    private Transform player;
    private Vector3 velocity = Vector3.zero;
    private bool lastFaceLeft = false; // Переменная для запоминания последнего направления камеры

    private void Start()
    {
        // Гарантируем положительные значения отступов
        offset = new Vector2(Mathf.Abs(offset.x), offset.y);

        // Находим и присваиваем трансформ игрока
        FindPlayer();
    }

    private void FindPlayer()
    {
        // Находим и присваиваем трансформ игрока по тегу
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        player = playerObject?.transform;
    }

private void LateUpdate()
{
    // Поиск игрока только в методе LateUpdate()
    if (player == null)
    {
        FindPlayer();
        return; // Прерываем выполнение метода, если игрок еще не найден
    }

    // Остальной код остается без изменений
    float horizontalVelocity = player.GetComponent<Rigidbody2D>().velocity.x;
    bool faceLeft = horizontalVelocity < -0.1f;

    if (Mathf.Abs(horizontalVelocity) > 0.1f)
    {
        lastFaceLeft = faceLeft;
    }

    float targetX = player.position.x + (lastFaceLeft ? -offset.x : offset.x);
    float targetY = player.position.y + offset.y;
    float targetZ = transform.position.z;

    Vector3 targetPosition = new Vector3(targetX, targetY, targetZ);
    float smoothDamping = faceLeft || !faceLeft ? horizontalDamping : damping; 
    transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothDamping);
}

}
