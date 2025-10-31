using System.Collections.Generic;
using UnityEngine;

public class PuzzleController : MonoBehaviour
{
    [Header("Puzzle Pieces in Order")]
    public List<PuzzlePiece> puzzlePieces; // Order matters

    private int currentIndex = 0;

    void Start()
    {
        UpdateGlow();
    }

    public void TrySnap(PuzzlePiece piece, Transform target)
    {
        // Only snap if it's the current piece and the target is allowed
        if (piece == puzzlePieces[currentIndex] && piece.allowedTargets.Contains(target))
        {
            piece.SnapIntoPlace(target);
            currentIndex++;
            UpdateGlow();
        }
    }

    private void UpdateGlow()
    {
        // Turn off all glows
        foreach (var piece in puzzlePieces)
            piece.SetGlow(false);

        // Glow only the next piece in sequence
        if (currentIndex < puzzlePieces.Count)
            puzzlePieces[currentIndex].SetGlow(true);
    }
}