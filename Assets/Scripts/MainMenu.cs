using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    public UnityEngine.UI.Button continueButton;
    public string gameSceneName = "SampleScene"; // название вашей игровой сцены
    public GameObject loadingScreen;

    void Start()
    {
        if (continueButton == null)
        {
            Debug.LogError("[MENU] continueButton не назначен в Inspector!");
            return;
        }

        // Проверяем, есть ли сохранение
        bool hasSave = SimpleSaveManager.Instance != null && SimpleSaveManager.Instance.HasSave();
        continueButton.interactable = hasSave;

        Debug.Log("[MENU] Кнопка Continue активна: " + hasSave);

        continueButton.onClick.AddListener(OnContinueClicked);
    }

    public void OnContinueClicked()
    {
        Debug.Log("[MENU] Нажата кнопка Continue");
        StartCoroutine(LoadGameSceneAndRestorePosition());
    }

    private IEnumerator LoadGameSceneAndRestorePosition()
    {
        Debug.Log("[MENU] Начинаем загрузку сцены: " + gameSceneName);

        if (loadingScreen != null)
            loadingScreen.SetActive(true);

        // Загружаем игровую сцену
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(gameSceneName);
        asyncLoad.allowSceneActivation = true;

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        Debug.Log("[MENU] Сцена загружена, ждём инициализации...");

        // Ждём 2 кадра, чтобы все скрипты инициализировались
        yield return null;
        yield return null;

        // Находим игрока
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            Debug.Log("[MENU] Игрок найден: " + player.name);

            // Пробуем найти скрипт сохранения на игроке
            PlayerPositionSaver saver = player.GetComponent<PlayerPositionSaver>();
            if (saver != null)
            {
                Debug.Log("[MENU] Найден PlayerPositionSaver, загружаем позицию...");
                saver.LoadPosition();
            }
            else
            {
                // Если скрипта нет, загружаем позицию напрямую
                Vector3 savedPos = SimpleSaveManager.Instance.GetSavedPosition();
                player.transform.position = savedPos;
                Debug.Log("[MENU] Позиция загружена напрямую: " + savedPos);
            }
        }
        else
        {
            Debug.LogError("[MENU] Игрок не найден! Убедитесь, что у объекта игрока стоит тег Player");
        }

        if (loadingScreen != null)
            loadingScreen.SetActive(false);

        Debug.Log("[MENU] Загрузка завершена");
    }

    public void OnNewGame()
    {
        Debug.Log("[MENU] Новая игра");
        if (SimpleSaveManager.Instance != null)
            SimpleSaveManager.Instance.DeleteSave();
        SceneManager.LoadScene(gameSceneName);
    }

    public void OnExit()
    {
        Debug.Log("[MENU] Выход из игры");
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
