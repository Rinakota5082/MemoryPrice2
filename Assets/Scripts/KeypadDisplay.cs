using TMPro; // пространство имён TextMeshPro
using UnityEngine;

public class KeypadDisplay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI displayText; // Перетащите сюда ваш TMP объект
    [SerializeField] private KeypadController keypadController; // Ссылка на ваш скрипт панели
    [SerializeField] private string placeholderText = "00000";

    void OnEnable()
    {
        // Подпишитесь на событие обновления кода (создайте его в KeypadController)
        if (keypadController != null)
            keypadController.OnCodeUpdated += UpdateDisplay;
    }

    void OnDisable()
    {
        if (keypadController != null)
            keypadController.OnCodeUpdated -= UpdateDisplay;
    }

    // Эта функция будет менять текст на панели
    private void UpdateDisplay(string currentCode)
    {
        if (displayText == null) return;

        // Если код пустой — показываем плейсхолдер
        if (string.IsNullOrEmpty(currentCode))
        {
            //displayText.text = placeholderText;
        }
        else
        {
            displayText.text = currentCode;
        }
    }
}