using UnityEngine;
using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using static PlayerStat;

public class Enemy : MonoBehaviour
{
private float pathRecalculationInterval = 0.2f;
private float timeSinceLastPathRecalculation;
public float visibilityRange = 10f;
public float viewDistance = 10.0f;
public LayerMask viewMask;
private Seeker seeker;
private Rigidbody2D rb;
private Path path;
private int currentWaypoint = 0;
public float speed;
public float hp = 100f;
    public float maxHealth = 100f;

public Transform player;
public float attackRange = 1f;
public float attackDamage = 10f;
private int obstacleMask;
private Vector3 lastKnownPlayerPosition;

public float attackDelay = 1.5f;
public float attackTimer = 0f;
public float attackStartDelay = 0.3f;
public float attackStartTimer = 0f;

    private float waypointDistanceThreshold = 0.1f;

// Задержка урона
private bool isTakingDamage = false;
private float damageCooldown = 0.3f;
private float damageCooldownTimer = 0f;


///bar
    public Image healthBar;
    private Camera mainCamera;
    private float currentHealth;








public GameObject coinPrefab;
public int coinValue = 1;

private Vector2 lastPlayerPosition;
private float timeSinceLastPlayerPositionUpdate;

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

private void FindPath()
{
    seeker.StartPath(transform.position, player.position, OnPathComplete);
}




private void OnPathComplete(Path p)
{
    if (!p.error)
    {
        path = p;
        currentWaypoint = 0;
    }
}

private void Update()
{
    // Check if the player exists
    if (player == null)
    {
        return;
    }

    float distanceToPlayer = Vector2.Distance(transform.position, player.position);

    // Check if the player is within the visibility range
if (distanceToPlayer <= attackRange)
    {
        // Attack the player
        if (attackStartTimer >= attackStartDelay)
        {
            // Check if the enemy is not in the damage cooldown state
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

if (isTakingDamage)
    {
        damageCooldownTimer -= Time.deltaTime;
        if (damageCooldownTimer <= 0f)
        {
            // Сброс флага задержки урона
            isTakingDamage = false;
        }
    }
    
    // Check if there are any obstacles blocking the line of sight to the player
    RaycastHit2D hit = Physics2D.Linecast(transform.position, player.position, obstacleMask);
    if (hit.collider != null)
    {
        UpdatePathToLastPlayerPosition();
        return;
    }

    // Recalculate the path to the player
    if (Time.time - timeSinceLastPathRecalculation > pathRecalculationInterval)
    {
        seeker.StartPath(transform.position, player.position, OnPathComplete);
        timeSinceLastPathRecalculation = Time.time;
    }

    // Check if the enemy has reached the end of the path
    if (path == null || currentWaypoint >= path.vectorPath.Count)
    {
        return;
    }

    // Check if the enemy is within attack range of the player
    if (distanceToPlayer <= attackRange)
    {
        // Attack the player
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

    // Move the enemy towards the next waypoint
    Vector2 targetPosition = new Vector2(path.vectorPath[currentWaypoint].x, path.vectorPath[currentWaypoint].y);
    Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
    rb.velocity = direction * speed;

    // Check if the enemy has reached the waypoint
    if (Vector2.Distance(transform.position, targetPosition) < waypointDistanceThreshold)
    {
        currentWaypoint++;
    }
}


private void FixedUpdate()
{
    if (player == null) {
        return;
    }

    float distanceToPlayer = Vector2.Distance(transform.position, player.position);

if (distanceToPlayer <= attackRange)
    {
        // Attack the player
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
    if (distanceToPlayer >= attackRange){
        attackTimer = 0f;
    }

    if (distanceToPlayer > visibilityRange) {
        UpdatePathToLastPlayerPosition();
        return;
    }

    RaycastHit2D hit = Physics2D.Linecast(transform.position, player.position, obstacleMask);
    if (hit.collider != null) {
        UpdatePathToLastPlayerPosition();
        return;
    }

    if (path == null) {
        return;
    }

    if (currentWaypoint >= path.vectorPath.Count) {
        return;
    }

    Vector2 targetPosition = path.vectorPath[currentWaypoint];
    Vector2 newPosition = Vector2.MoveTowards(rb.position, targetPosition, speed * Time.fixedDeltaTime);

    rb.MovePosition(newPosition);

    if (Vector2.Distance(rb.position, targetPosition) < 0.1f) {
        currentWaypoint++;
    }
}


void UpdatePathToLastPlayerPosition() {
    if (Time.time - timeSinceLastPathRecalculation > pathRecalculationInterval) {
        seeker.StartPath(transform.position, player.position, OnPathComplete);
        timeSinceLastPathRecalculation = Time.time;
    }
}





private void UpdatePathToPlayer()
{
    seeker.StartPath(transform.position, player.position, OnPathComplete);
}



private void OnPlayerPositionChanged()
{
    UpdatePathToPlayer();
}

private void Attack()
{
    PlayerStat playerStat = player.GetComponent<PlayerStat>();
    playerStat.TakeDamage(attackDamage);
}

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


private void Die()
{
    GameObject coin = Instantiate(coinPrefab, transform.position, Quaternion.identity);
    Coin script = coin.GetComponent<Coin>();
    script.value = coinValue;
    Destroy(gameObject);
}
}

