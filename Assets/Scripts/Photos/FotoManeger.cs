//using UnityEngine;

//public class FotoManeger : MonoBehaviour
//{
//    [SerializeField] private Door1 doorToUnlock;
//    public bool foto1 = false;
//    public bool foto2 = false;
//    public bool foto3 = false;
//    public bool foto4 = false;
//    public bool foto5 = false;
//    public bool foto6 = false;
//    void Start()
//    {

//    }
//    void Update()
//    {
//        if (foto1 && foto2 && foto3 && foto4 && foto5) { doorToUnlock.Point6 = true; }
//    }

//}


using UnityEngine;

public class FotoManeger : MonoBehaviour
{
    [SerializeField] private Door1 doorToUnlock;
    public bool foto1 = false;
    public bool foto2 = false;
    public bool foto3 = false;
    public bool foto4 = false;
    public bool foto5 = false;
    public bool foto6 = false;

    // 🔥 НОВЫЕ ПОЛЯ (добавить)
    [Header("Обратная связь")]
    public TypewriterSubtitle subtitleSpeaker;  // Ссылка на объект с субтитрами
    public AudioClip successClip;               // Аудио для правильного решения
    public AudioClip failClip;                  // Аудио для неправильного решения
    public string successText = "Она росла. А ты… ты был рядом? Или уже тогда думал только о себе?";
    public string failText = "Этого не было. Этого не могло быть. Твоя память защищается, Марк. Она подменяет больное — ложным.";

    private bool feedbackGiven = false;         // Чтобы не повторять реплику много раз
    private bool puzzleCompleted = false;       // Чтобы не проверять после завершения

    void Start()
    {

    }

    void Update()
    {
        // Проверяем, все ли правильные фотографии на столе (foto5 и foto6)
        bool allTruePhotosPlaced = foto5 && foto6;

        // Проверяем, все ли неправильные фотографии в мусорке (foto1, foto2, foto3, foto4)
        bool allFalsePhotosTrashed = foto1 && foto2 && foto3 && foto4;

        // Если всё собрано правильно И ещё не давали обратную связь
        if (allTruePhotosPlaced && allFalsePhotosTrashed && !puzzleCompleted)
        {
            puzzleCompleted = true;
            ShowSuccessFeedback();

            // Открываем дверь (как и было)
            if (doorToUnlock != null)
                doorToUnlock.Point6 = true;
        }
        // Если всё собрано НЕправильно (например, неправильные фото на столе или правильные в мусорке)
        // Проверяем, что хотя бы одна правильная фотография НЕ на столе ИЛИ хотя бы одна неправильная НЕ в мусорке
        // И при этом хоть что-то уже собрано (чтобы не говорить "неправильно" в пустой комнате)
        else if (!puzzleCompleted && !feedbackGiven && (AnyPhotoPlaced()))
        {
            // Проверяем, есть ли ошибка: правильная фотография не на столе ИЛИ неправильная не в мусорке
            bool hasTruePhotoNotOnDesk = (!foto5 || !foto6);
            bool hasFalsePhotoNotInTrash = (!foto1 || !foto2 || !foto3 || !foto4);

            // Если есть хоть одна ошибка
            if (hasTruePhotoNotOnDesk || hasFalsePhotoNotInTrash)
            {
                // Дополнительная проверка: если всё уже правильно, не показываем ошибку
                if (!(allTruePhotosPlaced && allFalsePhotosTrashed))
                {
                    feedbackGiven = true;
                    ShowFailFeedback();
                }
            }
        }
    }

    // 🔥 НОВЫЙ МЕТОД: проверяет, положил ли игрок хоть одну фотографию
    private bool AnyPhotoPlaced()
    {
        return foto1 || foto2 || foto3 || foto4 || foto5 || foto6;
    }

    // 🔥 НОВЫЙ МЕТОД: правильное решение
    private void ShowSuccessFeedback()
    {
        if (subtitleSpeaker != null && successClip != null)
        {
            subtitleSpeaker.voiceClip = successClip;
            subtitleSpeaker.subtitleText = successText;
            subtitleSpeaker.Play();
            Debug.Log("✅ Правильное решение головоломки с фотографиями!");
        }
        else
        {
            Debug.LogWarning("❌ Не назначен subtitleSpeaker или successClip в FotoManeger!");
        }
    }

    // 🔥 НОВЫЙ МЕТОД: неправильное решение
    private void ShowFailFeedback()
    {
        if (subtitleSpeaker != null && failClip != null)
        {
            subtitleSpeaker.voiceClip = failClip;
            subtitleSpeaker.subtitleText = failText;
            subtitleSpeaker.Play();
            Debug.Log("❌ Неправильное решение головоломки с фотографиями!");
        }
        else
        {
            Debug.LogWarning("❌ Не назначен subtitleSpeaker или failClip в FotoManeger!");
        }
    }
}

