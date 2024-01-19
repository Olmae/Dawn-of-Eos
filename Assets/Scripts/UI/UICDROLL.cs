using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICDROLL : MonoBehaviour
{



[SerializeField]   public Image imageCooldown;
private bool isCooldown = false;
private float cooldownTime = 2.0f;
private float cooldownTimer = 0.0f;

void Start()
{
    imageCooldown.fillAmount = 0.0f;
}

void Update()
{
    if(isCooldown == true)
    {
        Debug.Log("true");
        ApplyCooldown();
    }
}

void ApplyCooldown(){
    cooldownTimer -= Time.deltaTime;
Debug.Log("3");
    if(cooldownTimer < 0.0f)
    {
        Debug.Log("1");
        isCooldown = false;
        imageCooldown.fillAmount = 0.0f;
    }
    else
    {
                Debug.Log("2");

        imageCooldown.fillAmount = cooldownTimer / cooldownTime;
    }
}

public void useSpell()
{

    if(Input.GetKeyDown(KeyCode.Space))
    {
         Debug.Log("CDUI");
        isCooldown = true;
    }
    else
    {
        isCooldown = false;
        cooldownTimer = cooldownTime;
    }
}
}


