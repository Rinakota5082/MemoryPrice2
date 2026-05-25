using UnityEngine;

public class FotoManeger : MonoBehaviour
{
    [SerializeField] private Door1 doorToUnlock;
    public bool foto1 = false;
    public bool foto2 = false;
    public bool foto3 = false;
    public bool foto4 = false;
    public bool foto5 = false;
    public bool foto6 = false;
    void Start()
    {
        
    }
    void Update()
    {
        if (foto1 && foto2 && foto3 && foto4 && foto5) { doorToUnlock.Point6 = true; }
    }

}
