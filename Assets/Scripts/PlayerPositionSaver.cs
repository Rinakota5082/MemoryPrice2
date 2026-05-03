using UnityEngine;

public class PlayerPositionSaver : MonoBehaviour
{
    [Header("Settings")]
    public float saveInterval = 5f;

    [Header("VR Setup")]
    [Tooltip("Объект, позицию которого сохранять (камера или риг)")]
    public Transform objectToSave;

    private float saveTimer = 0f;

    void Start()
    {
        if (objectToSave == null)
        {
            objectToSave = transform;
        }
    }

    void Update()
    {
        saveTimer += Time.deltaTime;
        if (saveTimer >= saveInterval)
        {
            saveTimer = 0f;

            if (SimpleSaveManager.Instance != null)
            {
                SimpleSaveManager.Instance.SaveGame(objectToSave.position);
            }
        }
    }

    void OnDisable()
    {
        if (SimpleSaveManager.Instance != null)
        {
            SimpleSaveManager.Instance.SaveGame(objectToSave.position);
        }
    }

    void OnApplicationQuit()
    {
        if (SimpleSaveManager.Instance != null)
        {
            SimpleSaveManager.Instance.SaveGame(objectToSave.position);
        }
    }

    public void LoadPosition()
    {
        if (SimpleSaveManager.Instance != null && SimpleSaveManager.Instance.HasSave())
        {
            Vector3 savedPos = SimpleSaveManager.Instance.GetSavedPosition();
            objectToSave.position = savedPos;
        }
    }
}