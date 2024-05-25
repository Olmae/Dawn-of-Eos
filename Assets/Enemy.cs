using UnityEngine;
using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using static PlayerStat;

public class Enemy : MonoBehaviour
{
    // Интервал перерасчета пути
    private float pathRecalculationInterval = 0.2f;
    private float timeSinceLastPathRecalculation;

    // Диапазоны видимости и атаки
    public float visibilityRange = 10f;
    public float viewDistance = 10.0f;
    public LayerMask viewMask;
    private LayerMask decorLayerMask;


    // Компоненты для поиска пути и управления движением
    private Seeker seeker;
    private Rigidbody2D rb;
    private Path path;
    private int currentWaypoint = 0;

    // Параметры движения и здоровья
    public float speed;
    public float hp = 100f;
    public float maxHealth = 100f;

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
    FindPath();
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
           // Проверка наличия игрока
    if (player == null)
    {
        return;
    }

    float distanceToPlayer = Vector2.Distance(transform.position, player.position);

    // Проверка, видит ли враг игрока
    if (distanceToPlayer <= visibilityRange && CanSeePlayer())
    {
        // Установка флага видимости игрока
        isPlayerVisible = true;
        timeSinceLastSawPlayer = Time.time; // Обновление времени последней видимости игрока
        FindPathToPlayer(); // Найти путь к игроку
    }
    else
    {
        // Если враг не видит игрока, но видел его ранее
        if (isPlayerVisible)
        {
            // Если прошло достаточно времени с момента последней видимости игрока
            if (Time.time - timeSinceLastSawPlayer > timeToForgetPlayer)
            {
                isPlayerVisible = false; // Сброс флага видимости игрока
            }
            else
            {
                // Враг ещё помнит игрока, поэтому обновляем путь
                FindPathToLastKnownPlayerPosition();
            }
        }
        else
        {
            // Если враг не видит игрока и не помнит его, обновляем путь к последней известной позиции игрока
            FindPathToLastKnownPlayerPosition();
        }
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
        rb.velocity = direction * speed;

        // Проверка, достиг ли враг текущей точки пути
        if (Vector2.Distance(transform.position, targetPosition) < waypointDistanceThreshold)
        {
            currentWaypoint++;
        }
    }

    // Фиксированный апдейт для движения
    private void FixedUpdate()
    {
        if (player == null)
        {
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

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

        // Проверка наличия препятствий на линии видимости к игроку
        RaycastHit2D hit = Physics2D.Linecast(transform.position, player.position, obstacleMask | decorLayerMask);
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
void UpdatePathToLastPlayerPosition()
{
    seeker.StartPath(transform.position, player.position, OnPathComplete);
    currentWaypoint = 0; // Сброс текущей позиции пути
}

// Метод для проверки, видит ли враг игрока
private bool CanSeePlayer()
{
    Vector2 directionToPlayer = player.position - transform.position;
    float distanceToPlayer = directionToPlayer.magnitude;

    // Проверка, находится ли игрок в пределах дистанции видимости и не закрывается ли ему препятствие
    if (distanceToPlayer <= visibilityRange && !Physics2D.Raycast(transform.position, directionToPlayer.normalized, distanceToPlayer, obstacleMask))
    {
        return true;
    }

    return false;
}

// Метод для поиска пути к последней известной позиции игрока
private void FindPathToLastKnownPlayerPosition()
{
    seeker.StartPath(transform.position, lastKnownPlayerPosition, OnPathComplete);
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