using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CollectionSlot : MonoBehaviour
{
    public GameObject slotPrefab; // —сылка на префаб UI-слота
    public Transform gridContainer; // —сылка на объект с Grid Layout Group

    private void OnEnable()
    {
        StartCoroutine(DisplayCollection());
    }

    IEnumerator DisplayCollection()
    {
        foreach (Transform child in gridContainer) Destroy(child.gameObject);
        yield return null; 

        var allItems = CollectionManager.Instance.allCollectibleItems;
        foreach (var item in allItems)
        {
            GameObject slot = Instantiate(slotPrefab, gridContainer);

            Image iconImage = slot.GetComponentInChildren<Image>();
            Text nameText = slot.GetComponentInChildren<Text>();

            bool isUnlocked = CollectionManager.Instance.IsItemUnlocked(item.itemID);
            if (isUnlocked)
            {
                if (iconImage != null) iconImage.sprite = item.icon;
                if (nameText != null) nameText.text = item.displayName;
                slot.GetComponent<Button>().interactable = true;
            }
            else
            {
                //if (iconImage != null) iconImage.sprite= "???" ;
                if (nameText != null) nameText.text = "???";
                slot.GetComponent<Button>().interactable = false;
            }
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        foreach (Transform child in gridContainer) Destroy(child.gameObject);
    }
}