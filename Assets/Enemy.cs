using UnityEngine;
using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using static PlayerStat;

public class Enemy : MonoBehaviour
{
    // Интервал перерасчета пути
    private float pathRecalculationInterval = 0.5f;
    private float timeSinceLastPathRecalculation;

private Vector3 previousPlayerPosition;
private Vector3 playerVelocity;


    // Диапазоны видимости и атаки
    public float visibilityRange = 10f;
    public float viewDistance = 10.0f;
    public LayerMask viewMask;
    private LayerMask decorLayerMask;
private Vector3 lastSeenPlayerPosition;


    // Компоненты для поиска пути и управления движением
    private Seeker seeker;
    private Rigidbody2D rb;
    private Path path;
    private int currentWaypoint = 0;

    // Параметры движения и здоровья
    public float speed;
    public float hp = 100f;
    public float maxHealth = 100f;
// Получаем направление к игроку


    // Объект игрока и параметры атаки
    public Transform player;
    public float attackRange = 1f;
    public float attackDamage = 10f;
    private int obstacleMask;
    private Vector3 lastKnownPlayerPosition;

    // Параметры задержки атаки
    public float attackDelay = 1.5f;
    public float attackTimer = 0f;
    public float attackStartDelay = 0.3f;
    public float attackStartTimer = 0f;

    // Параметры пути
    private float waypointDistanceThreshold = 0.1f;

    // Параметры задержки урона
    private bool isTakingDamage = false;
    private float damageCooldown = 0.3f;
    private float damageCooldownTimer = 0f;

    // Интерфейс здоровья
    public Image healthBar;
    private Camera mainCamera;
    private float currentHealth;

    // Объекты для выпадения монет
    public GameObject coinPrefab;
    public int coinValue = 1;
private bool isPlayerVisible = false; // Флаг, показывающий, видит ли враг игрока
private float timeSinceLastSawPlayer = 0f; // Время с момента последней видимости игрока
private float timeToForgetPlayer = 5f; // Время, через которое враг забудет игрока после потери видимости

    private Vector2 lastPlayerPosition;
    private float timeSinceLastPlayerPositionUpdate;

    // Инициализация при старте
private void Start()
{
    decorLayerMask = LayerMask.GetMask("DecorLayer");

    maxHealth = hp;
    mainCamera = Camera.main; // присвоить ссылку на главную камеру
    AstarPath.active.logPathResults = PathLog.None;
    UnityEngine.Debug.Log("Старт Энеми");

    seeker = GetComponent<Seeker>();
    rb = GetComponent<Rigidbody2D>();

    // Найти игрока по тегу "Player"
    player = GameObject.FindGameObjectWithTag("Player").transform;

    obstacleMask = LayerMask.GetMask("Wall");
    UpdatePathToPlayer();
}


    // Отображение здоровья над врагом
    private void OnGUI()
    {
        // Получаем координаты головы врага в экранных координатах
        Vector3 worldPosition = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);
        Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPosition);

        // Рисуем healthbar над головой врага
        Rect rect = new Rect(screenPosition.x - 50f, Screen.height - screenPosition.y + 20f, 100f, 20f);
        GUI.color = Color.cyan;
        GUI.DrawTexture(new Rect(rect.x, rect.y, rect.width, rect.height), Texture2D.whiteTexture);
        GUI.color = Color.red;
        GUI.DrawTexture(new Rect(rect.x, rect.y, rect.width * (hp / maxHealth), rect.height), Texture2D.whiteTexture);
    }

    // Обновление пути к игроку
    private void FindPath()
    {
        seeker.StartPath(transform.position, player.position, OnPathComplete);
    }

    // Обработчик завершения поиска пути
private void OnPathComplete(Path p)
{
    if (!p.error)
    {
        path = p;
        currentWaypoint = 0; // Сброс текущей позиции пути
    }
}


    // Обновление в каждом кадре
    private void Update()
    {
// Обновление скорости и направления игрока




           // Проверка наличия игрока
    if (player == null)
    {
        return;
    }

    float distanceToPlayer = Vector2.Distance(transform.position, player.position);

if (distanceToPlayer <= visibilityRange && CanSeePlayer())
{
    isPlayerVisible = true;
    timeSinceLastSawPlayer = Time.time;
    lastSeenPlayerPosition = player.position; // Обновляем последнюю видимую позицию игрока
    FindPathToPlayer(); // Вызываем метод поиска пути к игроку
}

        else
        {
            if (isPlayerVisible)
            {
                if (Time.time - timeSinceLastSawPlayer > timeToForgetPlayer)
                {
                    isPlayerVisible = false;
                }
                else
                {
                    FindPathToLastKnownPlayerPosition(); // Перейти к последней известной позиции игрока
                }
            }
            else
            {
                FindPathToLastKnownPlayerPosition(); // Перейти к последней известной позиции игрока
            }
        }
        if (!isPlayerVisible && Time.time - timeSinceLastPathRecalculation > pathRecalculationInterval)
        {
            UpdatePathToLastPlayerPosition();
            timeSinceLastPathRecalculation = Time.time;
        }

    if (!isPlayerVisible && path != null)
    {
        MoveAlongPath();
    }
        // Получение урона
        if (isTakingDamage)
        {
            damageCooldownTimer -= Time.deltaTime;
            if (damageCooldownTimer <= 0f)
            {
                // Сброс флага задержки урона
                isTakingDamage = false;
            }
        }

        // Проверка, есть ли препятствия на линии видимости к игроку
        RaycastHit2D hit = Physics2D.Linecast(transform.position, player.position, obstacleMask | decorLayerMask);
        if (hit.collider != null)
        {
            UpdatePathToLastPlayerPosition();
            return;
        }

        // Перерасчет пути к игроку
        if (Time.time - timeSinceLastPathRecalculation > pathRecalculationInterval)
        {
            seeker.StartPath(transform.position, player.position, OnPathComplete);
            timeSinceLastPathRecalculation = Time.time;
        }

        // Проверка, достиг ли враг конечной точки пути
        if (path == null || currentWaypoint >= path.vectorPath.Count)
        {
            return;
        }

        // Проверка, находится ли враг в пределах атаки к игроку
        if (distanceToPlayer <= attackRange)
        {
            // Атака игрока
            if (attackStartTimer >= attackStartDelay)
            {
                attackTimer += Time.deltaTime;
                if (attackTimer >= attackDelay)
                {
                    Attack();
                    attackTimer = 0f;
                }
            }
            else
            {
                attackStartTimer += Time.deltaTime;
            }
            return;
        }

        // Движение врага к следующей точке пути
        Vector2 targetPosition = new Vector2(path.vectorPath[currentWaypoint].x, path.vectorPath[currentWaypoint].y);
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        rb.velocity = direction * speed * Time.deltaTime; // Скорость движения врага

        // Проверка, достиг ли враг текущей точки пути
        if (Vector2.Distance(transform.position, targetPosition) < waypointDistanceThreshold)
        {
            currentWaypoint++;
        }
            playerVelocity = (isPlayerVisible) ? (player.position - previousPlayerPosition) / Time.deltaTime : Vector3.zero;
    previousPlayerPosition = player.position;
    }
// Предсказание будущей позиции игрока
private Vector3 PredictPlayerPosition(float time)
{
    return player.position + playerVelocity * time;
}

    // Фиксированный апдейт для движения
    private void FixedUpdate()
    {
        if (player == null)
        {
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (!isPlayerVisible)
            {
                // Добавить проверку на препятствия при движении к игроку
                RaycastHit2D obstacleHit = Physics2D.Linecast(transform.position, player.position, obstacleMask | decorLayerMask);
                if (obstacleHit.collider != null)
                {
                    UpdatePathToLastPlayerPosition();
                    return;
                }
            }



        // Атака игрока
        if (distanceToPlayer <= attackRange)
        {
            if (attackStartTimer >= attackStartDelay)
            {
                attackTimer += Time.deltaTime;
                if (attackTimer >= attackDelay)
                {
                    Attack();
                    attackTimer = 0f;
                }
            }
            else
            {
                attackStartTimer += Time.deltaTime;
            }
            return;
        }
        if (distanceToPlayer >= attackRange)
        {
            attackTimer = 0f;
        }

        // Обновление пути к последней известной позиции игрока
        if (distanceToPlayer > visibilityRange)
        {
            UpdatePathToLastPlayerPosition();
            return;
        }

Vector2 direction = ((Vector2)player.position - (Vector2)transform.position).normalized;

RaycastHit2D hit = Physics2D.BoxCast(transform.position, GetComponent<Collider2D>().bounds.size, 0f, direction, distanceToPlayer, obstacleMask | decorLayerMask);
            if (hit.collider != null)
            {
                UpdatePathToLastPlayerPosition();
                return;
            }


        if (path == null)
        {
            return;
        }

        if (currentWaypoint >= path.vectorPath.Count)
        {
            return;
        }

        // Движение врага к следующей точке пути
        Vector2 targetPosition = path.vectorPath[currentWaypoint];
        Vector2 newPosition = Vector2.MoveTowards(rb.position, targetPosition, speed * Time.fixedDeltaTime);

        rb.MovePosition(newPosition);

        if (Vector2.Distance(rb.position, targetPosition) < 0.1f)
        {
            currentWaypoint++;
        }
    }

// Обновление пути к последней известной позиции игрока
void UpdatePathToLastKnownPlayerPosition()
{
    seeker.StartPath(transform.position, lastSeenPlayerPosition, OnPathComplete);
    currentWaypoint = 0;

    // Обновляем переменную lastSeenPlayerPosition
lastKnownPlayerPosition = lastSeenPlayerPosition;
}

// Передвижение вдоль пути с учётом размеров хитбокса
// Передвижение вдоль пути с учётом размеров хитбокса
private void MoveAlongPath()
{
    if (path == null || currentWaypoint >= path.vectorPath.Count)
    {
        return;
    }

    // Получаем позицию следующей точки пути
    Vector2 targetPosition = path.vectorPath[currentWaypoint];

    // Рассчитываем направление движения к следующей точке
    Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;

    // Перемещаемся к следующей точке
    Vector2 newPosition = (Vector2)transform.position + direction * speed * Time.fixedDeltaTime;
    rb.MovePosition(newPosition);

    // Проверяем, достигли ли следующей точки пути
    if (Vector2.Distance(transform.position, targetPosition) < waypointDistanceThreshold)
    {
        currentWaypoint++;
    }

    // Если достигли конца пути и игрок невидим, двигаемся к последней известной позиции игрока
    if (currentWaypoint >= path.vectorPath.Count && !isPlayerVisible)
    {
        FindPathToLastKnownPlayerPosition();
    }
}



private Vector3 PredictedPlayerPosition(float time)
{
    return player.position + playerVelocity * time;
}

// Метод для проверки, видит ли враг игрока
private bool CanSeePlayer()
{
    Vector2 directionToPlayer = (isPlayerVisible) ? player.position - transform.position : PredictedPlayerPosition(1.0f) - transform.position;
    float distanceToPlayer = directionToPlayer.magnitude;

    // Проверяем, находится ли игрок в пределах дистанции видимости
    if (distanceToPlayer <= visibilityRange)
    {
        // Проверка, есть ли препятствие на линии видимости к игроку
RaycastHit2D hit = Physics2D.Linecast(transform.position, player.position, obstacleMask | decorLayerMask);
        if (hit.collider != null)
        {
            UpdatePathToLastPlayerPosition();
            return false; // Возвращаем false, так как игрок не виден из-за препятствия
        }

        // Обновляем lastSeenPlayerPosition
        lastSeenPlayerPosition = player.position;
        return true; // Возвращаем true, если игрок виден
    }

    return false; // Возвращаем false, если игрок находится за пределами дистанции видимости
}



// Метод для поиска пути к последней известной позиции игрока
private void FindPathToLastKnownPlayerPosition()
{
        Vector3 targetPosition = (isPlayerVisible) ? lastSeenPlayerPosition : PredictedPlayerPosition(1.0f);
    seeker.StartPath(transform.position, targetPosition, OnPathComplete);
    currentWaypoint = 0;
    if (!isPlayerVisible)
    {
        seeker.StartPath(transform.position, lastKnownPlayerPosition, OnPathComplete);
                lastKnownPlayerPosition = player.position; // Обновляем lastKnownPlayerPosition

    }
}

// Метод для поиска пути к игроку
private void FindPathToPlayer()
{
    seeker.StartPath(transform.position, player.position, OnPathComplete);
}


    // Обновление пути к игроку
void UpdatePathToPlayer()
{
    seeker.StartPath(transform.position, player.position, OnPathComplete);
    currentWaypoint = 0; // Сброс текущей позиции пути
lastPlayerPosition = player.position;

}


    // Обработчик изменения позиции игрока
    private void OnPlayerPositionChanged()
    {
        UpdatePathToPlayer();
    }

    // Атака игрока
    private void Attack()
    {
        PlayerStat playerStat = player.GetComponent<PlayerStat>();
        playerStat.TakeDamage(attackDamage);
    }

    // Получение урона
    public void TakeDamage(float damage)
    {
        if (!isTakingDamage)
        {
            hp -= damage;

            if (hp <= 0)
            {
                Die();
            }

            // Устанавливаем флаг задержки урона
            isTakingDamage = true;
            damageCooldownTimer = damageCooldown;
        }
    }
// Обновление пути к последней известной позиции игрока
private void UpdatePathToLastPlayerPosition()
{
    seeker.StartPath(transform.position, lastPlayerPosition, OnPathComplete);
    currentWaypoint = 0;

    // Обновляем переменную lastPlayerPosition
    lastPlayerPosition = player.position;
}

    // Смерть врага
private void Die()
{
    GameObject coin = Instantiate(coinPrefab, transform.position, Quaternion.identity);
    Coin script = coin.GetComponent<Coin>();
    script.value = coinValue;
    Destroy(gameObject);

    // Добавление убийства в счетчик
    PlayerStat playerStat = player.GetComponent<PlayerStat>();
    playerStat.AddKill();
}
}