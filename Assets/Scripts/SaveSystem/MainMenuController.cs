using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Button continueButton; // <-- Добавь префикс
    [SerializeField] private string defaultGameScene = "GameScene";

    private void Start()
    {
        continueButton.interactable = SaveSystem.HasSave();
        continueButton.onClick.AddListener(OnContinue);
    }

    private void OnContinue()
    {
        VRSaveData data = SaveSystem.LoadGame();
        if (data != null && !string.IsNullOrEmpty(data.sceneName))
            SceneManager.LoadScene(data.sceneName);
        else
            SceneManager.LoadScene(defaultGameScene);
    }
}