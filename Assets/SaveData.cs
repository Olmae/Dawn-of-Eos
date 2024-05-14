using UnityEngine;
using System.Collections.Generic; // Добавлено using-указание для использования List<>

[System.Serializable]
public class SaveData
{
    public List<Achievement> achievements;
    public int money;
    public int enemiesKilled;

    public SaveData(List<Achievement> _achievements, int _money, int _enemiesKilled)
    {
        achievements = _achievements;
        money = _money;
        enemiesKilled = _enemiesKilled;
    }
}
