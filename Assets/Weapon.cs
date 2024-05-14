using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Custom/Weapon")]
[System.Serializable]
public class Weapon : ScriptableObject
{
    public Sprite weaponSprite;

    public string weaponName; // Имя оружия
    public float attackDamage = 10f; // Урон
    public float attackRadius = 3f; // Радиус атаки
    public float areaDamage = 0f; // Урон по области
    public float areaRadius = 0f; // Радиус области атаки
    public bool areaAttackOn = false;
    public float attackCooldown = 0.5f; // Время между атаками
    public AudioClip attackSound; // Звук атаки

    // Дополнительные параметры оружия
    public float attackSpeed = 1.0f; // Скорость атаки
    public float attackDuration = 1.0f;
    public GameObject attackEffect; // Эффект атаки
    public float attackAnimationLength; // Длина анимации атаки

    // Другие параметры оружия, такие как цвет, эффекты и т.д., могут быть добавлены здесь
}
