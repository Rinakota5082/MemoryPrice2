using UnityEngine;

public class PlayerPositionSaver : MonoBehaviour
{
    [Header("Settings")]
    [Min(0.1f)]
    public float saveInterval = 5f;

    [Header("VR Setup")]
    [Tooltip("Объект, позицию которого сохранять (камера или риг)")]
    public Transform objectToSave;

    private float saveTimer = 0f;

    private void Start()
    {
        if (objectToSave == null)
            objectToSave = transform;

        Debug.Log("[PlayerPositionSaver] Start: objectToSave = " + objectToSave.name);
    }

    private void Update()
    {
        if (objectToSave == null || SimpleSaveManager.Instance == null)
            return;

        saveTimer += Time.deltaTime;
        if (saveTimer >= saveInterval)
        {
            saveTimer = 0f;
            SaveCurrentPosition();
        }
    }

    private void OnDisable()
    {
        SaveCurrentPosition();
    }

    private void OnApplicationQuit()
    {
        SaveCurrentPosition();
    }

    private void SaveCurrentPosition()
    {
        if (objectToSave == null || SimpleSaveManager.Instance == null)
            return;

        Vector3 pos = objectToSave.position;
        Debug.Log("PlayerPositionSaver: Saved position = " + pos);
        SimpleSaveManager.Instance.SaveGame(pos);
    }

    public void LoadPosition()
    {
        if (objectToSave == null || SimpleSaveManager.Instance == null)
            return;

        if (SimpleSaveManager.Instance.HasSave())
        {
            Vector3 savedPos = SimpleSaveManager.Instance.GetSavedPosition();
            Debug.Log("PlayerPositionSaver: Loaded position = " + savedPos);
            objectToSave.position = savedPos;
        }
        else
        {
            Debug.Log("PlayerPositionSaver: No save found, position not loaded.");
        }
    }
}