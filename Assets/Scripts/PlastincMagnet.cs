using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class PlastincMagnet : MonoBehaviour
{
    [Header("Настройки")]
    public float snapDistance = 0.35f;
    public float snapSpeed = 10f;
    public string slotTag = "PlSlot";

    [Tooltip("Если пусто — ищется слот по тегу при входе в триггер")]
    public Transform targetSlot;

    [Header("Звуки")]
    [Tooltip("Нота/мелодия этой пластинки (если на слоте нет Music)")]
    public AudioClip snapSound;

    [Range(0f, 1f)]
    public float snapSoundVolume = 0.7f;

    public AudioSource audioSource;

    [Tooltip("Проигрывать ноту при входе в зону слота (до прилипания)")]
    public bool playMelodyOnTriggerEnter = true;

    bool isSnapping;
    bool isPlaced;
    bool isBeingHeld;
    bool playedMelodyThisVisit;

    Rigidbody rb;
    XRGrabInteractable grabbable;
    Collider plasticCollider;
    Transform originalParent;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grabbable = GetComponent<XRGrabInteractable>();
        plasticCollider = GetComponent<Collider>();
        originalParent = transform.parent;

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioSource == null && snapSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1f;
        }
    }

    void OnEnable()
    {
        if (grabbable == null)
            return;

        grabbable.selectEntered.AddListener(OnSelectEntered);
        grabbable.selectExited.AddListener(OnSelectExited);
    }

    void OnDisable()
    {
        if (grabbable == null)
            return;

        grabbable.selectEntered.RemoveListener(OnSelectEntered);
        grabbable.selectExited.RemoveListener(OnSelectExited);
    }

    void Update()
    {
        if (isPlaced || isBeingHeld || targetSlot == null)
            return;

        if (!isSnapping && Vector3.Distance(transform.position, targetSlot.position) <= snapDistance)
            StartSnap();

        if (!isSnapping)
            return;

        transform.position = Vector3.Lerp(transform.position, targetSlot.position, Time.deltaTime * snapSpeed);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetSlot.rotation, Time.deltaTime * snapSpeed);

        if (Vector3.Distance(transform.position, targetSlot.position) < 0.01f)
            FinishPlacement();
    }

    void OnSelectEntered(SelectEnterEventArgs args)
    {
        isBeingHeld = true;
    }

    public void OnSelectExited(SelectExitEventArgs args)
    {
        isBeingHeld = false;

        if (isPlaced)
            return;

        TrySnapToTargetSlot();
    }

    void OnTriggerEnter(Collider other)
    {
        if (isPlaced || !other.CompareTag(slotTag))
            return;

        targetSlot = other.transform;
        playedMelodyThisVisit = false;

        if (playMelodyOnTriggerEnter)
            PlayMelodyForSlot(other);

        if (!isBeingHeld)
            TrySnapToTargetSlot();
    }

    void OnTriggerStay(Collider other)
    {
        if (isPlaced || isBeingHeld || !other.CompareTag(slotTag))
            return;

        if (targetSlot == null)
            targetSlot = other.transform;

        TrySnapToTargetSlot();
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(slotTag))
            return;

        playedMelodyThisVisit = false;

        if (!isSnapping && targetSlot == other.transform)
            targetSlot = null;
    }

    void TrySnapToTargetSlot()
    {
        if (targetSlot == null || isPlaced || isSnapping || isBeingHeld)
            return;

        if (Vector3.Distance(transform.position, targetSlot.position) <= snapDistance)
            StartSnap();
    }

    void StartSnap()
    {
        if (isSnapping || isPlaced || targetSlot == null)
            return;

        isSnapping = true;

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.useGravity = false;
        }

        if (plasticCollider != null)
            plasticCollider.isTrigger = true;

        if (grabbable != null)
            grabbable.enabled = false;
    }

    void FinishPlacement()
    {
        if (targetSlot == null)
        {
            isSnapping = false;
            return;
        }

        transform.SetPositionAndRotation(targetSlot.position, targetSlot.rotation);
        transform.SetParent(targetSlot);

        isSnapping = false;
        isPlaced = true;

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }

        if (plasticCollider != null)
        {
            plasticCollider.isTrigger = false;
            plasticCollider.enabled = true;
        }

        if (grabbable != null)
            grabbable.enabled = true;

        PlayMelodyForSlot(targetSlot, force: true);
        FindFirstObjectByType<PuzzleManager>()?.CheckAllBooksPlaced();
    }

    void PlayMelodyForSlot(Component slot, bool force = false)
    {
        if (!force && playedMelodyThisVisit)
            return;

        var clip = snapSound;
        if (slot != null)
        {
            var slotMusic = slot.GetComponent<Music>();
            if (slotMusic != null && slotMusic.melodyClip != null)
                clip = slotMusic.melodyClip;
        }

        if (clip == null)
            return;

        if (audioSource != null)
            audioSource.PlayOneShot(clip, snapSoundVolume);
        else
            AudioSource.PlayClipAtPoint(clip, transform.position, snapSoundVolume);

        playedMelodyThisVisit = true;
    }

    public void RemoveFromSlot()
    {
        if (!isPlaced)
            return;

        isPlaced = false;
        isSnapping = false;
        playedMelodyThisVisit = false;

        transform.SetParent(originalParent);

        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.None;
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        if (grabbable != null)
            grabbable.enabled = true;

        if (plasticCollider != null)
        {
            plasticCollider.enabled = true;
            plasticCollider.isTrigger = false;
        }
    }
}
