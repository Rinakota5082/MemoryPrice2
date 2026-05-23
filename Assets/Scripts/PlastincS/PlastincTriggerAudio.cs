using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlastincTriggerAudio : MonoBehaviour
{
    [Tooltip("Мелодия этой пластинки")]
    public AudioClip melody;

    [Range(0f, 1f)]
    public float volume = 1f;

    [Tooltip("Теги зоны-триггера (PlSlot, Pl_ID и т.д.)")]
    public string[] slotTags = { "PlSlot" };

    PlastincMagnet magnet;

    void Awake()
    {
        magnet = GetComponent<PlastincMagnet>();
        if (melody == null && magnet != null)
            melody = magnet.snapSound;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!IsSlotCollider(other))
            return;
    }

    bool IsSlotCollider(Collider other)
    {
        if (other.GetComponent<PlastincSlot>() != null)
            return true;

        if (slotTags == null)
            return false;

        for (var i = 0; i < slotTags.Length; i++)
        {
            var tag = slotTags[i];
            if (!string.IsNullOrEmpty(tag) && other.CompareTag(tag))
                return true;
        }
        return false;
    }

    
}
