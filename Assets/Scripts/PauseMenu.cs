using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private string menuSceneName = "MainMenuScene";

    [Header("Behavior")]
    [Tooltip("Для VR обычно лучше false, чтобы локомоция не ломалась.")]
    [SerializeField] private bool freezeTimeOnPause = false;

    private bool isPaused;
    private GameControls inputActions;

    private void Awake()
    {
        inputActions = new GameControls();
        inputActions.UI.Pause.performed += OnPausePerformed;
    }

    private void OnEnable()
    {
        inputActions?.Enable();
    }

    private void OnDisable()
    {
        if (isPaused)
            Resume();

        inputActions?.Disable();
    }


    private void OnDestroy()
    {
        if (inputActions != null)
            inputActions.UI.Pause.performed -= OnPausePerformed;
    }

    private void Start()
    {
        isPaused = false;

        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);

        Time.timeScale = 1f;
    }

    private void OnPausePerformed(UnityEngine.InputSystem.InputAction.CallbackContext _)
    {
        TogglePause();
    }

    public void TogglePause()
    {
        if (isPaused) Resume();
        else Pause();
    }

    public void Pause()
    {
        isPaused = true;

        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(true);

        if (freezeTimeOnPause)
            Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    //public void Resume()
    //{
    //    isPaused = false;

    //    if (pauseMenuPanel != null)
    //        pauseMenuPanel.SetActive(false);

    //    Time.timeScale = 1f;

    //    Cursor.lockState = CursorLockMode.Locked;
    //    Cursor.visible = false;
    //}


    public void Resume()
    {
        isPaused = false;

        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;  // Для VR может быть None
        Cursor.visible = false;
    }



    //public void SaveAndQuitToMenu()
    //{
    //    // 🔥 НАХОДИМ ИГРОКА И СОХРАНЯЕМ ЕГО ПОЗИЦИЮ
    //    GameObject player = GameObject.FindGameObjectWithTag("Player");

    //    if (player != null)
    //    {
    //        // Пытаемся найти компонент сохранения
    //        PlayerPositionSaver saver = player.GetComponent<PlayerPositionSaver>();
    //        if (saver != null)
    //        {
    //            saver.SavePosition();
    //            Debug.Log($"[Pause] Игра сохранена через PlayerPositionSaver!");
    //        }
    //        else
    //        {
    //            // Если нет компонента, сохраняем напрямую через SimpleSaveManager
    //            if (SimpleSaveManager.Instance != null)
    //            {
    //                SimpleSaveManager.Instance.SaveGame(player.transform);
    //                Debug.Log($"[Pause] Игра сохранена напрямую! Позиция: {player.transform.position}");
    //            }
    //            else
    //            {
    //                Debug.LogWarning("[Pause] SimpleSaveManager.Instance = null! Не удалось сохранить.");
    //            }
    //        }
    //    }
    //    else
    //    {
    //        Debug.LogWarning("[Pause] Игрок с тегом 'Player' не найден! Проверь тег у XR Origin.");
    //    }

    //    // Возвращаем время и грузим меню
    //    Time.timeScale = 1f;
    //    SceneManager.LoadScene(menuSceneName);
    //}
    public void SaveAndQuitToMenu()
    {
        // 🔥 НАХОДИМ ИГРОКА И СОХРАНЯЕМ ЕГО ПОЗИЦИЮ
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            PlayerPositionSaver saver = player.GetComponent<PlayerPositionSaver>();
            if (saver != null)
            {
                saver.SavePosition();
                Debug.Log($"[Pause] Игра сохранена через PlayerPositionSaver!");
            }
            else
            {
                if (SimpleSaveManager.Instance != null)
                {
                    SimpleSaveManager.Instance.SaveGame(player.transform);
                    Debug.Log($"[Pause] Игра сохранена напрямую!");
                }
            }
        }

        // ✅ ВАЖНО: СБРАСЫВАЕМ ВСЕ НАСТРОЙКИ ПЕРЕД ЗАГРУЗКОЙ МЕНЮ
        Time.timeScale = 1f;           // Возвращаем нормальное время
        Cursor.lockState = CursorLockMode.None;  // Разблокируем курсор
        Cursor.visible = true;         // Делаем курсор видимым

        // Закрываем панель паузы, если она ещё открыта
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);

        isPaused = false;

        // Загружаем главное меню
        SceneManager.LoadScene(menuSceneName);
    }
}