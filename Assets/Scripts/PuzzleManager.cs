using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public int totalBooks = 5;
    private int placedBooks = 0;

    public void CheckAllBooksPlaced()
    {
        placedBooks++;
        Debug.Log("Книг поставлено: " + placedBooks + " из " + totalBooks);

        if (placedBooks >= totalBooks)
        {
            Debug.Log("ВСЕ КНИГИ НА МЕСТЕ!");
            // Здесь будет код включения ночника
        }
    }
}