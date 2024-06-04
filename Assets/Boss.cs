using UnityEngine;

public class Boss : MonoBehaviour
{
    public float detectionRange = 10f;
    public float attackRange = 3f;
    public float moveSpeed = 5f;
    public float maxHealth = 100;
    public int attackDamage = 20;
    public float attackDelay = 2f;
    public GameObject spawnPrefab; // Префаб для спавна
    public Transform player;

    public float currentHealth;
    private bool isAttacking = false;
    private Rigidbody2D rb;

    // Новые переменные для задержки урона
    public float damageCooldown = 1f; // Время задержки между получением урона
    private bool isTakingDamage = false;
    private float damageCooldownTimer = 0f;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    void Update()
    {
        if (!isAttacking)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (distanceToPlayer <= detectionRange)
            {
                if (distanceToPlayer <= attackRange)
                {
                    Attack();
                }
                else
                {
                    MoveTowardsPlayer();
                }
            }
        }

        // Обновляем таймер задержки урона
        if (isTakingDamage)
        {
            damageCooldownTimer -= Time.deltaTime;
            if (damageCooldownTimer <= 0f)
            {
                isTakingDamage = false;
            }
        }
    }

    void MoveTowardsPlayer()
    {
        Vector2 moveDirection = (player.position - transform.position).normalized;
        rb.velocity = moveDirection * moveSpeed;
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerStat playerStat = other.gameObject.GetComponent<PlayerStat>();
            if (playerStat != null && isAttacking)
            {
                DealDamage(playerStat);
            }
        }
    }

    void Attack()
    {
        isAttacking = true;
        Invoke("StopAttack", attackDelay);
    }

    void StopAttack()
    {
        isAttacking = false;
    }

    void DealDamage(PlayerStat playerStat)
    {
        if (playerStat != null)
        {
            playerStat.TakeDamage(attackDamage);
        }
    }

    public void TakeDamage(float damage)
    {
        if (!isTakingDamage)
        {
            currentHealth -= damage;

            if (currentHealth <= 0)
            {
                Die();
            }

            // Устанавливаем флаг задержки урона
            isTakingDamage = true;
            damageCooldownTimer = damageCooldown;
        }
    }

    void Die()
    {
        // Создаем новый экземпляр префаба
        if (spawnPrefab != null)
        {
            Instantiate(spawnPrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}
