using System.IO;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public float playerPosX;
    public float playerPosY;
    public float playerPosZ;
}

public class SimpleSaveManager : MonoBehaviour
{
    public static SimpleSaveManager Instance { get; private set; }

    private string saveFilePath;
    private SaveData currentSave;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        saveFilePath = Path.Combine(Application.persistentDataPath, "player_position_save.json");

        // Попробуем загрузить сохранённые данные
        Load();
    }

    // Проверяем, есть ли вообще сохранение
    public bool HasSave()
    {
        bool has = File.Exists(saveFilePath);
        Debug.Log("SimpleSaveManager.HasSave: " + has);
        return has;
    }

    // Сохраняем позицию
    public void SaveGame(Vector3 position)
    {
        if (currentSave == null)
            currentSave = new SaveData();

        currentSave.playerPosX = position.x;
        currentSave.playerPosY = position.y;
        currentSave.playerPosZ = position.z;

        string json = JsonUtility.ToJson(currentSave, true);
        File.WriteAllText(saveFilePath, json);

        Debug.Log("SimpleSaveManager: Saved position to file: " + position + " (path: " + saveFilePath + ")");
    }

    // Возвращаем сохранённую позицию
    public Vector3 GetSavedPosition()
    {
        if (!HasSave())
            return Vector3.zero;

        string json = File.ReadAllText(saveFilePath);
        currentSave = JsonUtility.FromJson<SaveData>(json);

        Vector3 pos = new Vector3(
            currentSave.playerPosX,
            currentSave.playerPosY,
            currentSave.playerPosZ
        );

        Debug.Log("SimpleSaveManager: Loaded position from file: " + pos);
        return pos;
    }

    // Загрузить при старте (если нужно)
    private void Load()
    {
        if (!HasSave())
        {
            currentSave = new SaveData();
            Debug.Log("SimpleSaveManager: No save file, created new empty SaveData.");
        }
        else
        {
            string json = File.ReadAllText(saveFilePath);
            currentSave = JsonUtility.FromJson<SaveData>(json);
            Debug.Log("SimpleSaveManager: Loaded initial save data.");
        }
    }
}