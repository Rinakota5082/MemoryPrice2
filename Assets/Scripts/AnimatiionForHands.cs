using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
public class AnimatiionForHands : MonoBehaviour
{
    [SerializeField]
    XRInputValueReader<float> m_TriggerInput;
    [SerializeField]
    XRInputValueReader<float> m_GripInput;
    [SerializeField] Animator animator;
    
    // Update is called once per frame
    void Update()
    {
        //Добавить на предметы компоненты XR Grab Interactable для взаимодействия с ними
        //Debug.Log(m_TriggerInput.ReadValue()+" "+m_GripInput.ReadValue());
        animator.SetFloat("Trigger",m_TriggerInput.ReadValue());
        animator.SetFloat("Grip", m_GripInput.ReadValue());
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    /*void Start()
    {
        
    }
    */
}
