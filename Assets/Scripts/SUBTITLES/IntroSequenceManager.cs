using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro; // 🔥 Добавили для работы с TextMeshPro

public class IntroSequenceManager : MonoBehaviour
{
    [Header("Префаб с TypewriterSubtitle")]
    public GameObject voicePrefab;

    [Header("Список реплик")]
    public List<IntroReplica> replicas;

    [Header("Настройки")]
    public float delayBetweenReplicas = 0.5f;
    public bool playOnStart = true;

    // 🔥 НОВОЕ: Задержка перед стартом первой реплики (чтобы игрок "вошёл" в сцену)
    [Tooltip("Задержка перед началом диалога (сек)")]
    public float startDelay = 1f;

    // 🔥 НОВОЕ: Прямая ссылка на нужный TextMeshProUGUI (чтобы не находил неправильный)
    [Header("Субтитры")]
    [Tooltip("Перетащите сюда ваш TextMeshProUGUI (обязательно!)")]
    public TextMeshProUGUI subtitleDisplay;

    private bool isPlaying = false;

    [System.Serializable]
    public class IntroReplica
    {
        [Tooltip("Аудиофайл с озвучкой")]
        public AudioClip voiceClip;

        [Tooltip("Текст субтитра")]
        [TextArea(2, 5)]
        public string subtitleText;
    }

    void Start()
    {
        if (playOnStart && replicas != null && replicas.Count > 0)
        {
            StartCoroutine(PlayAllReplicas());
        }
    }

    public IEnumerator PlayAllReplicas()
    {
        if (isPlaying) yield break;
        isPlaying = true;

        Debug.Log("🎬 Начинаем воспроизведение вступительной последовательности...");

        // 🔥 Ждём начальную задержку перед стартом
        yield return new WaitForSeconds(startDelay);

        // 🔥 Если текст не назначен вручную — пробуем найти (но с предупреждением)
        if (subtitleDisplay == null)
        {
            subtitleDisplay = FindFirstObjectByType<TextMeshProUGUI>();
            if (subtitleDisplay != null)
                Debug.LogWarning($"⚠️ TextMeshProUGUI найден автоматически: {subtitleDisplay.name}. Лучше назначить вручную!");
            else
                Debug.LogError("❌ TextMeshProUGUI не найден! Назначьте его в поле subtitleDisplay.");
        }

        for (int i = 0; i < replicas.Count; i++)
        {
            IntroReplica replica = replicas[i];
            Debug.Log($"🔊 Воспроизведение реплики {i + 1}/{replicas.Count}: {replica.subtitleText}");

            // Проигрываем текущую реплику
            yield return StartCoroutine(PlaySingleReplica(replica));

            // Ждём небольшую паузу между репликами (если не последняя)
            if (i < replicas.Count - 1)
            {
                yield return new WaitForSeconds(delayBetweenReplicas);
            }
        }

        Debug.Log("✅ Все вступительные реплики проиграны!");
        isPlaying = false;

        OnSequenceComplete();
    }

    private IEnumerator PlaySingleReplica(IntroReplica replica)
    {
        if (voicePrefab == null)
        {
            Debug.LogError("❌ voicePrefab не назначен!");
            yield break;
        }

        if (replica.voiceClip == null)
        {
            Debug.LogError("❌ voiceClip в реплике отсутствует!");
            yield break;
        }

        // 🔥 ПРОВЕРКА: есть ли ссылка на текст
        if (subtitleDisplay == null)
        {
            Debug.LogError("❌ subtitleDisplay не назначен! Реплика не будет показывать текст.");
            // Не прерываем — хотя бы звук проиграется
        }

        // Создаём временный объект для этой реплики
        GameObject go = Instantiate(voicePrefab, transform.position, Quaternion.identity);
        TypewriterSubtitle ts = go.GetComponent<TypewriterSubtitle>();

        if (ts == null)
        {
            Debug.LogError("❌ На префабе нет компонента TypewriterSubtitle!");
            Destroy(go);
            yield break;
        }

        // Настраиваем реплику
        ts.voiceClip = replica.voiceClip;
        ts.subtitleText = replica.subtitleText;

        // 🔥 ГЛАВНОЕ ИСПРАВЛЕНИЕ: передаём правильный TextMeshProUGUI
        ts.subtitleDisplay = subtitleDisplay;

        // Запускаем воспроизведение
        ts.Play();

        // Ждём, пока аудио не закончится + небольшая буферная задержка
        float waitTime = replica.voiceClip.length + ts.clearDelay + 0.2f;
        yield return new WaitForSeconds(waitTime);

        // Уничтожаем временный объект
        Destroy(go);
    }

    private void OnSequenceComplete()
    {
        Debug.Log("🎉 Вступление закончено! Можно разблокировать управление игроком.");
        // Здесь можно отправить событие, разблокировать движение персонажа и т.д.
        // Например:
        // PlayerController.Instance.UnlockControls();
    }

    // 🔧 Для удобного тестирования: публичный метод запуска
    public void StartSequence()
    {
        if (!isPlaying)
            StartCoroutine(PlayAllReplicas());
    }

    // 🔧 Для отладки: остановка последовательности
    public void StopSequence()
    {
        isPlaying = false;
        StopAllCoroutines();
    }
}