using System; // Добавляем для работы с событием
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class KeypadController : MonoBehaviour
{   
    [System.Serializable]
    public class Codes
    {
        public string digit;
    }
    [Header("Code Settings")]
    
    [SerializeField] private Codes[] correctCode ; // Код, который нужно угадать
    [SerializeField] private int maxDigits = 4; 

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
    [SerializeField] private XRSimpleInteractable resetButton;   
    [SerializeField] private XRSimpleInteractable backspaceButton; 

    [Header("Feedback")]
    [SerializeField] private MeshRenderer displayRenderer; 
    [SerializeField] private Material correctCodeMaterial; 
    [SerializeField] private Material wrongCodeMaterial;   

    [Header("Actions")]
    [SerializeField] private GameObject objectToActivate;

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
        if (displayRenderer != null)
            defaultMaterial = displayRenderer.material;
        SetupButtons();
    }

    private void SetupButtons()
    {
        
        foreach (KeypadButton btn in buttons)
        {
            if (btn.button == null) continue;
            
            string digitToAdd = btn.digit;
            btn.button.selectEntered.AddListener((args) => AddDigit(digitToAdd));
        }
        resetButton.selectEntered.AddListener((args) => ResetCode());
        backspaceButton.selectEntered.AddListener((args) => Backspace());
    }
    public void AddDigit(string digit)
    {
        if (isCodeCorrect) return;

        if (currentInput.Length < maxDigits)
        {
            currentInput += digit;          
            OnCodeUpdated?.Invoke(currentInput);
        }
        if (currentInput.Length == maxDigits && !isCodeCorrect)
        {
            Debug.Log(currentInput);
            CheckCode();
        }
    }
    public void Backspace()
    {
        if (isCodeCorrect) return;

        if (currentInput.Length > 0)
        {
            currentInput = currentInput.Substring(0, currentInput.Length - 1);
            OnCodeUpdated?.Invoke(currentInput);
        }
    }
    public void ResetCode()
    {
        if (isCodeCorrect) return;

        currentInput = "";
        OnCodeUpdated?.Invoke(currentInput);

        // Сброс цвета дисплея
        if (displayRenderer != null)
            displayRenderer.material = defaultMaterial;
    }
    public void CheckCode()
    {
        if (isCodeCorrect) return;
        int n = 0;
        foreach (Codes correctcode in correctCode)
        {
            Debug.Log(correctcode);
            if (currentInput == correctcode.digit)
            {
                n += 1;
                isCodeCorrect = true;
                Debug.Log(currentInput);
                doorToUnlock.Unlock();
                if (displayRenderer != null)
                    displayRenderer.material = correctCodeMaterial;
                if (audioSource != null && correctSound != null)
                    audioSource.PlayOneShot(correctSound);
                
            } }
            if(n==0)
            {
                if (displayRenderer != null && wrongCodeMaterial != null)
                {
                    displayRenderer.material = wrongCodeMaterial;
                    // Возвращаем обычный цвет через секунду
                    Invoke(nameof(ResetDisplayColor), 0.5f);
                }
                if (audioSource != null && wrongSound != null)
                    audioSource.PlayOneShot(wrongSound);
                currentInput = "";
                OnCodeUpdated?.Invoke(currentInput);
            
            }
    }
    private void ResetDisplayColor()
    {
        if (displayRenderer != null && !isCodeCorrect)
            displayRenderer.material = defaultMaterial;
    }
    public bool IsCodeCorrect() => isCodeCorrect;
}