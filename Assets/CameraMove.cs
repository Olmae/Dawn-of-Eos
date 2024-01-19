using UnityEngine;

public class CameraMove : MonoBehaviour
{
    // Параметры плавности следования камеры
    public float damping = 0.1f;

    // Отступы камеры от игрока
    public Vector2 offset = new Vector2(2f, 1f);

    // Приватные переменные
    private Transform player;
    private Vector3 velocity = Vector3.zero;

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
        if (player != null)
        {
            // Определяем направление взгляда игрока
            bool faceLeft = player.GetComponent<Rigidbody2D>().velocity.x < -0.1f;

            // Рассчитываем целевую позицию в зависимости от направления взгляда
            Vector3 target = new Vector3(
                player.position.x + (faceLeft ? -offset.x : offset.x),
                player.position.y + offset.y,
                transform.position.z
            );

            // Плавно перемещаем камеру к целевой позиции
            transform.position = Vector3.SmoothDamp(transform.position, target, ref velocity, damping);
        }
    }
}
