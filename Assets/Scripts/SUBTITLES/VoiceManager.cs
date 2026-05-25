using UnityEngine;
using TMPro;
using System.Collections;

public class VoiceManager : MonoBehaviour
{
    public static VoiceManager Instance;

    [Header("Субтитры")]
    public TextMeshProUGUI subtitleDisplay;

    [Header("Настройки печати")]
    public float charsPerSecond = 20f;

    private AudioSource audioSource;
    private Coroutine currentSubtitleRoutine;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    /// <summary>
    /// Проигрывает реплику и печатает субтитры
    /// </summary>
    /// <param name="clip">Аудиофайл с репликой</param>
    /// <param name="subtitleText">Текст субтитра</param>
    public void PlayReplica(AudioClip clip, string subtitleText)
    {
        if (clip == null)
        {
            Debug.LogWarning("Нет аудиоклипа!");
            return;
        }

        // Останавливаем текущую реплику, если она играет
        if (audioSource.isPlaying)
            audioSource.Stop();

        // Останавливаем текущую печать субтитров
        if (currentSubtitleRoutine != null)
            StopCoroutine(currentSubtitleRoutine);

        // Меняем клип и запускаем
        audioSource.clip = clip;
        audioSource.Play();

        // Запускаем печать субтитров
        if (subtitleDisplay != null && !string.IsNullOrEmpty(subtitleText))
        {
            currentSubtitleRoutine = StartCoroutine(TypeText(subtitleText));
        }

        // Очищаем субтитры после окончания аудио
        StartCoroutine(ClearAfterDelay(clip.length));
    }

    private IEnumerator TypeText(string fullText)
    {
        subtitleDisplay.text = "";
        float delay = 1f / charsPerSecond;

        for (int i = 0; i <= fullText.Length; i++)
        {
            subtitleDisplay.text = fullText.Substring(0, i);
            yield return new WaitForSeconds(delay);
        }
    }

    private IEnumerator ClearAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (subtitleDisplay != null && subtitleDisplay.text != "")
        {
            subtitleDisplay.text = "";
        }
        currentSubtitleRoutine = null;
    }
}