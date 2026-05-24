using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(AudioSource))]
public class SubtitleManager : MonoBehaviour
{
    [Header("Компоненты")]
    [Tooltip("TextMeshProUGUI, в который будут выводиться субтитры")]
    public TextMeshProUGUI subtitleText;

    [Header("Список субтитров")]
    public List<SubtitleLine> subtitles;

    [Header("Настройки")]
    [Tooltip("Автоматически запустить воспроизведение при старте сцены")]
    public bool playOnStart = false;

    [Tooltip("Очищать текст субтитров по окончании")]
    public bool clearOnFinish = true;

    private AudioSource audioSource;
    private bool isPlaying = false;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (subtitleText != null)
            subtitleText.text = "";
    }

    void Start()
    {
        if (playOnStart && audioSource.clip != null)
        {
            PlayWithSubtitles();
        }
    }

    /// <summary>
    /// Запускает воспроизведение аудиоклипа и синхронизирует субтитры.
    /// </summary>
    public void PlayWithSubtitles()
    {
        if (audioSource.clip == null)
        {
            Debug.LogWarning("Нет аудиоклипа в AudioSource!");
            return;
        }

        if (subtitles == null || subtitles.Count == 0)
        {
            // Нет субтитров — просто проигрываем звук
            audioSource.Play();
            return;
        }

        audioSource.Play();
        isPlaying = true;
        StartCoroutine(UpdateSubtitles());
    }

    /// <summary>
    /// Останавливает воспроизведение и очищает субтитры.
    /// </summary>
    public void StopWithSubtitles()
    {
        if (audioSource.isPlaying)
            audioSource.Stop();

        if (clearOnFinish && subtitleText != null)
            subtitleText.text = "";

        isPlaying = false;
        StopAllCoroutines();
    }

    private IEnumerator UpdateSubtitles()
    {
        int currentIndex = 0;

        while (audioSource.isPlaying && currentIndex < subtitles.Count)
        {
            float currentTime = audioSource.time;

            // Если текущее время достигло времени следующего субтитра
            if (currentTime >= subtitles[currentIndex].time)
            {
                if (subtitleText != null)
                    subtitleText.text = subtitles[currentIndex].text;

                currentIndex++;
            }

            yield return null; // ждём один кадр
        }

        // Воспроизведение закончилось
        if (clearOnFinish && subtitleText != null)
            subtitleText.text = "";

        isPlaying = false;
    }
}