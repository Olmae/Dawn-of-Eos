using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class CharacterController2D : MonoBehaviour
{
    // Состояния персонажа
    // Состояния персонажа
    private enum State
    {
        Normal,
        Rolling,
        Attacking,
        Cooldown
    }

    private State state; // Объявление переменной state
    public BowController bowController;

    // Компоненты и переменные для управления персонажем
    [Header("Movement")]
    [SerializeField] private Animator WalkAnim;
    private Rigidbody2D rigidbody2D;
    private Vector3 moveDir;
    private Vector3 rollDir;
    private float rollSpeed;
    private bool wasMoving = false;
    private float stopSlowdownTime = 0.1f; // Время замедления при остановке
    private float currentStopSlowdownTime = 0f;

    [Header("Roll")]
    [SerializeField] private LayerMask dashLayerMask;
    public float MoveSpeed;
    public float RollCD = 2.0f;
    private bool isCooldown = false;
    private float cooldownTimer = 0.0f;
    public float nextFire = 0;

    [Header("Attack")]
    [SerializeField] private LayerMask enemyLayerMask;
    public Transform attackPoint;
    public GameObject AttackEffectPrefab;
    private bool hasAttacked = false;
    public Collider2D weaponCollider;
    public Bow currentBow; // Текущий лук
public GameObject objectToDisableOnLMB;
public GameObject objectToEnableOnLMB;
public GameObject objectToDisableOnRMB;
public GameObject objectToEnableOnRMB;




    // Параметры атаки
    public Weapon currentWeapon;
    public SpriteRenderer weaponSpriteRenderer;
    public Material[] weaponMaterials; // Массив материалов для разных оружий
    public float attackRadius = 3f;
    public float attackDelay = 0.5f;
    public float attackDamage = 10f;
    public float nextAttackTime = 0.1f;
    public float attackCooldown = 0.5f;
    private bool hasDealtDamage = false;
    private bool isAttacking = false;
    private float attackAnimationTime = 0f;

    [Header("Area Attack")]
    [SerializeField] private bool areaAttackOn = false;
    [SerializeField] private float areaDamage = 0f;
    [SerializeField] private float areaRadius = 0f;
    public GameObject areaAttackEffectPrefab;


    [Header("UI")]
    public Image imageCooldown;

    // Animator
    public Animator animator;

    [Header("AudioMixer")]

    public AudioMixer audioMixer;
    public AudioMixerGroup sfxGroup;
void Start()
{
    rigidbody2D = GetComponent<Rigidbody2D>();
    state = State.Normal;
    imageCooldown = GameObject.FindWithTag("CDUI").GetComponent<Image>();

    // Стартовое оружие 
    


    // Попытка автоматического поиска компонента Animator среди дочерних объектов
    if(animator == null)
    {
        animator = GetComponentInChildren<Animator>();
        if(animator == null)
        {
            // Если компонент не найден среди дочерних объектов, попытаемся найти его в дочернем объекте "Capsule"
            Transform capsule = transform.Find("Capsule");
            if(capsule != null)
            {
                animator = capsule.GetComponent<Animator>();
                if(animator == null)
                {
                    Debug.LogError("Компонент Animator не найден на дочернем объекте 'Capsule' у объекта " + gameObject.name);
                }
            }
            else
            {
                Debug.LogError("Дочерний объект 'Capsule' не найден у объекта " + gameObject.name);
            }
        }
    }
}


void Awake()
{
    rigidbody2D = GetComponent<Rigidbody2D>();
    state = State.Normal;
    imageCooldown = GameObject.FindWithTag("CDUI").GetComponent<Image>();

    // Попытка автоматического поиска компонента Animator среди дочерних объектов
    if(animator == null)
    {
        animator = GetComponentInChildren<Animator>();
        if(animator == null)
        {
            // Если компонент не найден среди дочерних объектов, попытаемся найти его в дочернем объекте "Capsule"
            Transform capsule = transform.Find("Capsule");
            if(capsule != null)
            {
                animator = capsule.GetComponent<Animator>();
                if(animator == null)
                {
                    Debug.LogError("Компонент Animator не найден на дочернем объекте 'Capsule' у объекта " + gameObject.name);
                }
            }
            else
            {
                Debug.LogError("Дочерний объект 'Capsule' не найден у объекта " + gameObject.name);
            }
        }
    }
}


public void UpdateWeaponSprite(Sprite weaponSprite)
{
    if (weaponSpriteRenderer != null && weaponSprite != null)
    {
        weaponSpriteRenderer.sprite = weaponSprite;
    }
}


public void UseBow()
{
    if (currentBow != null && bowController != null)
    {
        if (!bowController.IsReloading)
        {
            bowController.ShootArrow();
        }
        else
        {
            Debug.Log("Лук еще перезаряжается!");
        }
    }
    else
    {
        Debug.Log("Лук не выбран или контроллер лука не найден!");
    }
}



    public void UseWeapon()
    {
        if (currentWeapon != null)
        {
            // Здесь может быть логика использования другого оружия
            Debug.Log("Используем другое оружие!");
        }
        else
        {
            Debug.Log("Оружие не выбрано!");
        }
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
                if (nextFire > 0f)
                {
                    nextFire -= Time.deltaTime;
                }
                    // Проверка на использование ролла
                    if (nextFire <= 0f && Input.GetKeyDown(KeyCode.Space))
                    {
                        StartRollCooldown();
                    }



        // Устанавливаем параметр анимации для ходьбы в зависимости от направления движения
        WalkAnim.SetFloat("Speed", moveDir.magnitude);

        // Определяем направление взгляда персонажа и устанавливаем соответствующий параметр анимации
        if (moveX > 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (moveX < 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }


        if (wasMoving && moveDir == Vector3.zero)
        {
            // Если персонаж остановился после движения, начинаем замедление

            currentStopSlowdownTime = stopSlowdownTime; // Устанавливаем текущее время замедления

            // Дополнительные эффекты или действия можно добавить здесь
        }

        // Если персонаж все еще замедляется, обновляем его скорость
        if (currentStopSlowdownTime > 0)
        {
            // Замедляем скорость движения персонажа
            moveDir *= 0.5f; // Примерно на половину

            // Уменьшаем время замедления
            currentStopSlowdownTime -= Time.deltaTime;
        }

        // Обновляем флаг wasMoving на текущее состояние движения
        wasMoving = moveDir != Vector3.zero;

    if (Input.GetKeyDown(KeyCode.Mouse1))
    {
        ToggleSpriteRendererOnRMB();
        UseBow();
    }

            // Проверка на использование атаки по области
            if (Input.GetKeyDown(KeyCode.Q))
            {
                if (Time.time >= nextAttackTime && !isAttacking)
                {
                    // Начинаем атаку
                    isAttacking = true;

                    // Проигрываем звук атаки
                    if (currentWeapon != null && currentWeapon.attackSound != null)
                    {
                            PlaySound(currentWeapon.attackSound);
                    }

                    // Устанавливаем параметры атаки
                    nextAttackTime = Time.time + currentWeapon.attackCooldown;
                    animator.SetBool("Attack", true);
                    attackAnimationTime = Time.time + currentWeapon.attackDuration;

                    // Передаем параметры атаки оружия
                    attackRadius = currentWeapon.attackRadius;
                    attackDamage = currentWeapon.attackDamage;

                    // Если атака по области активирована, устанавливаем параметры для нее
                    if (areaAttackOn)
                    {
                        areaRadius = currentWeapon.areaRadius;
                        areaDamage = currentWeapon.areaDamage;
                    }
                }
                    // Проигрываем анимацию атаки и наносим урон врагам
                    if (isAttacking)
                    {
                        // Проверяем коллизии с врагами и наносим урон
                        DealDamageToEnemies();

                        // Если атака по области активирована, также наносим урон по области
                        if (areaAttackOn)
                        {
                            DealAreaDamage();
                        }

                        // Если анимация атаки завершилась
                        
                    }
            }

                // Проверка на возможность атаки
                if (Input.GetMouseButtonDown(0) && Time.time >= nextAttackTime && !isAttacking)
                    {
                        // Начинаем атаку
                        isAttacking = true;

                        // Проигрываем звук атаки
                        if (currentWeapon != null && currentWeapon.attackSound != null)
                        {
                            PlaySound(currentWeapon.attackSound);
                        }
                        ToggleSpriteRendererOnLMB();
                        // Устанавливаем параметры атаки
                        nextAttackTime = Time.time + currentWeapon.attackCooldown;
                        animator.SetBool("Attack", true);
                        attackAnimationTime = Time.time + currentWeapon.attackDuration;

                        // Передаем параметры атаки оружия
                        attackRadius = currentWeapon.attackRadius;
                        attackDamage = currentWeapon.attackDamage;

                        // Устанавливаем скорость атаки в аниматоре
                        float attackSpeed = currentWeapon.attackSpeed;
                        float animationSpeed = 1.0f / attackSpeed; // Вычисляем скорость анимации в обратной пропорции к скорости атаки
                        animator.SetFloat("Speed", animationSpeed);

                        // Сбрасываем флаг перед новой атакой
                        hasDealtDamage = false;
                    }

                    void PlaySound(AudioClip clip)
                        {
                            GameObject soundObject = new GameObject("AttackSound");
                            AudioSource audioSource = soundObject.AddComponent<AudioSource>();
                            audioSource.clip = clip;
                            audioSource.outputAudioMixerGroup = sfxGroup;
                            audioSource.Play();

                            Destroy(soundObject, clip.length);
                        }

               // Проигрываем анимацию атаки и наносим урон врагам
                if (isAttacking)
                    {
                        // Проверяем коллизии с врагами и наносим урон
                        DealDamageToEnemies();

                        // Если анимация атаки завершилась
                        if (Time.time >= attackAnimationTime)
                        {
                            animator.SetBool("Attack", false);
                            isAttacking = false;
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

                if (cooldownTimer <= 0)
                {
                    isCooldown = false;
                    imageCooldown.fillAmount = 0f;
                }
                else
                {
                    float cooldownFillSpeed = 1 / RollCD; // Скорость заполнения fillAmount
                    imageCooldown.fillAmount -= cooldownFillSpeed * Time.deltaTime; // Уменьшаем fillAmount плавно
                }
                break;

        }
    }
        
// Метод для нанесения урона врагам
private void DealDamageToEnemies()
{
    Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(weaponCollider.transform.position, attackRadius, enemyLayerMask);

    foreach (Collider2D enemy in hitEnemies)
    {
        Enemy enemyComponent = enemy.GetComponent<Enemy>();
        if (enemyComponent != null && !hasDealtDamage)
        {
            // Нанесение урона врагу
            enemyComponent.TakeDamage(attackDamage);
            hasDealtDamage = true; // Устанавливаем флаг, чтобы предотвратить нанесение дополнительного урона
        }
        
        // Проверяем, если это босс и не было нанесено урона
        Boss bossComponent = enemy.GetComponent<Boss>();
        if (bossComponent != null && !hasDealtDamage)
        {
            // Нанесение урона боссу
            bossComponent.TakeDamage(attackDamage);
            hasDealtDamage = true; // Устанавливаем флаг, чтобы предотвратить нанесение дополнительного урона
        }
    }
}


private void ToggleSpriteRendererOnLMB()
{
    if (objectToDisableOnLMB != null)
    {
        objectToDisableOnLMB.GetComponent<SpriteRenderer>().enabled = false;
    }
    if (objectToEnableOnLMB != null)
    {
        objectToEnableOnLMB.GetComponent<SpriteRenderer>().enabled = true;
    }
}

private void ToggleSpriteRendererOnRMB()
{
    if (objectToDisableOnRMB != null)
    {
        objectToDisableOnRMB.GetComponent<SpriteRenderer>().enabled = false;
    }
    if (objectToEnableOnRMB != null)
    {
        objectToEnableOnRMB.GetComponent<SpriteRenderer>().enabled = true;
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

    private void StartRollCooldown()
    {
        nextFire = RollCD;
        rollSpeed = 30f;
        state = State.Rolling;
        isCooldown = true;
        cooldownTimer = RollCD;
        StartCoroutine(ReduceCooldownOverTime(RollCD));
    }

    private IEnumerator ReduceCooldownOverTime(float cooldownTime)
    {
        float startTime = Time.time;
        while (Time.time < startTime + cooldownTime)
        {
            imageCooldown.fillAmount = 1f - ((Time.time - startTime) / RollCD);
            yield return null;
        }
        imageCooldown.fillAmount = 0f;
        isCooldown = false;
        state = State.Normal;
    }


// Метод для нанесения урона по области
private void DealAreaDamage()
{
    Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, areaRadius, enemyLayerMask);

    foreach (Collider2D enemy in hitEnemies)
    {
        Enemy enemyComponent = enemy.GetComponent<Enemy>();
        if (enemyComponent != null)
        {
            // Нанесение урона врагу по области
            enemyComponent.TakeDamage(areaDamage);
        }
    }

    // Проверяем попадание анимации атаки во врага и наносим урон
    Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, areaRadius, enemyLayerMask);
    foreach (Collider2D collider in hitColliders)
    {
        Enemy enemy = collider.GetComponent<Enemy>();
        if (enemy != null)
        {
            // Нанесение урона врагу
            enemy.TakeDamage(areaDamage);
        }
    }

    // Инстанциируем эффект атаки по области в точке попадания
    if (areaAttackEffectPrefab != null)
    {
        GameObject areaEffect = Instantiate(areaAttackEffectPrefab, transform.position, Quaternion.identity);

        // Изменяем размер эффекта в зависимости от радиуса атаки
        areaEffect.transform.localScale = new Vector3(areaRadius * 2, areaRadius * 2, 1);

        // Удаляем префаб через 1 секунду
        Destroy(areaEffect, 1f);
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
