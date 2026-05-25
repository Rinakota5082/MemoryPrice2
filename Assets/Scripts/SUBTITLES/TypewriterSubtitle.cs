using UnityEngine;
using TMPro;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class TypewriterSubtitle : MonoBehaviour
{
    [Header("Аудио")]
    public AudioClip voiceClip;

    [Header("Субтитры")]
    [TextArea(2, 5)]
    public string subtitleText;

    [Header("Настройки печати")]
    public float charsPerSecond = 20f;
    public bool clearOnFinish = true;

    private AudioSource audioSource;
    public TextMeshProUGUI subtitleDisplay;
    private bool hasPlayed = false;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = voiceClip;
        audioSource.playOnAwake = false;
    }

    void Start()
    {
        // Автоматически находим субтитры на сцене
        if (subtitleDisplay == null)
        {
            subtitleDisplay = FindFirstObjectByType<TextMeshProUGUI>();
            if (subtitleDisplay == null)
                Debug.LogError("TextMeshProUGUI не найден на сцене! Добавьте TextMeshPro на Canvas.");
        }
    }

    public void Play()
    {
        if (hasPlayed) return;
        if (voiceClip == null)
        {
            Debug.LogWarning("Нет аудиоклипа!");
            return;
        }

        audioSource.Play();
        hasPlayed = true;

        if (subtitleDisplay != null && !string.IsNullOrEmpty(subtitleText))
        {
            StartCoroutine(TypeText(subtitleText));
        }

        if (clearOnFinish)
            StartCoroutine(ClearAfterDelay(voiceClip.length));
    }

    public IEnumerator TypeText(string fullText)
    {
        subtitleDisplay.text = "";
        float delay = 1f / charsPerSecond;

        for (int i = 0; i <= fullText.Length; i++)
        {
            subtitleDisplay.text = fullText.Substring(0, i);
            yield return new WaitForSeconds(delay);
        }
    }

    public IEnumerator ClearAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (subtitleDisplay != null && subtitleDisplay.text == subtitleText)
            subtitleDisplay.text = "";
    }
}