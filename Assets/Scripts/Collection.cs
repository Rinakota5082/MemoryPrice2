using UnityEngine;

[CreateAssetMenu(fileName = "Collection", menuName = "Scriptable Objects/Collection")]
public class Collection : ScriptableObject
{
    // Уникальный ID предмета
    public string itemID;
    // Название для отображения в UI
    public string displayName;
    // Иконка
    public Sprite icon;
    public string description;
}
