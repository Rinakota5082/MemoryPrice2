using System.Reflection;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Rigidbody))]
public class PlastincMagnet : MonoBehaviour
{
    [Header("Настройки")]
    public float snapDistance = 0.5f;
    public float snapSpeed = 12f;

    [Tooltip("Тег зоны слота (PlSlot, Pl_ID, …)")]
    public string slotTag = "PlSlot";
    public Transform targetSlot;
    [Header("Звуки")]
    public AudioClip snapSound;
    [Range(0f, 1f)]
    public float snapSoundVolume = 1f;

    public bool playMelodyOnTriggerEnter = true;

    bool isSnapping;
    bool isPlaced;
    bool isBeingHeld;
    Rigidbody rb;
    XRGrabInteractable grabbable;
    Collider plasticCollider;
    Transform originalParent;
    private AudioSource audioSource;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grabbable = GetComponent<XRGrabInteractable>();
        plasticCollider = GetComponent<Collider>();
        originalParent = transform.parent;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.spatialBlend = 0.5f;
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        EnsureGrabUsesGravity();
        CacheDefaultSlot();
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

        if (!isSnapping && IsCloseToSlot())
            StartSnap();

        if (!isSnapping)
            return;

        var t = Time.deltaTime * snapSpeed;
        transform.position = Vector3.Lerp(transform.position, targetSlot.position, t);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetSlot.rotation, t);

        if (Vector3.Distance(transform.position, targetSlot.position) < 0.008f)
            FinishPlacement();
    }

    void OnSelectEntered(SelectEnterEventArgs args)
    {
        isBeingHeld = true;

        if (isPlaced)
            DetachFromSlot();

        if (isSnapping)
            CancelSnap();
    }

    public void OnSelectExited(SelectExitEventArgs args)
    {
        isBeingHeld = false;
        RestoreFreePhysics();
        StopMusic();
        if (!isPlaced)
            TrySnapToTargetSlot();
    }

    /// <summary>Вызывается PlastincSlot при входе в триггер.</summary>
    public void NotifyEnteredSlot(Transform slot, AudioClip slotMelodyOverride)
    {
        if (slot == null)
            return;

        targetSlot = slot;

        if (isPlaced || isBeingHeld)
            return;

        TrySnapToTargetSlot();
    }

    public void NotifyStayInSlot(Transform slot)
    {
        if (isPlaced || isBeingHeld || slot == null)
            return;

        targetSlot = slot;
        TrySnapToTargetSlot();
    }

    public void NotifyExitedSlot(Transform slot)
    {
        if (slot == null || targetSlot != slot)
            return;
        StopMusic();
        if (!isSnapping && !isPlaced)
            targetSlot = null;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!IsSlotCollider(other))
            return;

        //var clipOverride = GetClipFromSlot(other);
        NotifyEnteredSlot(other.transform, null);
    }

    void OnTriggerExit(Collider other)
    {
        if (!IsSlotCollider(other))
            return;

        NotifyExitedSlot(other.transform);
    }

    bool IsSlotCollider(Collider other)
    {
        if (other.GetComponent<PlastincSlot>() != null)
            return true;

        if (!string.IsNullOrEmpty(slotTag) && other.CompareTag(slotTag))
            return true;

        return false;
    }

    
    void TrySnapToTargetSlot()
    {
        if (targetSlot == null || isPlaced || isSnapping || isBeingHeld)
            return;

        if (IsCloseToSlot())
            StartSnap();
    }

    bool IsCloseToSlot()
    {
        return targetSlot != null && Vector3.Distance(transform.position, targetSlot.position) <= snapDistance;
    }

    void StartSnap()
    {
        if (isSnapping || isPlaced || targetSlot == null)
            return;

        isSnapping = true;

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        if (grabbable != null)
            grabbable.enabled = false;
    }

    void CancelSnap()
    {
        isSnapping = false;
        RestoreFreePhysics();

        if (grabbable != null)
            grabbable.enabled = true;
    }

    void FinishPlacement()
    {
        if (targetSlot == null)
        {
            isSnapping = false;
            return;
        }

        transform.SetPositionAndRotation(targetSlot.position, targetSlot.rotation);
        transform.SetParent(targetSlot, true);

        isSnapping = false;
        isPlaced = true;

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }

        if (grabbable != null)
            grabbable.enabled = true;

        PlayMusicOnPlastic();
    }
    private void PlayMusicOnPlastic()
    {
        AudioClip clip = snapSound;

        if (clip == null) return;

        // Воспроизводим музыку на пластине
        audioSource.clip = clip;
        audioSource.volume = snapSoundVolume;
        audioSource.Play();

        Debug.Log($"🎵 Пластина {gameObject.name} играет музыку: {clip.name}");
    }

    void DetachFromSlot()
    {
        isPlaced = false;
        isSnapping = false;
        transform.SetParent(originalParent, true);
        RestoreFreePhysics();
    }
    void RestoreFreePhysics()
    {
        if (rb == null)
            return;

        rb.constraints = RigidbodyConstraints.None;
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.WakeUp();
    }
    void EnsureGrabUsesGravity()
    {
        if (grabbable == null)
            return;

        var field = typeof(XRGrabInteractable).GetField(
            "m_ForceGravityOnDetach",
            BindingFlags.Instance | BindingFlags.NonPublic);

        field?.SetValue(grabbable, true);
    }

    void CacheDefaultSlot()
    {
        if (targetSlot != null)
            return;

        var slots = GameObject.FindGameObjectsWithTag(slotTag);
        if (slots.Length > 0)
            targetSlot = slots[0].transform;
    }

    public void RemoveFromSlot()
    {
        if (!isPlaced)
            return;
        StopMusic();
        DetachFromSlot();

        if (grabbable != null)
            grabbable.enabled = true;
    }
    public void StopMusic()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
            Debug.Log($"🛑 Музыка остановлена на {gameObject.name}");
        }
    }
}
