using UnityEngine;
using UnityEngine.SceneManagement;

public class SimpleSaveManager : MonoBehaviour
{
    public static SimpleSaveManager Instance { get; private set; }

    // Ключи для PlayerPrefs
    private const string SAVE_EXISTS_KEY = "SaveExists";
    private const string SAVED_SCENE_KEY = "SavedScene";
    private const string SAVED_POS_X = "SavedPosX";
    private const string SAVED_POS_Y = "SavedPosY";
    private const string SAVED_POS_Z = "SavedPosZ";
    private const string SAVED_ROT_X = "SavedRotX";
    private const string SAVED_ROT_Y = "SavedRotY";
    private const string SAVED_ROT_Z = "SavedRotZ";

    [Header("Настройки")]
    public string gameSceneName = "SampleScene";
    public string menuSceneName = "MainMenuScene";

    private void Awake()
    {
        // Синглтон — чтобы менеджер был один на всю игру
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Сохраняет позицию и поворот игрока, а также текущую сцену
    /// </summary>
    public void SaveGame(Transform playerTransform)
    {
        if (playerTransform == null)
        {
            Debug.LogWarning("❌ Не удалось сохранить: playerTransform = null");
            return;
        }

        // Сохраняем позицию
        PlayerPrefs.SetFloat(SAVED_POS_X, playerTransform.position.x);
        PlayerPrefs.SetFloat(SAVED_POS_Y, playerTransform.position.y);
        PlayerPrefs.SetFloat(SAVED_POS_Z, playerTransform.position.z);

        // Сохраняем поворот
        PlayerPrefs.SetFloat(SAVED_ROT_X, playerTransform.eulerAngles.x);
        PlayerPrefs.SetFloat(SAVED_ROT_Y, playerTransform.eulerAngles.y);
        PlayerPrefs.SetFloat(SAVED_ROT_Z, playerTransform.eulerAngles.z);

        // Сохраняем имя текущей сцены
        PlayerPrefs.SetString(SAVED_SCENE_KEY, SceneManager.GetActiveScene().name);

        // Отмечаем, что сохранение существует
        PlayerPrefs.SetInt(SAVE_EXISTS_KEY, 1);

        PlayerPrefs.Save();

        Debug.Log($"💾 Игра сохранена! Позиция: {playerTransform.position}, Сцена: {SceneManager.GetActiveScene().name}");
    }

    /// <summary>
    /// Загружает позицию игрока
    /// </summary>
    public Vector3 GetSavedPosition()
    {
        float x = PlayerPrefs.GetFloat(SAVED_POS_X, 0f);
        float y = PlayerPrefs.GetFloat(SAVED_POS_Y, 0f);
        float z = PlayerPrefs.GetFloat(SAVED_POS_Z, 0f);
        return new Vector3(x, y, z);
    }

    /// <summary>
    /// Загружает поворот игрока
    /// </summary>
    public Quaternion GetSavedRotation()
    {
        float x = PlayerPrefs.GetFloat(SAVED_ROT_X, 0f);
        float y = PlayerPrefs.GetFloat(SAVED_ROT_Y, 0f);
        float z = PlayerPrefs.GetFloat(SAVED_ROT_Z, 0f);
        return Quaternion.Euler(x, y, z);
    }

    /// <summary>
    /// Проверяет, есть ли сохранение
    /// </summary>
    public bool HasSave()
    {
        return PlayerPrefs.GetInt(SAVE_EXISTS_KEY, 0) == 1;
    }

    /// <summary>
    /// Удаляет сохранение (при начале новой игры)
    /// </summary>
    public void DeleteSave()
    {
        PlayerPrefs.DeleteKey(SAVE_EXISTS_KEY);
        PlayerPrefs.DeleteKey(SAVED_SCENE_KEY);
        PlayerPrefs.DeleteKey(SAVED_POS_X);
        PlayerPrefs.DeleteKey(SAVED_POS_Y);
        PlayerPrefs.DeleteKey(SAVED_POS_Z);
        PlayerPrefs.DeleteKey(SAVED_ROT_X);
        PlayerPrefs.DeleteKey(SAVED_ROT_Y);
        PlayerPrefs.DeleteKey(SAVED_ROT_Z);
        PlayerPrefs.Save();

        Debug.Log("🗑️ Сохранение удалено!");
    }

    /// <summary>
    /// Возвращает имя сохранённой сцены
    /// </summary>
    public string GetSavedSceneName()
    {
        return PlayerPrefs.GetString(SAVED_SCENE_KEY, gameSceneName);
    }
}