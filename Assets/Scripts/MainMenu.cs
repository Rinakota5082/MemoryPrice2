using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    public UnityEngine.UI.Button continueButton;
    public string gameSceneName = "SampleScene";
    public GameObject loadingScreen;

    //void Start()
    //{
    //    if (continueButton == null)
    //    {
    //        Debug.LogError("[MENU] continueButton не назначен в Inspector!");
    //        return;
    //    }

    //    // Проверяем, есть ли сохранение
    //    bool hasSave = SimpleSaveManager.Instance != null && SimpleSaveManager.Instance.HasSave();
    //    continueButton.interactable = hasSave;

    //    Debug.Log("[MENU] Кнопка Continue активна: " + hasSave);

    //    // НЕ ДОБАВЛЯЕМ ЧЕРЕЗ КОД, чтобы не дублировать вызов
    //    // continueButton.onClick.AddListener(OnContinueClicked);
    //}



    void Start()
    {
        // ✅ ПРИНУДИТЕЛЬНЫЙ СБРОС ВСЕХ НАСТРОЕК ПРИ ЗАГРУЗКЕ МЕНЮ
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Остальной код (проверка сохранения и т.д.)
        if (continueButton == null)
        {
            Debug.LogError("[MENU] continueButton не назначен в Inspector!");
            return;
        }

        bool hasSave = SimpleSaveManager.Instance != null && SimpleSaveManager.Instance.HasSave();
        continueButton.interactable = hasSave;
    }

    // ЭТОТ МЕТОД БУДЕТ ВИДЕН В OnClick
    public void OnContinueClicked()
    {
        Debug.Log("[MENU] Нажата кнопка Continue");
        StartCoroutine(LoadGameSceneAndRestorePosition());
    }

    // ЭТОТ МЕТОД БУДЕТ ВИДЕН В OnClick
    public void OnNewGame()
    {
        Debug.Log("[MENU] Новая игра");

        // Удаляем старое сохранение
        if (SimpleSaveManager.Instance != null)
        {
            SimpleSaveManager.Instance.DeleteSave();
            Debug.Log("[MENU] Сохранение удалено для новой игры");
        }

        // Загружаем сцену игры
        SceneManager.LoadScene(gameSceneName);
    }


    // ЭТОТ МЕТОД БУДЕТ ВИДЕН В OnClick
    public void OnExit()
    {
        Debug.Log("[MENU] Выход из игры");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private IEnumerator LoadGameSceneAndRestorePosition()
    {
        Debug.Log("[MENU] Начинаем загрузку сцены: " + gameSceneName);

        if (loadingScreen != null)
            loadingScreen.SetActive(true);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(gameSceneName);
        asyncLoad.allowSceneActivation = true;

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        Debug.Log("[MENU] Сцена загружена, ждём инициализации...");

        yield return null;
        yield return null;

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            Debug.Log("[MENU] Игрок найден: " + player.name);

            PlayerPositionSaver saver = player.GetComponent<PlayerPositionSaver>();
            if (saver != null)
            {
                Debug.Log("[MENU] Найден PlayerPositionSaver, загружаем позицию...");
                saver.LoadPosition();
            }
            else
            {
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
}