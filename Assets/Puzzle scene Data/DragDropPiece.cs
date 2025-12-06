using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// DragDropPiece.cs
// Tác dụng: skeleton drag-drop cho UI Button.
// Lưu ý: đây là phiên bản đơn giản dùng IPointer interfaces. Sau này ta sẽ tinh chỉnh snapping vào slots.
public class DragDropPiece : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas parentCanvas;
    private CanvasGroup canvasGroup;
    private Vector3 originalPosition;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = gameObject.GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
        parentCanvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = rectTransform.anchoredPosition;
        canvasGroup.blocksRaycasts = false; // để raycast rơi vào slot
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (parentCanvas == null) return;
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentCanvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out pos);
        rectTransform.anchoredPosition = pos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        // //giải thích: hiện tại chưa snap về slot, để nguyên vị trí drop. Sau này sẽ kiểm tra slot dưới pointer.
    }

    // //giải thích: API để reset vị trí nếu cần
    public void ResetPosition()
    {
        rectTransform.anchoredPosition = originalPosition;
    }
}
