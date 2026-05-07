using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class VRCollectible : MonoBehaviour
{
    public Collection myCollectibleData;

    public ParticleSystem collectionEffect;
    public AudioClip collectionSound;

    private void Start()
    {
        var grabInteractable = GetComponent<XRGrabInteractable>();
        // Проверяем, не собран ли уже этот предмет
        if (CollectionManager.Instance != null && CollectionManager.Instance.IsItemUnlocked(myCollectibleData.itemID))
        {
            // Если уже собран — сразу удаляем из сцены
            Destroy(gameObject);
            return;
        }
        if (grabInteractable != null)
            grabInteractable.selectEntered.AddListener(OnItemGrabbed);
    }

    private void OnItemGrabbed(SelectEnterEventArgs args)
    {
        // Сообщаем Менеджеру, что этот предмет собран
        CollectionManager.Instance.UnlockItem(myCollectibleData.itemID);
        if (collectionEffect != null) collectionEffect.Play();
        if (collectionSound != null) AudioSource.PlayClipAtPoint(collectionSound, transform.position);
        Destroy(gameObject);
    }
}