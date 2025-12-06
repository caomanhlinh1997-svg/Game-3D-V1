using UnityEngine;

public class DropSlot : MonoBehaviour
{
    [HideInInspector] public DraggablePiece currentPiece;

    // Cho biết slot này có đang trống không
    public bool IsEmpty()
    {
        return currentPiece == null;
    }

    // Gắn piece vào slot
    public void AssignPiece(DraggablePiece piece)
    {
        currentPiece = piece;
    }

    // Bỏ piece ra khỏi slot
    public void Clear()
    {
        currentPiece = null;
    }
}
