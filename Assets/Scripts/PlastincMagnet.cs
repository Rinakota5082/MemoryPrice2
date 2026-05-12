using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.XR.Interaction.Toolkit;

public class PlastincMagnet : MonoBehaviour
{
    [Header("Настройки")]
    public float snapDistance = 0.15f;
    public float snapSpeed = 10f;
    public string slotTag = "PlSlot";
    public Transform targetSlot;
    [Header("Звуки")]
    public AudioClip snapSound;
    public AudioSource audioSource;

    private bool isSnapping = false;
    private bool isPlaced = false;
    private Rigidbody rb;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabbable;
    private Collider bookCollider;
    private Transform originalParent;
    private bool isBeingHeld = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        grabbable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        bookCollider = GetComponent<Collider>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && snapSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
        if (grabbable != null)
        {
            grabbable.selectEntered.AddListener(_=>isBeingHeld=true);
            grabbable.selectExited.AddListener(OnSelectExited);
        }
    }

    void Update()
    {
        if (!isPlaced && !isBeingHeld && targetSlot != null)
        {
            float diatanceToSlot = Vector3.Distance(transform.position, targetSlot.position);
            if(diatanceToSlot <= snapDistance&& !isSnapping)
            {
                StartSnap();
            }
        }
        {
            
        }
        if (isSnapping && targetSlot != null)
        {
            transform.position = Vector3.Lerp(transform.position, targetSlot.position, Time.deltaTime * snapSpeed);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetSlot.rotation, Time.deltaTime * snapSpeed);

            if (Vector3.Distance(transform.position, targetSlot.position) < 0.01f)
            {
                FinishPlacement();
            }
        }
    }
    public void OnSelectExited(SelectExitEventArgs args)
    {
        if (isPlaced) return;

        if (targetSlot != null && Vector3.Distance(transform.position, targetSlot.position) <= snapDistance)
        {
            StartSnap();
        }
    }

    void StartSnap()
    {
        isSnapping = true;
        if (rb != null) { rb.isKinematic = true;    rb.linearVelocity = Vector3.zero;  rb.useGravity = false; }
        if (bookCollider != null) bookCollider.enabled = false;
        if (grabbable != null) grabbable.enabled = false;
    }

    void FinishPlacement()
    {
        transform.position = targetSlot.position;
        transform.rotation = targetSlot.rotation;

        isSnapping = false;
        isPlaced = true;
        transform.SetParent(targetSlot);
        if (grabbable != null)
        {
            grabbable.enabled = true;
            rb.isKinematic = false;
            rb.useGravity = false;
            rb.angularVelocity= Vector3.zero;
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }
        if (bookCollider != null) bookCollider.enabled = true;
        PlaySnapSound();
        FindObjectOfType<PuzzleManager>()?.CheckAllBooksPlaced();
    }
    void PlaySnapSound()
    {
        if (snapSound != null)
        {
            if(audioSource != null) audioSource.PlayOneShot(snapSound,0.5f);
            Debug.Log("Playing");
        }
        
    }
    public void RemoveFromSlot()
    {
        if (!isPlaced) return;

        isPlaced = false;
        transform.SetParent(originalParent);
        if (rb != null){rb.isKinematic = false; rb.useGravity = true; }
        if (grabbable != null){ grabbable.enabled = true;}
        if (bookCollider != null){bookCollider.enabled = true; }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlSlot") && targetSlot != null) { targetSlot = other.transform; }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(slotTag) && targetSlot == other.transform)
        {
            // Не сбрасываем targetSlot сразу, чтобы не прерывать начатое притягивание
            if (!isSnapping)
            {
                targetSlot = null;
            }
        }
    }
}