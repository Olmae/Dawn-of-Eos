using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomBox : MonoBehaviour
{
    public GameObject Box;


    void Start()
    {
        for (var i = 0; i < 10; i++) 
        {
            // Получаем позицию текущего объекта
            Vector3 currentPosition = transform.position;

            // Генерируем случайные смещения относительно текущей позиции
            var randomX = Random.Range(-10, 10);
            var randomY = Random.Range(-5, 5);

            // Применяем случайные смещения к текущей позиции
            currentPosition.x += randomX;
            currentPosition.y += randomY;

            // Создаем новый объект Box на полученной позиции
            Instantiate(Box, currentPosition, Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
