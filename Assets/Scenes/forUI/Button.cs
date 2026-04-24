using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
public class Button : MonoBehaviour
{
    
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void ChangeScene()
    {
        SceneManager.LoadScene("Assets/Scenes/SampleScene.unity");
    }
}
