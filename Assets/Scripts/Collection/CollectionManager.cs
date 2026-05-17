using System.Collections.Generic;
using UnityEngine;

public class CollectionManager : MonoBehaviour
{
    // Singleton-паттерн для доступа из любого скрипта
    public static CollectionManager Instance { get; private set; }
    // Список всех возможных предметов в игре
    public List<Collection> allCollectibleItems;
    // Хранилище для ID's уже собранных предметов
    private HashSet<string> unlockedItems = new HashSet<string>();
    private string saveKey = "CollectedItems";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadCollection();
    }

    // Вызывается, когда игрок нашел предмет в уровне
    public void UnlockItem(string itemID)
    {
        if (unlockedItems.Contains(itemID)) return; // Уже есть в коллекции
        
        unlockedItems.Add(itemID);
        SaveCollection();
        Debug.Log($"Предмет добавлен в коллекцию: {itemID}");
    }
    public bool IsItemUnlocked(string itemID)
    {
        return unlockedItems.Contains(itemID);
    }

    // Получить штамп о состоянии для всех предметов
    public bool[] GetAllUnlockStatus()
    {
        bool[] statuses = new bool[allCollectibleItems.Count];
        for (int i = 0; i < allCollectibleItems.Count; i++)
        {
            statuses[i] = unlockedItems.Contains(allCollectibleItems[i].itemID);
        }
        return statuses;
    }
    private void SaveCollection()
    {
        string[] ids = new string[unlockedItems.Count];
        unlockedItems.CopyTo(ids);
        string json = JsonUtility.ToJson(new SerializationWrapper<string> { items = ids });
        PlayerPrefs.SetString(saveKey, json);
        PlayerPrefs.Save();
    }

    // Загружаем список ID's из PlayerPrefs
    private void LoadCollection()
    {
        if (!PlayerPrefs.HasKey(saveKey)) return;
        string json = PlayerPrefs.GetString(saveKey);
        var wrapper = JsonUtility.FromJson<SerializationWrapper<string>>(json);
        unlockedItems.Clear();
        foreach (string id in wrapper.items)
        {
            unlockedItems.Add(id);
        }
    }
    
    // Вспомогательный класс для JSON-сериализации массивов
    [System.Serializable]
    class SerializationWrapper<T>
    {
        public T[] items;
    }
}