using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICDROLL : MonoBehaviour
{

    public CharacterController2D characterController;


[SerializeField]   public Image imageCooldown;
private bool isCooldown = false;
private float cooldownTime = 2.0f;
private float cooldownTimer = 0.0f;

void Start()
{
    // Находим игрока с тегом "Player"
    GameObject player = GameObject.FindGameObjectWithTag("Player");

    // Получаем компонент CharacterController2D с игрока
    if (player != null)
    {
        characterController = player.GetComponent<CharacterController2D>();
        if (characterController == null)
        {
            Debug.LogError("CharacterController2D не найден на объекте с тегом 'Player'");
        }
    }
    else
    {
        Debug.LogError("Игрок с тегом 'Player' не найден");
    }

    // Устанавливаем fillAmount в начальное значение
    imageCooldown.fillAmount = 0.0f;
}

void Update()
{
    if (isCooldown)
    {
        // Рассчитываем fillAmount пропорционально оставшемуся времени кулдауна
        float remainingCooldownRatio = Mathf.Clamp(cooldownTimer / characterController.RollCD, 0f, 1f);
        imageCooldown.fillAmount = remainingCooldownRatio;

        // Если кулдаун завершился, сбрасываем флаг и устанавливаем fillAmount в 0
        if (cooldownTimer <= 0)
        {
            isCooldown = false;
            cooldownTimer = 0.0f;
            imageCooldown.fillAmount = 0.0f;
        }
        else
        {
            // Уменьшаем таймер кулдауна
            cooldownTimer -= Time.deltaTime;
        }
    }
}




void ApplyCooldown()
{
    cooldownTimer -= Time.deltaTime;

    if (cooldownTimer <= 0)
    {
        isCooldown = false;
        cooldownTimer = 0.0f;
        imageCooldown.fillAmount = 0.0f; // Устанавливаем fillAmount в ноль, когда кулдаун закончился
    }
    else
    {
        imageCooldown.fillAmount = 1 - (cooldownTimer / characterController.RollCD); // Рассчитываем fillAmount от 1 до 0 в зависимости от оставшегося времени кулдауна
    }
}




public void useSpell()
{
    if (Input.GetKeyDown(KeyCode.Space) && !isCooldown)
    {
        Debug.Log("CDUI");
        isCooldown = true;
        cooldownTimer = characterController.RollCD; // Устанавливаем время кулдауна при активации заклинания
    }
}

}


