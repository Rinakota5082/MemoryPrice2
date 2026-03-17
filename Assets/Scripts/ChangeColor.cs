using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
public class ChangeColor : MonoBehaviour
{
    [SerializeField] string _tag;
    [SerializeField] Color _newColor;
    // Update is called once per frame
    public void ChangeC()
    {
        
        foreach( GameObject tagged in GameObject.FindGameObjectsWithTag(_tag))
        {
           if( tagged.TryGetComponent<MeshRenderer>(out MeshRenderer renderer))
            {
                renderer.material.color = _newColor;
            }
        }
    }
    public void ChangeColorBack()
    {
        foreach (GameObject tagged in GameObject.FindGameObjectsWithTag(_tag))
        {
            if (tagged.TryGetComponent<MeshRenderer>(out MeshRenderer renderer))
            {
                renderer.material.color = new Color();
            }
        }
    }
}
