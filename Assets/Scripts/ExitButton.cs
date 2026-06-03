using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitButton : MonoBehaviour
{
    [Header("Настройки выхода")]
    [SerializeField] private string menuSceneName = "MainMenuScene";

    public void ExitToMenu()
    {
        // Сохраняем позицию игрока (если нужен PlayerPositionSaver)
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerPositionSaver saver = player.GetComponent<PlayerPositionSaver>();
            if (saver != null)
                saver.SavePosition();
            else if (SimpleSaveManager.Instance != null)
                SimpleSaveManager.Instance.SaveGame(player.transform);
        }

        // Сбрасываем настройки
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Загружаем меню
        SceneManager.LoadScene(menuSceneName);
    }
}