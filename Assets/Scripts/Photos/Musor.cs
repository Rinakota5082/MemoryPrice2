using System.Reflection;
using System.Timers;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
public class Musor : MonoBehaviour
{
    [Header("Настройки")]
    public float snapDistance = 0.5f;
    public float snapSpeed = 12f;
    [SerializeField] private FotoManeger fotos;

    [Tooltip("Тег зоны слота ")]
    public string slotTag = "Musor";
    public Transform targetSlot;

    bool isSnapping;
    bool isPlaced;
    string tag;
    Rigidbody rb;
    XRGrabInteractable grabbable;
    Collider fotoCollider;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        grabbable = GetComponent<XRGrabInteractable>();
        fotoCollider = GetComponent<Collider>();
        grabbable.selectExited.AddListener(OnSelectExited);
        tag = gameObject.tag;
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
    void OnSelectExited(SelectExitEventArgs args)
    {
        if (isPlaced) return;
        if (targetSlot != null && Vector3.Distance(transform.position, targetSlot.position) <= snapDistance)
        { StartSnap(); }
    }
    void StartSnap()
    {
        isSnapping = true;
        if (rb != null) rb.isKinematic = true;
        if (fotoCollider != null) fotoCollider.enabled = false;
        if (grabbable != null) grabbable.enabled = false;
    }
    void FinishPlacement()
    {
        transform.position = targetSlot.position;
        transform.rotation = targetSlot.rotation;

        isSnapping = false;
        isPlaced = true;
        transform.SetParent(targetSlot);
        if (tag == "BreakFoto")
        {
            if (fotos.foto1)
            {
                if (fotos.foto2)
                {
                    if (fotos.foto3)
                    {
                        fotos.foto4 = true;
                    }
                    else{ fotos.foto3 = true; }
                }
                else { fotos.foto2 = true; }
            }
            else{ fotos.foto1 = true; }
        }
    }
}
