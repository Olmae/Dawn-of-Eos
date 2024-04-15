using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterController2D : MonoBehaviour
{
    // Состояния персонажа
    private enum State
    {
        Normal,
        Rolling,
        Attacking,
        Cooldown
    }

    // Компоненты и переменные для управления персонажем
    private Rigidbody2D rigidbody2D;
    private Vector3 moveDir;
    private Vector3 rollDir;
    private float rollSpeed;

    [SerializeField] private LayerMask dashLayerMask;
    public Image imageCooldown;

    // Параметры атаки
    public float attackRadius = 3f;
    public float attackDelay = 0.5f;
    public LayerMask enemyLayerMask;
    public Transform attackPoint;
    public float attackDamage = 10f;
    private float nextAttackTime = 0.1f;
    public float attackCooldown = 0.5f;

    // Состояние и параметры кулдауна
    private State state;
    private bool isCooldown = false;
    public float MoveSpeed;
    public float RollCD = 2.0f;
    private float cooldownTimer = 0.0f;
    private float nextFire = 0;
    public GameObject AttackEffectPrefab;
    private bool isAttacking = false;

    public Animator animator;
    public float attackAnimationLength;
    private float attackAnimationTime = 0f;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Awake()
{
    rigidbody2D = GetComponent<Rigidbody2D>();
    state = State.Normal;
    imageCooldown = GameObject.FindWithTag("CDUI").GetComponent<Image>();
}


    void Update()
    {
        switch (state)
        {
            case State.Normal:
                // Получение ввода для движения
                float moveX = 0f;
                float moveY = 0f;

                if (Input.GetKey(KeyCode.W))
                    moveY = +1f;
                if (Input.GetKey(KeyCode.S))
                    moveY = -1f;
                if (Input.GetKey(KeyCode.A))
                    moveX = -1f;
                if (Input.GetKey(KeyCode.D))
                    moveX = +1f;

                moveDir = new Vector3(moveX, moveY).normalized;

                // Проверка на использование ролла
                if (Time.time > nextFire)
                {
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        rollDir = moveDir;
                        rollSpeed = 30f;
                        state = State.Rolling;
                        nextFire = Time.time + RollCD;
                        UnityEngine.Debug.Log("CD start");
                        isCooldown = true;
                        cooldownTimer = RollCD;
                    }
                }

                // Проверка на использование атаки
                if (Input.GetMouseButtonDown(0) && Time.time >= nextAttackTime)
                {
                    UnityEngine.Debug.Log("Нажато ЛКМ");

                    // Установка времени следующей атаки
                    nextAttackTime = Time.time + attackCooldown;

                    // Установка параметра атаки в аниматоре в true
                    UnityEngine.Debug.Log("Setting Attack parameter to true");
                    animator.SetBool("Attack", true);
                    attackAnimationTime = Time.time + attackAnimationLength;
                    isAttacking = true;
                }

                // Проверка окончания анимации атаки
                if (isAttacking && Time.time >= attackAnimationTime)
                {
                    UnityEngine.Debug.Log("Setting Attack parameter to false");
                    animator.SetBool("Attack", false);
                    isAttacking = false;
                }

                // Нанесение урона врагам в течение всей анимации атаки
                if (isAttacking)
                {
                    Collider2D[] hitColliders = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, enemyLayerMask);
                    foreach (Collider2D collider in hitColliders)
                    {
                        Enemy enemy = collider.GetComponent<Enemy>();
                        if (enemy != null)
                        {
                            // Нанесение урона врагу
                            enemy.TakeDamage(attackDamage);
                        }
                    }
                }

                // Определение направления взгляда персонажа
                if (moveX > 0)
                {
                    transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                }
                if (moveX < 0)
                {
                    transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                }
                if (moveY > 0)
                {
                    transform.localScale = new Vector3(transform.localScale.x, Mathf.Abs(transform.localScale.y), transform.localScale.z);
                }

                break;

            case State.Rolling:
                // Замедление скорости ролла
                float rollSpeedDropMult = 5f;
                rollSpeed -= rollSpeed * rollSpeedDropMult * Time.deltaTime;
                float rollSpeedDropMin = 15f;
                if (rollSpeed < rollSpeedDropMin)
                {
                    state = State.Normal;
                }

                // Применение кулдауна
                cooldownTimer -= Time.deltaTime;
                imageCooldown.fillAmount = cooldownTimer / RollCD;

                if (cooldownTimer <= 0)
                {
                    isCooldown = false;
                    imageCooldown.fillAmount = 0;
                }

                break;
        }
    }

    private void FixedUpdate()
    {
        switch (state)
        {
            case State.Normal:
                // Установка скорости движения
                rigidbody2D.velocity = moveDir * MoveSpeed;
                break;
            case State.Rolling:
                // Установка скорости ролла
                rigidbody2D.velocity = moveDir * rollSpeed;
                break;
        }
    }


    // Применение кулдауна
// Применение кулдауна
void ApplyCooldown()
{
    cooldownTimer -= Time.deltaTime;
    imageCooldown.fillAmount = Mathf.Clamp(cooldownTimer / RollCD, 0f, 1f); // Устанавливаем FillAmount от 0 до 1

    if (cooldownTimer <= 0)
    {
        isCooldown = false;
        cooldownTimer = 0.0f;
        UnityEngine.Debug.Log("Кулдаун закончился");
    }   
}



    // Утилиты для работы с мышью
    public static class MouseUtils
    {
        public static Vector3 GetMouseWorldPosition()
        {
            return GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main, 0f);
        }

        public static Vector3 GetMouseWorldPositionWithZ()
        {
            return GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main, 0f);
        }

        public static Vector3 GetMouseWorldPositionWithZ(Camera worldCamera)
        {
            return GetMouseWorldPositionWithZ(Input.mousePosition, worldCamera, 0f);
        }    

        public static Vector3 GetMouseWorldPositionWithZ(Vector3 screenPosition, Camera worldCamera, float zPosition)
        {
            Vector3 worldPosition = worldCamera.ScreenToWorldPoint(screenPosition);
            worldPosition.z = zPosition;
            return worldPosition;
        }

        public static Vector3 GetDirToMouse(Vector3 fromPosition)
        {
            Vector3 mouseWorldPosition = GetMouseWorldPosition();
            return (mouseWorldPosition - fromPosition).normalized;
        }
    }
}
