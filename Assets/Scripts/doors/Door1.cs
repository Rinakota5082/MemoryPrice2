using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Door1 : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private XRGrabInteractable doorGrab;   // Ссылка на XR Grab Interactable ДВЕРИ
    [SerializeField] private HingeJoint doorHinge;          // Hinge Joint ДВЕРИ

    [Header("Visual & Audio")]
    [SerializeField] private MeshRenderer doorRenderer;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip unlockSound;

    public bool Point1 = false;
    public bool Point2 = false;
    public bool Point3 = false;
    public bool Point4 = false;
    public bool Point5 = false;
    public bool Point6 = false;
    private bool isUnlocked = false;                        //счетчик

    public void Update()
    {
        if (Point1 && Point2 && Point3 && Point4 && Point5 && Point6) { Unlock(); }
    }
    void Start()
    {
        // Изначально ДВЕРЬ нельзя хватать
        if (doorGrab != null)
            doorGrab.enabled = false;
        // Блокируем вращение двери
        if (doorHinge != null)
            doorHinge.useLimits = false;
    }
    
    public void Unlock()
    {
        if (isUnlocked) return;
        isUnlocked = true;
        if (doorGrab != null)
            doorGrab.enabled = true;
        if (doorHinge != null)
            doorHinge.useLimits = true;
        if (audioSource != null && unlockSound != null)
            audioSource.PlayOneShot(unlockSound);
        Debug.Log($"Door UNLOCKED! Now you can grab the DOOR.");
    }
}