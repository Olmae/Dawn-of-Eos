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

    private Vector2 lastPlayerPosition;
    private float timeSinceLastPlayerPositionUpdate;

    // Инициализация при старте
    private void Start()
    {
        maxHealth = hp;
        mainCamera = Camera.main; // присвоить ссылку на главную камеру
        AstarPath.active.logPathResults = PathLog.None;
        UnityEngine.Debug.Log("Старт Энеми");

        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player").transform;
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
            currentWaypoint = 0;
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

        // Проверка, находится ли игрок в пределах атаки
        if (distanceToPlayer <= attackRange)
        {
            // Атака игрока
            if (attackStartTimer >= attackStartDelay)
            {
                // Проверка, не находится ли враг в состоянии задержки урона
                if (!isTakingDamage)
                {
                    attackTimer += Time.deltaTime;
                    if (attackTimer >= attackDelay)
                    {
                        Attack();
                        attackTimer = 0f;
                    }
                }
            }
            else
            {
                attackStartTimer += Time.deltaTime;
            }
            return;
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
        RaycastHit2D hit = Physics2D.Linecast(transform.position, player.position, obstacleMask);
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
        RaycastHit2D hit = Physics2D.Linecast(transform.position, player.position, obstacleMask);
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
        if (Time.time - timeSinceLastPathRecalculation > pathRecalculationInterval)
        {
            seeker.StartPath(transform.position, player.position, OnPathComplete);
            timeSinceLastPathRecalculation = Time.time;
        }
    }

    // Обновление пути к игроку
    private void UpdatePathToPlayer()
    {
        seeker.StartPath(transform.position, player.position, OnPathComplete);
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
    }
}
