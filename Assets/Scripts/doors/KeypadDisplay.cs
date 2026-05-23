using TMPro; // пространство имён TextMeshPro
using UnityEngine;

public class KeypadDisplay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text displayText; 
    [SerializeField] private KeypadController keypadController;
    [Header("Display Settings")]
    [SerializeField] private string placeholderText = "0000";

    void Start()
    {
        if (keypadController != null)
        {
            keypadController.OnCodeUpdated += UpdateDisplay;
        }
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
    private void UpdateDisplay(string currentCode)
    {
        if (displayText == null) return;      
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