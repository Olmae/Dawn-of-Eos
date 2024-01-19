using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterController2D : MonoBehaviour
{
    private enum State
    {
        Normal,
        Rolling,
        Attacking,
        Cooldown
    }

    private Rigidbody2D rigidbody2D;
    private Vector3 moveDir;
    private Vector3 rollDir;
    
    private float rollSpeed;
    [SerializeField] private LayerMask dashLayerMask;
    [SerializeField] public Image imageCooldown;
    public float attackRadius = 3f;
    public float attackDelay = 0.5f;
    public LayerMask enemyLayerMask;
    public Transform attackPoint;
    public float attackDamage = 10f;
    private float nextAttackTime = 0.1f;
    public float attackCooldown = 0.5f;

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
    }

    void Update()
    {
        switch (state)
        {
            case State.Normal:
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
                        cooldownTimer = 2.0f;
                    }
                }

                if (Input.GetMouseButtonDown(0) && Time.time >= nextAttackTime)
                {
                    UnityEngine.Debug.Log("Нажато ЛКМ");

                    // Set the next attack time
                    nextAttackTime = Time.time + attackCooldown;

                    // Set the attack animation parameter to true
                    UnityEngine.Debug.Log("Setting Attack parameter to true");
                    animator.SetBool("Attack", true);
                    attackAnimationTime = Time.time + attackAnimationLength;
                    isAttacking = true;
                }

                // Check for the end of the attack animation
                if (isAttacking && Time.time >= attackAnimationTime)
                {
                    UnityEngine.Debug.Log("Setting Attack parameter to false");
                    animator.SetBool("Attack", false);
                    isAttacking = false;
                }

                // Damage enemies during the entire attack animation
                if (isAttacking)
                {
                    Collider2D[] hitColliders = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, enemyLayerMask);
                    foreach (Collider2D collider in hitColliders)
                    {
                        Enemy enemy = collider.GetComponent<Enemy>();
                        if (enemy != null)
                        {
                            // Damage enemy
                            enemy.TakeDamage(attackDamage);
                        }
                    }
                }

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
                float rollSpeedDropMult = 5f;
                rollSpeed -= rollSpeed * rollSpeedDropMult * Time.deltaTime;
                float rollSpeedDropMin = 15f;
                if (rollSpeed < rollSpeedDropMin)
                {
                    state = State.Normal;
                }
                break;
        }
    }

    private void FixedUpdate()
    {
        switch (state)
        {
            case State.Normal:
                rigidbody2D.velocity = moveDir * MoveSpeed;
                break;
            case State.Rolling:
                rigidbody2D.velocity = moveDir * rollSpeed;
                break;
        }
    }

    void ApplyCooldown()
    {
        cooldownTimer -= Time.deltaTime;
        UnityEngine.Debug.Log("2");
        imageCooldown.fillAmount = cooldownTimer / RollCD;

        if (Time.time > nextFire)
        {
            isCooldown = false;
            imageCooldown.fillAmount = 0;   
            cooldownTimer = 0.0f;
            UnityEngine.Debug.Log("nextfire<=0");
        }   
    }

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
