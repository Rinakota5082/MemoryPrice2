using UnityEngine;

public class TriggerSubtitle : MonoBehaviour
{
    [Tooltip("Менеджер субтитров, который нужно запустить")]
    public SubtitleManager subtitleManager;

    [Tooltip("Изменить аудиоклип перед воспроизведением (опционально)")]
    public AudioClip newClip;

    [Tooltip("Воспроизвести только один раз")]
    public bool once = true;

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (once && triggered) return;
        if (!other.CompareTag("Player")) return; // убедитесь, что у игрока тег "Player"

        if (subtitleManager != null)
        {
            if (newClip != null)
                subtitleManager.GetComponent<AudioSource>().clip = newClip;

            subtitleManager.PlayWithSubtitles();
            triggered = true;
        }
    }
}