using UnityEngine;

public class SimpleSaveManager : MonoBehaviour
{
    private static SimpleSaveManager instance;
    public static SimpleSaveManager Instance { get { return instance; } }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveGame(Vector3 playerPosition)
    {
        PlayerPrefs.SetFloat("PlayerPosX", playerPosition.x);
        PlayerPrefs.SetFloat("PlayerPosY", playerPosition.y);
        PlayerPrefs.SetFloat("PlayerPosZ", playerPosition.z);
        PlayerPrefs.SetInt("HasSave", 1);
        PlayerPrefs.Save();

        Debug.Log("[SAVE] Сохранена позиция: " + playerPosition);
    }

    public Vector3 GetSavedPosition()
    {
        if (HasSave())
        {
            float x = PlayerPrefs.GetFloat("PlayerPosX", 0);
            float y = PlayerPrefs.GetFloat("PlayerPosY", 0);
            float z = PlayerPrefs.GetFloat("PlayerPosZ", 0);
            Vector3 pos = new Vector3(x, y, z);
            Debug.Log("[LOAD] Загружена позиция: " + pos);
            return pos;
        }
        Debug.Log("[LOAD] Сохранение не найдено");
        return Vector3.zero;
    }

    public bool HasSave()
    {
        return PlayerPrefs.GetInt("HasSave", 0) == 1;
    }

    public void DeleteSave()
    {
        PlayerPrefs.DeleteKey("PlayerPosX");
        PlayerPrefs.DeleteKey("PlayerPosY");
        PlayerPrefs.DeleteKey("PlayerPosZ");
        PlayerPrefs.DeleteKey("HasSave");
        PlayerPrefs.Save();
        Debug.Log("[SAVE] Сохранение удалено");
    }
}