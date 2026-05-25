using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Обязательно для TextMeshPro

[RequireComponent(typeof(AudioSource))]
public class SubtitleManager : MonoBehaviour
{
    [Header("Компоненты")]
    [Tooltip("Ссылка на TextMeshProUGUI")]
    public TextMeshProUGUI subtitleText;

    [Header("Список субтитров")]
    public List<SubtitleLine> subtitles;

    [Header("Настройки")]
    public bool playOnStart = false;
    public bool clearOnFinish = true;

    [Tooltip("Задержка перед стартом проверки субтитров (помогает при рассинхроне)")]
    public float startDelay = 0.1f;

    private AudioSource audioSource;
    private bool isPlaying = false;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        // Проверка при старте: если ссылки нет — пишем жирную ошибку в консоль
        if (subtitleText == null)
            Debug.LogError("❌ SubtitleManager: Не назначен компонент Subtitle Text! Перетащите ваш TMP текст в инспекторе.");
        else
            subtitleText.text = "";
    }

    void Start()
    {
        if (playOnStart && audioSource.clip != null)
        {
            PlayWithSubtitles();
        }
    }

    public void PlayWithSubtitles()
    {
        if (audioSource.clip == null)
        {
            Debug.LogWarning("⚠️ Нет аудиоклипа в AudioSource!");
            return;
        }

        // Сбрасываем время аудио в 0, чтобы синхронизация была точной
        audioSource.time = 0f;
        audioSource.Play();

        isPlaying = true;

        // Запускаем корутину с небольшой задержкой, чтобы аудио успело инициализироваться
        StartCoroutine(UpdateSubtitles());
    }

    public void StopWithSubtitles()
    {
        if (audioSource.isPlaying) audioSource.Stop();
        if (clearOnFinish && subtitleText != null) subtitleText.text = "";
        isPlaying = false;
        StopAllCoroutines();
    }

    private IEnumerator UpdateSubtitles()
    {
        // Небольшая пауза в начале для стабильности
        yield return new WaitForSeconds(startDelay);

        int currentIndex = 0;

        // Продолжаем, пока аудио играет ИЛИ пока не показали все субтитры
        // (иногда аудио чуть короче последней субтитровой метки)
        while ((audioSource.isPlaying || currentIndex < subtitles.Count) && isPlaying)
        {
            float currentTime = audioSource.time;

            // Проверка: если время пришло и индекс в пределах списка
            if (currentIndex < subtitles.Count && currentTime >= subtitles[currentIndex].time)
            {
                string newText = subtitles[currentIndex].text;

                if (subtitleText != null)
                {
                    subtitleText.text = newText;

                    // 🔥 ВАЖНО ДЛЯ VR И TMP:
                    // Принудительно обновляем меш текста, чтобы он появился мгновенно
                    subtitleText.ForceMeshUpdate();

                    // Для отладки: раскомментируйте строку ниже, чтобы видеть в консоли, что текст ставится
                    // Debug.Log($"[Subtitles {currentTime:F2}s]: {newText}");
                }

                currentIndex++;
            }

            yield return null; // Ждём следующий кадр
        }

        // Финальная очистка
        if (clearOnFinish && subtitleText != null)
        {
            subtitleText.text = "";
            subtitleText.ForceMeshUpdate();
        }

        isPlaying = false;
    }
}