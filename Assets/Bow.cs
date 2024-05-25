using UnityEngine;

[CreateAssetMenu(fileName = "New Bow", menuName = "Custom/Bow")]
[System.Serializable]
public class Bow : ScriptableObject
{
    public Sprite bowSprite;

    public string bowName; // Имя лука
    public float arrowDamage = 20f; // Урон стрелы
    public float arrowSpeed = 10f; // Скорость полета стрелы
    public AudioClip shootSound; // Звук выстрела
    public GameObject arrowPrefab; // Префаб стрелы

    // Дополнительные параметры лука
    public float reloadTime = 1.0f; // Время перезарядки
    public float maxRange = 20f; // Максимальная дальность стрельбы

    // Другие параметры лука, такие как цвет, эффекты и т.д., могут быть добавлены здесь
}
