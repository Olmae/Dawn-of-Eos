using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickEUI : MonoBehaviour
{
    public GameObject targetObject;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Проверяем, столкнулись ли с объектом персонажа
        if (other.CompareTag("Player"))
        {
            targetObject.SetActive(true);
        }
    }
        private void OnTriggerExit2D(Collider2D other)
    {
        // Проверяем, вышел ли персонаж из зоны триггера
        if (other.CompareTag("Player"))
        {
            targetObject.SetActive(false);
        }
    }
    
}
