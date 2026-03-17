using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
public class GrabParenter : MonoBehaviour
{
    
    public void OnGrab(SelectEnterEventArgs args)
    {
        //Debug.Log("Взял");
        args.interactableObject.transform.SetParent(args.interactorObject.transform);
    }
    public void OnUngrab(SelectExitEventArgs args)
    {
        args.interactableObject.transform.SetParent(null);
    }

}
