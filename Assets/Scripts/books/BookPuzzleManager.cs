using UnityEngine;
using System.Collections.Generic;

public class BookPuzzleManager : MonoBehaviour
{
    [Header("Настройки порядка книг")]
    [Tooltip("Правильный порядок: имена книг в том порядке, как они должны стоять на полке (слева направо)")]
    public string[] correctOrder = { "Колобок", "Маленький принц", "Мстители", "Гарри Поттер", "Властелин колец" };

    [Header("Слоты для книг (перетащите слоты в правильном порядке слева направо)")]
    public Transform[] bookSlots;  // Слоты на полке в правильном порядке
    public string[] slotTags = { "BookSlot1", "BookSlot2", "BookSlot3", "BookSlot4", "BookSlot5" };

    [Header("Книги и их имена")]
    public GameObject[] books;  // Все книги-объекты
    public string[] bookNames;  // Имена книг (должны совпадать с correctOrder)

    [Header("Компоненты для обратной связи")]
    public TypewriterSubtitle subtitleSpeaker;  // Ссылка на менеджер субтитров
    public AudioClip wrongOrderClip;   // Аудио для "Нет, не так..."
    public AudioClip correctOrderClip; // Аудио для "Ты слышишь? Она говорит спасибо..."

    [Header("Переход к следующей головоломке")]
    public GameObject photoPuzzleObject;  // Объект/панель с головоломкой фотографий
    public GameObject bookPuzzleObject;   // Объект/панель с головоломкой книг

    private Dictionary<Transform, string> slotToBookMap = new Dictionary<Transform, string>();
    private bool puzzleCompleted = false;

    void Start()
    {
        // Инициализируем словарь: каждый слот связан с именем книги, которая туда поставлена
        foreach (Transform slot in bookSlots)
        {
            slotToBookMap[slot] = null;
        }

        // Скрываем головоломку с фото, пока книги не решены
        if (photoPuzzleObject != null)
            photoPuzzleObject.SetActive(false);
    }

    // Вызывается из BookMagnet, когда книга установлена на слот
    public void BookPlaced(Transform slot, string bookName)
    {
        if (puzzleCompleted) return;

        // Находим индекс слота
        int slotIndex = System.Array.IndexOf(bookSlots, slot);
        if (slotIndex == -1) return;

        // Сохраняем, какая книга стоит в этом слоте
        slotToBookMap[slot] = bookName;

        // Проверяем, все ли слоты заполнены
        bool allFilled = true;
        foreach (var kvp in slotToBookMap)
        {
            if (kvp.Value == null)
            {
                allFilled = false;
                break;
            }
        }

        if (allFilled)
        {
            CheckPuzzleCompletion();
        }
    }

    void CheckPuzzleCompletion()
    {
        bool isCorrect = true;

        // Проверяем порядок: каждый слот должен содержать правильную книгу
        for (int i = 0; i < bookSlots.Length; i++)
        {
            string placedBook = slotToBookMap[bookSlots[i]];
            string expectedBook = correctOrder[i];

            if (placedBook != expectedBook)
            {
                isCorrect = false;
                break;
            }
        }

        if (isCorrect)
        {
            // 🎉 ПРАВИЛЬНЫЙ ПОРЯДОК!
            puzzleCompleted = true;

            if (subtitleSpeaker != null && correctOrderClip != null)
            {
                subtitleSpeaker.voiceClip = correctOrderClip;
                subtitleSpeaker.subtitleText = "Ты слышишь? Она говорит «спасибо». Ты всегда был для неё героем. А потом… что случилось потом, Марк? Осмотри комнату и найди следующую загадку.";
                subtitleSpeaker.Play();
            }

            // 🚪 Открываем доступ ко второй головоломке

            if (photoPuzzleObject != null)
                photoPuzzleObject.SetActive(true);   // Показываем головоломку с фотографиями
        }
        else
        {
            // ❌ НЕПРАВИЛЬНЫЙ ПОРЯДОК!
            if (subtitleSpeaker != null && wrongOrderClip != null)
            {
                subtitleSpeaker.voiceClip = wrongOrderClip;
                subtitleSpeaker.subtitleText = "Нет, не так. Порядок должен быть другой.";
                subtitleSpeaker.Play();
            }

            // Можно также подсветить красным неправильные места или дать подсказку
            HighlightWrongSlots();
        }
    }

    void HighlightWrongSlots()
    {
        // Опционально: подсветить слоты, где книги стоят неправильно
        for (int i = 0; i < bookSlots.Length; i++)
        {
            string placedBook = slotToBookMap[bookSlots[i]];
            string expectedBook = correctOrder[i];

            if (placedBook != expectedBook && placedBook != null)
            {
                // Например, изменить цвет материала слота
                Renderer renderer = bookSlots[i].GetComponent<Renderer>();
                if (renderer != null)
                    renderer.material.color = Color.red;
            }
        }

        // Через 2 секунды вернуть нормальный цвет
        Invoke(nameof(ResetSlotColors), 2f);
    }

    void ResetSlotColors()
    {
        foreach (Transform slot in bookSlots)
        {
            Renderer renderer = slot.GetComponent<Renderer>();
            if (renderer != null)
                renderer.material.color = Color.white;
        }
    }
}