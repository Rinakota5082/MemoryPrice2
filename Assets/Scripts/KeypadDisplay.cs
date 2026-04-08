using TMPro; // пространство имён TextMeshPro
using UnityEngine;

public class KeypadDisplay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text displayText; 
    [SerializeField] private KeypadController keypadController;
    [Header("Display Settings")]
    [SerializeField] private string placeholderText = "00000";    // Текст, когда код пустой

    void Start()
    {
        // Подписываемся на события панели
        if (keypadController != null)
        {
            keypadController.OnCodeUpdated += UpdateDisplay;
        }

        // Начальное состояние
        UpdateDisplay("");
    }

    void OnDestroy()
    {
        // Отписываемся при удалении
        if (keypadController != null)
        {
            keypadController.OnCodeUpdated -= UpdateDisplay;
        }
    }

    // Эта функция будет менять текст на панели
    private void UpdateDisplay(string currentCode)
    {
        if (displayText == null) return;

        // Если код пустой — показывает плейсхолдер
        if (string.IsNullOrEmpty(currentCode))
        {
            displayText.text = placeholderText;
            return;
        }
        else
        {
            displayText.text = currentCode;
        }
    }
}