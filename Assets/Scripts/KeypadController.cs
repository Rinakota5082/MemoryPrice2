using System; // Добавляем для работы с событием
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class KeypadController : MonoBehaviour
{
    [Header("Code Settings")]
    [SerializeField] private string correctCode = "12345"; // Код, который нужно угадать
    [SerializeField] private int maxDigits = 5; // Максимальная длина кода

    [System.Serializable]
    public class KeypadButton
    {
        public XRSimpleInteractable button; 
        public string digit;                
    }
    [Header("Button References")]
    [SerializeField] private KeypadButton[] buttons; // Список всех кнопок панели

    [Header("Door to Unlock")]
    [SerializeField] private DoorUnlock doorToUnlock;  // Ссылка на объект Door

    [Header("Action Buttons")]
    [SerializeField] private XRSimpleInteractable resetButton;   // Кнопка сброса
    [SerializeField] private XRSimpleInteractable backspaceButton; // Кнопка удаления

    [Header("Feedback")]
    [SerializeField] private MeshRenderer displayRenderer; // Ссылка на дисплей (для подсветки)
    [SerializeField] private Material correctCodeMaterial; // Материал при правильном коде
    [SerializeField] private Material wrongCodeMaterial;   // Материал при ошибке 

    [Header("Actions")]
    [SerializeField] private GameObject objectToActivate; // Что произойдет при успехе (дверь)

    [Header("Audio (Optional)")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip correctSound;
    [SerializeField] private AudioClip wrongSound;

    // СОБЫТИЕ ДЛЯ ОБНОВЛЕНИЯ ДИСПЛЕЯ
    public event Action<string> OnCodeUpdated; // Будет вызываться при каждом нажатии

    private string currentInput = "";
    private Material defaultMaterial;
    private bool isCodeCorrect = false;

    void Start()
    {
        // Сохраняем стандартный материал дисплея, чтобы вернуть его при сбросе
        if (displayRenderer != null)
            defaultMaterial = displayRenderer.material;
        SetupButtons();
    }

    private void SetupButtons()
    {
        // Подписываем цифровые кнопки
        foreach (KeypadButton btn in buttons)
        {
            if (btn.button == null) continue;

            // Захватываем текущую цифру в локальную переменную, чтобы не было бага с замыканием
            string digitToAdd = btn.digit;
            btn.button.selectEntered.AddListener((args) => AddDigit(digitToAdd));
        }

        // Подписываем кнопку Reset
        if (resetButton != null)
            resetButton.selectEntered.AddListener((args) => ResetCode());

        // Подписываем кнопку Backspace
        if (backspaceButton != null)
            backspaceButton.selectEntered.AddListener((args) => Backspace());
    }

    // функцию вызываем для каждой цифры из инспектора
    public void AddDigit(string digit)
    {
        if (isCodeCorrect) return; // Если код уже введен, игнорируем нажатия

        if (currentInput.Length < maxDigits)
        {
            currentInput += digit;
            Debug.Log($"Current code: {currentInput}");            
            OnCodeUpdated?.Invoke(currentInput);
        }

        // если набрана нужная длина 
        if (currentInput.Length == maxDigits && !isCodeCorrect)
        {
            CheckCode();
        }
    }

    //  Функция для кнопки "Стереть последнюю цифру"
    public void Backspace()
    {
        if (isCodeCorrect) return;

        if (currentInput.Length > 0)
        {
            currentInput = currentInput.Substring(0, currentInput.Length - 1);
            OnCodeUpdated?.Invoke(currentInput);
        }
    }

    // Функция для кнопки "Сброс" (очистить все)
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

    // проверка кода
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

            if (doorToUnlock != null)
                doorToUnlock.Unlock();  // Вызываем разблокировку РУЧКИ
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