using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Door : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private XRGrabInteractable doorGrab;   // Ссылка на XR Grab Interactable ДВЕРИ
    [SerializeField] private HingeJoint doorHinge;          // Hinge Joint ДВЕРИ

    [Header("Visual & Audio")]
    [SerializeField] private MeshRenderer doorRenderer;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip unlockSound;

    [Header("Условия открытия")]
    public bool Point1 = false;  // Колонка 1 (должно быть 9)
    public bool Point2 = false;  // Колонка 2 (должно быть 5)
    public bool Point3 = false;  // Колонка 3 (должно быть 10)
    public bool Point4 = false;  // Колонка 4 (должно быть 2)
    public bool Point5 = false;  // Правильная пластинка (PlastincMagnet1)

    [Header("Обратная связь")]
    public TypewriterSubtitle subtitleSpeaker;
    public AudioClip wrongPlasticClip;
    public AudioClip wrongKolonkaClip;
    public AudioClip successClip;
    public string wrongPlasticText = "Пластинка не та. Это не её музыка.";
    public string wrongKolonkaText = "Звук не тот. Настройки сбились. Или память подводит?";
    public string successText = "Правильный выбор. Ты помнишь обложку. Помнишь, как она держала этот диск в руках и улыбалась. А потом поставила в граммофон и сказала: «Это наш с тобой саундтрек, Марк.»";

    private bool isUnlocked = false;      // Дверь уже открыта?
    private bool feedbackGiven = false;   // Уже давали обратную связь об ошибке?
    private bool puzzleCompleted = false; // Головоломка уже полностью решена?

    void Start()
    {
        // Изначально ДВЕРЬ нельзя хватать
        if (doorGrab != null)
            doorGrab.enabled = false;

        // Блокируем вращение двери
        if (doorHinge != null)
            doorHinge.useLimits = false;
    }

    void Update()
    {
        // Проверяем, все ли условия выполнены
        if (Point1 && Point2 && Point3 && Point4 && Point5)
        {
            Unlock();
            CheckAndGiveFeedback();
        }
        else
        {
            CheckAndGiveFeedback();
        }
    }

    public void Unlock()
    {
        if (isUnlocked) return;
        isUnlocked = true;

        if (doorGrab != null)
            doorGrab.enabled = true;

        if (doorHinge != null)
            doorHinge.useLimits = true;

        if (audioSource != null && unlockSound != null)
            audioSource.PlayOneShot(unlockSound);

        Debug.Log($"Door UNLOCKED! Now you can grab the DOOR.");
    }

    private void CheckAndGiveFeedback()
    {
        // Если головоломка уже решена — больше ничего не говорим
        if (puzzleCompleted) return;

        bool plasticCorrect = Point5;
        bool allKolonkasCorrect = Point1 && Point2 && Point3 && Point4;

        // ✅ ВСЁ ПРАВИЛЬНО
        if (plasticCorrect && allKolonkasCorrect && !puzzleCompleted)
        {
            puzzleCompleted = true;
            ShowSuccessFeedback();
            return;
        }

        // Если уже давали обратную связь об ошибке — не повторяем
        if (feedbackGiven) return;

        // Проверяем, хоть что-то уже сделано (чтобы не говорить об ошибке в пустой комнате)
        bool anythingDone = Point1 || Point2 || Point3 || Point4 || Point5;
        if (!anythingDone) return;

        // ❌ ПЛАСТИНКА НЕПРАВИЛЬНАЯ (а колонки уже настраивали)
        if (!plasticCorrect && (Point1 || Point2 || Point3 || Point4))
        {
            feedbackGiven = true;
            ShowWrongPlasticFeedback();
            return;
        }

        // ❌ КОЛОНКИ НАСТРОЕНЫ НЕПРАВИЛЬНО (но пластинка правильная)
        if (plasticCorrect && !allKolonkasCorrect)
        {
            feedbackGiven = true;
            ShowWrongKolonkaFeedback();
            return;
        }
    }

    private void ShowWrongPlasticFeedback()
    {
        if (subtitleSpeaker != null && wrongPlasticClip != null)
        {
            subtitleSpeaker.voiceClip = wrongPlasticClip;
            subtitleSpeaker.subtitleText = wrongPlasticText;
            subtitleSpeaker.Play();
            Debug.Log("❌ Неправильная пластинка!");
        }
        else
        {
            Debug.LogWarning("❌ Не назначен subtitleSpeaker или wrongPlasticClip в Door!");
        }
    }

    private void ShowWrongKolonkaFeedback()
    {
        if (subtitleSpeaker != null && wrongKolonkaClip != null)
        {
            subtitleSpeaker.voiceClip = wrongKolonkaClip;
            subtitleSpeaker.subtitleText = wrongKolonkaText;
            subtitleSpeaker.Play();
            Debug.Log("❌ Неправильная настройка колонок!");
        }
        else
        {
            Debug.LogWarning("❌ Не назначен subtitleSpeaker или wrongKolonkaClip в Door!");
        }
    }

    private void ShowSuccessFeedback()
    {
        if (subtitleSpeaker != null && successClip != null)
        {
            subtitleSpeaker.voiceClip = successClip;
            subtitleSpeaker.subtitleText = successText;
            subtitleSpeaker.Play();
            Debug.Log("✅ Третья головоломка решена правильно!");
        }
        else
        {
            Debug.LogWarning("❌ Не назначен subtitleSpeaker или successClip в Door!");
        }
    }

    // Опционально: сброс флага ошибки (можно вызывать при изменении пластинки или настроек)
    public void ResetFeedback()
    {
        if (!puzzleCompleted)
            feedbackGiven = false;
    }
}