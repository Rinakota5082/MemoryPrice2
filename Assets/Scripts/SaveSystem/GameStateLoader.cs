using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateLoader : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";

    private void OnEnable() => SceneManager.sceneLoaded += ApplySave;
    private void OnDisable() => SceneManager.sceneLoaded -= ApplySave;

    private void ApplySave(Scene scene, LoadSceneMode mode)
    {
        VRSaveData data = SaveSystem.LoadGame();
        if (data == null || data.sceneName != scene.name) return;

        var player = GameObject.FindGameObjectWithTag(playerTag);
        if (player == null) return;

        player.transform.position = new Vector3(data.posX, data.posY, data.posZ);
        player.transform.rotation = Quaternion.Euler(data.rotX, data.rotY, data.rotZ);
    }
}