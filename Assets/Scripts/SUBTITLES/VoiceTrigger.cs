using UnityEngine;
using TMPro;

public class VoiceTrigger : MonoBehaviour
{
    [Header("Настройки реплики")]
    [Tooltip("Префаб с компонентом TypewriterSubtitle")]
    public GameObject voicePrefab;

    [Tooltip("Аудиофайл с озвучкой")]
    public AudioClip voiceClip;

    [Tooltip("Текст субтитра")]
    [TextArea(2, 5)]
    public string subtitleText;

    [Tooltip("Воспроизвести только один раз")]
    public bool playOnce = true;

    [Header("Поиск текста")]
    [Tooltip("Перетащите вручную TextMeshProUGUI (приоритет над тегом)")]
    public TextMeshProUGUI manualSubtitleDisplay;

    [Tooltip("Тег для поиска текста (по умолчанию: Subtitle)")]
    public string subtitleTag = "Subtitle";

    private bool hasPlayed = false;

    private void OnTriggerEnter(Collider other)
    {
        // Если уже проигрывали и стоит флаг playOnce — выходим
        if (playOnce && hasPlayed) return;

        // Проверяем, что вошёл именно игрок
        if (!other.CompareTag("Player")) return;

        // Проверяем обязательные поля
        if (voicePrefab == null)
        {
            Debug.LogError($"❌ [{name}] voicePrefab не назначен!");
            return;
        }

        if (voiceClip == null)
        {
            Debug.LogError($"❌ [{name}] voiceClip не назначен!");
            return;
        }

        // Создаём объект с голосом
        GameObject go = Instantiate(voicePrefab, transform.position, Quaternion.identity);
        TypewriterSubtitle ts = go.GetComponent<TypewriterSubtitle>();

        if (ts != null)
        {
            // Назначаем данные реплики
            ts.voiceClip = voiceClip;
            ts.subtitleText = subtitleText;

            // 🔥 НАХОДИМ И НАЗНАЧАЕМ TEXTMESHPOUGUI
            TextMeshProUGUI textComponent = null;

            // 🔹 ПРИОРИТЕТ 1: Если назначили вручную в инспекторе
            if (manualSubtitleDisplay != null)
            {
                textComponent = manualSubtitleDisplay;
                Debug.Log($"✅ [{name}] Используется ручной текст: {manualSubtitleDisplay.name}");
            }
            // 🔹 ПРИОРИТЕТ 2: Ищем по тегу
            else if (!string.IsNullOrEmpty(subtitleTag))
            {
                GameObject textObj = GameObject.FindGameObjectWithTag(subtitleTag);
                if (textObj != null)
                {
                    textComponent = textObj.GetComponent<TextMeshProUGUI>();
                    if (textComponent != null)
                    {
                        Debug.Log($"✅ [{name}] Найдено по тегу '{subtitleTag}': {textObj.name}");
                    }
                    else
                    {
                        Debug.LogError($"❌ [{name}] На объекте '{textObj.name}' нет компонента TextMeshProUGUI!");
                    }
                }
                else
                {
                    Debug.LogWarning($"⚠️ [{name}] Объект с тегом '{subtitleTag}' не найден! Пробуем авто-поиск...");
                }
            }

            // 🔹 ПРИОРИТЕТ 3: Авто-поиск любого TextMeshProUGUI на сцене
            if (textComponent == null)
            {
                textComponent = FindFirstObjectByType<TextMeshProUGUI>();
                if (textComponent != null)
                {
                    Debug.LogWarning($"⚠️ [{name}] Используется первый найденный текст: {textComponent.name}");
                    Debug.LogWarning($"💡 Совет: Назначьте тег '{subtitleTag}' нужному объекту для точного поиска!");
                }
                else
                {
                    Debug.LogError($"❌ [{name}] TextMeshProUGUI не найден на сцене! Создайте Canvas с текстом.");
                }
            }

            // Назначаем найденный текст в скрипт субтитров
            if (textComponent != null)
            {
                ts.subtitleDisplay = textComponent;
            }

            // Запускаем воспроизведение
            ts.Play();
        }
        else
        {
            Debug.LogError($"❌ [{name}] На префабе не найден компонент TypewriterSubtitle!");
            Destroy(go);
            return;
        }

        // Уничтожаем объект после окончания аудио + небольшой запас
        float destroyDelay = voiceClip.length + 2f;
        Destroy(go, destroyDelay);

        // Помечаем, что реплика проиграна
        hasPlayed = true;

        Debug.Log($"🎬 [{name}] Реплика запущена: '{subtitleText}'");
    }

    // 🔧 Сброс триггера для повторного тестирования в редакторе
#if UNITY_EDITOR
    private void OnValidate()
    {
        // Сбрасываем hasPlayed при изменении настроек в инспекторе
        if (!Application.isPlaying)
            hasPlayed = false;
    }
#endif

    // 🔧 Публичный метод для сброса (если нужно проиграть повторно в игре)
    public void ResetTrigger()
    {
        hasPlayed = false;
    }
}