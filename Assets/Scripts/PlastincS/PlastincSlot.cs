using UnityEngine;

/// <summary>
/// Триггер-зона слота для пластинок (тег PlSlot или свой тег, напр. Pl_ID).
/// </summary>
[RequireComponent(typeof(Collider))]
public class PlastincSlot : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        var magnet = FindMagnet(other);
        if (magnet == null)
            return;

       /* var triggerAudio = magnet.GetComponent<PlastincTriggerAudio>();
        if (triggerAudio != null)
            //triggerAudio.PlayMelody();*/

        magnet.NotifyEnteredSlot(transform, null);
    }

    void OnTriggerStay(Collider other)
    {
        var magnet = FindMagnet(other);
        if (magnet == null)
            return;

        magnet.NotifyStayInSlot(transform);
    }

    void OnTriggerExit(Collider other)
    {
        var magnet = FindMagnet(other);
        if (magnet == null)
            return;

        magnet.NotifyExitedSlot(transform);
    }

    static PlastincMagnet FindMagnet(Collider other)
    {
        var magnet = other.GetComponentInParent<PlastincMagnet>();
        if (magnet == null)
            magnet = other.GetComponent<PlastincMagnet>();
        return magnet;
    }

}
