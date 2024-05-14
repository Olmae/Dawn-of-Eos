using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveManager : MonoBehaviour 
{
    // Метод для сохранения данных
    public void SaveData(List<Achievement> achievements, int money, int enemiesKilled)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/save.dat";
        
        // Проверяем, существует ли файл, и если нет, создаем его
        if (!File.Exists(path))
        {
            File.Create(path).Close();
            Debug.Log("Creating new save file at: " + path);
        }

        FileStream stream = new FileStream(path, FileMode.Create);

        SaveData data = new SaveData(achievements, money, enemiesKilled);

        formatter.Serialize(stream, data);
        stream.Close();
        Debug.Log("Saving data to: " + path);

    }

    // Метод для загрузки данных
    public SaveData LoadData()
    {
        string path = Application.persistentDataPath + "/save.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            SaveData data = formatter.Deserialize(stream) as SaveData;
            stream.Close();

            return data;
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }
}
