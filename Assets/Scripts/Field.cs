using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Image))]
public class Field : MonoBehaviour, IDropHandler, IPointerClickHandler
{
    public RectTransform rectTransform;
    public int position;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if(eventData.pointerDrag != null)
            eventData.pointerDrag.GetComponent<DragDropManager>().Drop(position, gameObject);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        foreach(Piece piece in UIManager.INSTANCE.pieces)
        {
            if(piece == null)
                continue;
            if(piece.GetComponent<DragDropManager>().clicked)
            {
                piece.GetComponent<DragDropManager>().clicked = false;
                piece.GetComponent<DragDropManager>().Drop(position, gameObject);
                return;
            }
        }
    }
}
