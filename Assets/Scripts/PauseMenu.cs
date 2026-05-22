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
        // Если объект выключили во время паузы, гарантированно возвращаем игру.
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


        // На всякий случай, если сцена ранее осталась в паузе.
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

    public void Resume()
    {
        isPaused = false;

        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);

        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void SaveAndQuitToMenu()
    {
        //Ищем игрока по тегу "Player" (должен висеть на XR Origin!)
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            // Сохраняем через наш SaveSystem (сцена + позиция + поворот)

            Debug.Log($"[Pause] Игра сохранена: {player.transform.position}");
        }
        else
        {
            Debug.LogWarning("[Pause] Игрок с тегом 'Player' не найден! Проверь тег у XR Origin.");
        }

        // Возвращаем время и грузим меню
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuSceneName);
    }
}