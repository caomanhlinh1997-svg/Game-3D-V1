using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class DraggablePiece : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rect;
    private Canvas canvas;
    private CanvasGroup group;

    private Vector2 originalPos;
    private DropSlot currentSlot; // slot mà piece đang nằm

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        group = gameObject.AddComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPos = rect.anchoredPosition;
        group.blocksRaycasts = false;

        // Nếu đang nằm trong slot → clear slot đó
        if (currentSlot != null)
        {
            currentSlot.Clear();
            currentSlot = null;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out pos);

        rect.anchoredPosition = pos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        group.blocksRaycasts = true;

        // Kiểm tra có slot nào bên dưới không
        var raycastResults = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);

        DropSlot foundSlot = null;

        foreach (var result in raycastResults)
        {
            foundSlot = result.gameObject.GetComponent<DropSlot>();
            if (foundSlot != null) break;
        }

        if (foundSlot != null && foundSlot.IsEmpty())
        {
            SnapIntoSlot(foundSlot);
        }
        else
        {
            // không trúng slot → trả về vị trí cũ
            rect.anchoredPosition = originalPos;
        }
    }

    private void SnapIntoSlot(DropSlot slot)
    {
        rect.SetParent(slot.transform);
        rect.anchoredPosition = Vector2.zero;

        currentSlot = slot;
        slot.AssignPiece(this);
    }
}
