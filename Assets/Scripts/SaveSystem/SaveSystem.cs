using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static readonly string SavePath = Application.persistentDataPath + "/vr_autosave.json";

    public static bool HasSave() => File.Exists(SavePath);

    public static void SaveGame(string sceneName, Transform playerTransform)
    {
        VRSaveData data = new()
        {
            sceneName = sceneName,
            posX = playerTransform.position.x,
            posY = playerTransform.position.y,
            posZ = playerTransform.position.z,
            rotX = playerTransform.eulerAngles.x,
            rotY = playerTransform.eulerAngles.y,
            rotZ = playerTransform.eulerAngles.z
        };
        File.WriteAllText(SavePath, JsonUtility.ToJson(data, true));
    }

    public static VRSaveData LoadGame()
    {
        if (!HasSave()) return null;
        return JsonUtility.FromJson<VRSaveData>(File.ReadAllText(SavePath));
    }

    public static void ClearSave()
    {
        if (HasSave()) File.Delete(SavePath);
    }
}