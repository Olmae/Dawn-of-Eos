using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private Dictionary<string, float> itemEffects = new Dictionary<string, float>();

    // Добавление предмета и его эффекта в инвентарь
    public void AddItem(string itemName, float damageModifier)
    {
        if (!itemEffects.ContainsKey(itemName))
        {
            itemEffects.Add(itemName, damageModifier);
            // Применить эффект предмета к персонажу (в данном случае урону)
            ApplyItemEffects();
        }
        else
        {
            Debug.LogWarning("Предмет уже находится в инвентаре: " + itemName);
        }
    }

    // Удаление предмета из инвентаря
    public void RemoveItem(string itemName)
    {
        if (itemEffects.ContainsKey(itemName))
        {
            itemEffects.Remove(itemName);
            // Применить эффекты предметов к персонажу (в данном случае урону)
            ApplyItemEffects();
        }
        else
        {
            Debug.LogWarning("Предмет не найден в инвентаре: " + itemName);
        }
    }

    // Применение эффектов всех предметов из инвентаря к персонажу
    private void ApplyItemEffects()
    {
        // Получить контроллер персонажа
        CharacterController2D characterController = GetComponent<CharacterController2D>();

        // Суммировать эффекты всех предметов на урон персонажа
        float totalDamageModifier = 1f;
        foreach (var itemEffect in itemEffects)
        {
            totalDamageModifier *= itemEffect.Value;
        }

        // Применить модификатор урона к персонажу
        characterController.attackDamage = totalDamageModifier * characterController.attackDamage;
    }
}
