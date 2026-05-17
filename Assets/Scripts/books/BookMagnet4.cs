using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class BookMagnet4 : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private Door doorToUnlock;
    public float snapDistance = 0.15f;
    public float snapSpeed = 10f;
    public string slotTag = "BookSlot4";
    public Transform targetSlot;
    private bool isSnapping = false;
    private bool isPlaced = false;
    private Rigidbody rb;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabbable;
    private Collider bookCollider;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        grabbable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        bookCollider = GetComponent<Collider>();

        // ← Автоматическая привязка события (не нужно в Inspector!)
        if (grabbable != null)
        {
            grabbable.selectExited.AddListener(OnSelectExited);
        }
    }

    void Update()
    {
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

    // ← public обязательно!
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
        if (rb != null) rb.isKinematic = true;
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

        if (doorToUnlock != null)
            doorToUnlock.Point4 = true;

        FindObjectOfType<PuzzleManager>()?.CheckAllBooksPlaced();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BookSlot4") && targetSlot != null) { targetSlot = other.transform; }
    }

}