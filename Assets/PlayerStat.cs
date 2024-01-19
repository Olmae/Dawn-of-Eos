using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerStat : MonoBehaviour
{
    public float maxHealth = 6f;
    public float currentHealth;
    public Text healthText; // привязываем текст UI здоровья к этой переменной
    public GameObject pauseMenu;


// Задержка урона
private bool isHealing = false;
private float healingCooldown = 1f;
private float healingCooldownTimer = 0f;

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthText();
    }

private void FixedUpdate()
{
    if (currentHealth > maxHealth)
        currentHealth = maxHealth;

    // Handle healing cooldown
    if (isHealing)
    {
        healingCooldownTimer -= Time.deltaTime;
        if (healingCooldownTimer <= 0f)
        {
            // Сброс флага задержки хп
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

        // Устанавливаем флаг задержки хп
        isHealing = true;
        healingCooldownTimer = healingCooldown;

        UpdateHealthText(); // обновляем UI здоровья после получения урона
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
        healthText.text = "ХП: " + currentHealth.ToString(); // обновляем текст UI здоровья
    }
}
