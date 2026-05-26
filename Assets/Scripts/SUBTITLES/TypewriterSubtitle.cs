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

    [Tooltip("Перетащите GameObject с текстом (не сам текст!)")]
    public GameObject subtitleDisplayObject;

    // И добавьте свойство для удобного доступа:
    public TextMeshProUGUI subtitleDisplay
    {
        get
        {
            if (subtitleDisplayObject == null) return null;
            return subtitleDisplayObject.GetComponent<TextMeshProUGUI>();
        }
        set
        {
            // При присвоении сохраняем GameObject, а не компонент
            if (value == null)
                subtitleDisplayObject = null;
            else
                subtitleDisplayObject = value.gameObject;
        }
    }
    [Header("Настройки печати")]
    public float charsPerSecond = 20f;
    public bool clearOnFinish = true;
    public float clearDelay = 2f; // Задержка перед очисткой после конца печати

    private AudioSource audioSource;
    private bool hasPlayed = false;
    private Coroutine currentTypingCoroutine;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("❌ AudioSource не найден! [RequireComponent] должен был добавить его автоматически.");
        }
        audioSource.playOnAwake = false;
        audioSource.clip = voiceClip;
    }

    void Start()
    {
        // Авто-поиск только если не назначено вручную
        if (subtitleDisplay == null)
        {
            subtitleDisplay = FindFirstObjectByType<TextMeshProUGUI>();
            if (subtitleDisplay == null)
                Debug.LogWarning("⚠️ TextMeshProUGUI не найден автоматически. Назначьте его вручную в инспекторе!");
        }
    }

    public void Play()
    {
        if (hasPlayed) return;
        hasPlayed = true;

        // 🔥 ПРОВЕРКИ ПЕРЕД ИСПОЛЬЗОВАНИЕМ
        if (voiceClip == null)
        {
            Debug.LogError("❌ voiceClip не назначен!");
            return;
        }

        if (subtitleDisplay == null)
        {
            Debug.LogError("❌ subtitleDisplay = null! Назначьте TextMeshProUGUI в инспекторе или создайте Canvas с текстом.");
            return;
        }

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                Debug.LogError("❌ Критическая ошибка: нет AudioSource!");
                return;
            }
        }

        // 🎵 Запускаем аудио
        audioSource.clip = voiceClip;
        audioSource.Play();
        Debug.Log($"🔊 Играем: {voiceClip.name}");

        // 📝 Запускаем печать текста
        if (!string.IsNullOrEmpty(subtitleText))
        {
            Debug.Log($"📝 Печатаем: {subtitleText}");

            // Останавливаем предыдущую печать, если была
            if (currentTypingCoroutine != null)
                StopCoroutine(currentTypingCoroutine);

            currentTypingCoroutine = StartCoroutine(TypeText(subtitleText));
        }
        else
        {
            Debug.LogWarning("⚠️ subtitleText пустой — ничего не печатаем");
        }

        // 🧹 Планируем очистку
        if (clearOnFinish)
        {
            float totalDuration = voiceClip.length + clearDelay;
            StartCoroutine(ClearAfterDelay(totalDuration));
        }
    }

    private IEnumerator TypeText(string fullText)
    {
        if (subtitleDisplay == null) yield break;

        subtitleDisplay.text = "";
        subtitleDisplay.ForceMeshUpdate(); // 🔥 Важно для TMP в VR!

        float delay = 1f / Mathf.Max(charsPerSecond, 1f);

        // ✅ Правильный цикл: от 1 до длины текста включительно
        for (int i = 1; i <= fullText.Length; i++)
        {
            if (subtitleDisplay != null)
            {
                subtitleDisplay.text = fullText.Substring(0, i);
                subtitleDisplay.ForceMeshUpdate(); // 🔥 Обновляем меш каждый кадр
            }
            yield return new WaitForSeconds(delay);
        }

        Debug.Log("✅ Печать завершена");
    }

    private IEnumerator ClearAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (subtitleDisplay != null && clearOnFinish)
        {
            subtitleDisplay.text = "";
            subtitleDisplay.ForceMeshUpdate();
            Debug.Log("🧹 Текст очищен");
        }
    }

    // Остановка воспроизведения (полезно для отладки)
    public void StopPlayback()
    {
        if (audioSource != null && audioSource.isPlaying)
            audioSource.Stop();

        if (currentTypingCoroutine != null)
            StopCoroutine(currentTypingCoroutine);

        if (subtitleDisplay != null && clearOnFinish)
        {
            subtitleDisplay.text = "";
            subtitleDisplay.ForceMeshUpdate();
        }

        hasPlayed = false;
    }
}