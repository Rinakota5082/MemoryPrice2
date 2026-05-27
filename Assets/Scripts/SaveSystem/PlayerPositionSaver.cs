using UnityEngine;

public class PlayerPositionSaver : MonoBehaviour
{
    [Header("Настройки")]
    public bool loadOnStart = true;
    public bool autoSaveOnDestroy = true;  // Сохранять при выходе из игры

    private Transform playerTransform;

    void Awake()
    {
        playerTransform = transform;
    }

    void Start()
    {
        if (loadOnStart && SimpleSaveManager.Instance != null && SimpleSaveManager.Instance.HasSave())
        {
            LoadPosition();
        }
    }

    void OnDestroy()
    {
        if (autoSaveOnDestroy && SimpleSaveManager.Instance != null)
        {
            SavePosition();
        }
    }

    /// <summary>
    /// Сохраняет текущую позицию и поворот
    /// </summary>
    public void SavePosition()
    {
        if (SimpleSaveManager.Instance == null)
        {
            Debug.LogWarning("⚠️ SimpleSaveManager.Instance = null! Не удалось сохранить.");
            return;
        }

        SimpleSaveManager.Instance.SaveGame(playerTransform);
    }

    /// <summary>
    /// Загружает сохранённую позицию и поворот
    /// </summary>
    public void LoadPosition()
    {
        if (SimpleSaveManager.Instance == null)
        {
            Debug.LogWarning("⚠️ SimpleSaveManager.Instance = null! Не удалось загрузить.");
            return;
        }

        if (!SimpleSaveManager.Instance.HasSave())
        {
            Debug.Log("ℹ️ Нет сохранения для загрузки.");
            return;
        }

        Vector3 savedPos = SimpleSaveManager.Instance.GetSavedPosition();
        Quaternion savedRot = SimpleSaveManager.Instance.GetSavedRotation();

        playerTransform.position = savedPos;
        playerTransform.rotation = savedRot;

        Debug.Log($"📀 Позиция загружена: {savedPos}, Поворот: {savedRot.eulerAngles}");
    }

    /// <summary>
    /// Очищает сохранение (для отладки)
    /// </summary>
    public void ClearSave()
    {
        if (SimpleSaveManager.Instance != null)
            SimpleSaveManager.Instance.DeleteSave();
    }
}