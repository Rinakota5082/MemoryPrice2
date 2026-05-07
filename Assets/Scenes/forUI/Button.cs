using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
public class Button : MonoBehaviour
{
    internal bool interactable;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void ChangeScene( string sceneName)
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogWarning("Не найдено");
        }
    }
}
