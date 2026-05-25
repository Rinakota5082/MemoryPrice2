using UnityEngine;
using TMPro;


public class VoiceTrigger : MonoBehaviour
{
    public GameObject voicePrefab;      // сюда перетащите префаб Voice_Replica
    public AudioClip voiceClip;         // конкретный аудиофайл для этой реплики
    [TextArea(2, 5)]
    public string subtitleText;         // текст субтитра для этой реплики
    public bool playOnce = true;

    private bool hasPlayed = false;

    private void OnTriggerEnter(Collider other)
    {
        if (playOnce && hasPlayed) return;
        if (!other.CompareTag("Player")) return;

        if (voicePrefab != null && voiceClip != null)
        {
            // Создаём объект с голосом
            GameObject go = Instantiate(voicePrefab, transform.position, Quaternion.identity);
            TypewriterSubtitle ts = go.GetComponent<TypewriterSubtitle>();
            if (ts != null)
            {
                ts.voiceClip = voiceClip;
                ts.subtitleText = subtitleText;
                // Находим TextMeshPro на Canvas и подставляем
                ts.subtitleDisplay = FindFirstObjectByType<TextMeshProUGUI>();
                ts.Play();
            }

            // Уничтожаем объект после окончания аудио + чуть на запас
            Destroy(go, (voiceClip.length + 2f));
            hasPlayed = true;
        }
    }
}