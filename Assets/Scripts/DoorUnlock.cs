using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class DoorUnlock : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private XRGrabInteractable doorGrab;   // Ссылка на XR Grab Interactable ДВЕРИ
    [SerializeField] private HingeJoint doorHinge;          // Hinge Joint ДВЕРИ

    [Header("Visual & Audio")]
    [SerializeField] private MeshRenderer doorRenderer;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip unlockSound;

    private bool isUnlocked = false;

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

        // . ДВЕРЬ теперь можно хватать!
        if (doorGrab != null)
            doorGrab.enabled = true;

        // . Включаем лимиты вращения двери
        if (doorHinge != null)
            doorHinge.useLimits = true;

        // . Звук
        if (audioSource != null && unlockSound != null)
            audioSource.PlayOneShot(unlockSound);

        Debug.Log($"Door UNLOCKED! Now you can grab the DOOR.");
    }
}