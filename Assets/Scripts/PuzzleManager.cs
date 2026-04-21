using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public int totalBooks = 5;
    private int placedBooks = 0;
    private bool puzzleCompleted = false; // Флаг, чтобы не запустить финал дважды

    public void CheckAllBooksPlaced()
    {
        if (puzzleCompleted) return;

        placedBooks++;
        Debug.Log("Книг поставлено: " + placedBooks + " из " + totalBooks);

        if (placedBooks >= totalBooks)
        {
            puzzleCompleted = true;
            Debug.Log("ГОЛОВОЛОМКА РЕШЕНА!");
            OnPuzzleSolved();
        }
    }

    void OnPuzzleSolved()
    {
        // Сюда добавьте включение ночника, звук победы и т.д.
        // Пример: nightLamp.SetActive(true);
    }
}