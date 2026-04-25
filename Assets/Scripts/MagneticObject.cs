using UnityEngine;

using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class MagneticObject : MonoBehaviour
{
    [Header("Target")]

    public Transform targetPoint;

    [Header("Magnet Settings")]

    public float magnetSpeed = 5f;

    public float reachDistance = 0.1f;

    public bool attractOnlyWhenReleased = true;

    [Header("Events")]
    public UnityEngine.Events.UnityEvent onObjectReached;

    private bool isAttracting = false;
    private Rigidbody rb;
    private bool isHeld = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        var grabInteractable = GetComponent<XRGrabInteractable>();
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(_ => isHeld = true);
            grabInteractable.selectExited.AddListener(_ => isHeld = false);
        }
    }

    void Update()
    {
        if (!isAttracting) return;

        if (attractOnlyWhenReleased && isHeld) return;

        float step = magnetSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, step);
        if (Vector3.Distance(transform.position, targetPoint.position) <= reachDistance)
        {
            isAttracting = false;
            onObjectReached?.Invoke();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;

                rb.isKinematic = true;
            }
            //Debug.Log($"{gameObject.name} ������ ����!");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BookSlot"))
        {
            isAttracting = true;
            //Debug.Log($"{gameObject.name} ����� � ��������� ����!");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("BookSlot"))
        {
            isAttracting = false;
            //Debug.Log($"{gameObject.name} ������� ��������� ����");
        }
    }
}