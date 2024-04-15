using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerStat : MonoBehaviour
{
    public float maxHealth = 6f;
    public float currentHealth;
    public Text healthText;
    public GameObject pauseMenu;

    private bool isHealing = false;
    private float healingCooldown = 1f;
    private float healingCooldownTimer = 0f;

    private void Start()
    {
        currentHealth = maxHealth;
        // Находим текст с тегом "Health" и присваиваем его переменной healthText
        healthText = GameObject.FindGameObjectWithTag("Health").GetComponent<Text>();
        UpdateHealthText();
    }

    private void FixedUpdate()
    {
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        if (isHealing)
        {
            healingCooldownTimer -= Time.deltaTime;
            if (healingCooldownTimer <= 0f)
            {
                isHealing = false;
            }
        }
    }

    public void TakeDamage(float damage)
    {
        if (!isHealing)
        {
            currentHealth -= damage;

            if (currentHealth <= 0)
            {
                Die();
            }

            isHealing = true;
            healingCooldownTimer = healingCooldown;

            UpdateHealthText();
        }
    }

    private void Die()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        Debug.Log("die");
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1f;
    }

    public void UpdateHealthText()
    {
        healthText.text = "ХП: " + currentHealth.ToString();
    }
}
