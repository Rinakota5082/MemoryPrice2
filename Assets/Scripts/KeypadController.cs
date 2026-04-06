using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System; // Добавляем для работы с событием

public class KeypadController : MonoBehaviour
{
    [Header("Code Settings")]
    [SerializeField] private string correctCode = "12345"; // Код, который нужно угадать
    [SerializeField] private int maxDigits = 5; // Максимальная длина кода

    [Header("Feedback")]
    [SerializeField] private MeshRenderer displayRenderer; // Ссылка на дисплей (для подсветки)
    [SerializeField] private Material correctCodeMaterial; // Материал при правильном коде
    [SerializeField] private Material wrongCodeMaterial;   // Материал при ошибке (опционально)

    [Header("Actions")]
    [SerializeField] private GameObject objectToActivate; // Что произойдет при успехе (дверь, свет и т.д.)

    [Header("Audio (Optional)")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip correctSound;
    [SerializeField] private AudioClip wrongSound;

    // 👇 ЭТО СОБЫТИЕ ДЛЯ ОБНОВЛЕНИЯ ДИСПЛЕЯ
    public event Action<string> OnCodeUpdated; // Будет вызываться при каждом нажатии

    private string currentInput = "";
    private Material defaultMaterial;
    private bool isCodeCorrect = false;

    void Start()
    {
        // Сохраняем стандартный материал дисплея, чтобы вернуть его при сбросе
        if (displayRenderer != null)
            defaultMaterial = displayRenderer.material;
    }

    // 👇 Эту функцию вызываем для каждой цифры из инспектора
    public void AddDigit(string digit)
    {
        if (isCodeCorrect) return; // Если код уже введен, игнорируем нажатия

        if (currentInput.Length < maxDigits)
        {
            currentInput += digit;
            Debug.Log($"Current code: {currentInput}");

            // 👇 Оповещаем всех подписчиков (например, дисплей), что код обновился
            OnCodeUpdated?.Invoke(currentInput);
        }

        // Автоматическая проверка, если набрана нужная длина (опционально)
        if (currentInput.Length == maxDigits && !isCodeCorrect)
        {
            CheckCode();
        }
    }

    // 👇 Функция для кнопки "Стереть последнюю цифру"
    public void Backspace()
    {
        if (isCodeCorrect) return;

        if (currentInput.Length > 0)
        {
            currentInput = currentInput.Substring(0, currentInput.Length - 1);
            OnCodeUpdated?.Invoke(currentInput);
        }
    }

    // 👇 Функция для кнопки "Сброс" (очистить все)
    public void ResetCode()
    {
        if (isCodeCorrect) return;

        currentInput = "";
        OnCodeUpdated?.Invoke(currentInput);
        Debug.Log("Code reset");

        // Сброс цвета дисплея
        if (displayRenderer != null)
            displayRenderer.material = defaultMaterial;
    }

    // 👇 Функция для кнопки "Enter" (проверка кода)
    public void CheckCode()
    {
        if (isCodeCorrect) return;

        if (currentInput == correctCode)
        {
            isCodeCorrect = true;
            Debug.Log("CODE CORRECT!");

            // Визуальный фидбек
            if (displayRenderer != null)
                displayRenderer.material = correctCodeMaterial;

            // Звуковой фидбек
            if (audioSource != null && correctSound != null)
                audioSource.PlayOneShot(correctSound);

            // Активируем целевой объект
            if (objectToActivate != null)
            {
                objectToActivate.SetActive(true);//скорее всего не будем использовать


                /*// Если это дверь, можно вызвать метод Unlock
                var door = objectToActivate.GetComponent<Door>();
                if (door != null) door.Unlock();*/
            }
        }
        else
        {
            Debug.Log($"WRONG CODE! '{currentInput}' is not correct.");

            // Визуальный фидбек ошибки
            if (displayRenderer != null && wrongCodeMaterial != null)
            {
                displayRenderer.material = wrongCodeMaterial;
                // Возвращаем обычный цвет через секунду
                Invoke(nameof(ResetDisplayColor), 0.5f);
            }

            // Звуковой фидбек ошибки
            if (audioSource != null && wrongSound != null)
                audioSource.PlayOneShot(wrongSound);

            // Очищаем ввод при ошибке
            currentInput = "";
            OnCodeUpdated?.Invoke(currentInput);
        }
    }

    private void ResetDisplayColor()
    {
        if (displayRenderer != null && !isCodeCorrect)
            displayRenderer.material = defaultMaterial;
    }

    // Дополнительно: метод для получения текущего состояния (можно использовать для сохранения)
    public bool IsCodeCorrect() => isCodeCorrect;
}