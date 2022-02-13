using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class DragDropManager : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Canvas canvas;
    public RectTransform rectTransform;
    [HideInInspector]
    public CanvasGroup canvasGroup;
    public Vector3 startPos;
    public bool draggable = false;
    public bool isDragging = false;
    public bool clicked = false;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(!draggable)
            return;
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnBeginDrag(PointerEventData eventData) 
    {
        if(!draggable)
            return;
        isDragging = true;
        canvasGroup.alpha = .6f;
        canvasGroup.blocksRaycasts = false;
        startPos = rectTransform.anchoredPosition;
    }

    public void Drop(int _position, GameObject _other)
    {
        string result = GameManager.INSTANCE.chess.TryMove(new Move(GetComponent<Piece>().position, _position));
        if(result == "success" || result == "checkmate")
        {
            UIManager.INSTANCE.pieces[_position] = UIManager.INSTANCE.pieces[GetComponent<Piece>().position];
            UIManager.INSTANCE.pieces[GetComponent<Piece>().position] = null;
            UIManager.INSTANCE.UpdateDragState();
            GetComponent<Piece>().position = _position;
            rectTransform.anchoredPosition = _other.GetComponent<RectTransform>().anchoredPosition;
            startPos = rectTransform.anchoredPosition;
            Piece p;
            if(_other.TryGetComponent(out p))
                Destroy(_other);
        }
        else
        {
            UIManager.INSTANCE.DrawMoves(new Move[0]);
            rectTransform.anchoredPosition = startPos;
        }
        
        if(result == "checkmate")
            UIManager.INSTANCE.Checkmate();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
        UIManager.INSTANCE.DrawMoves(new Move[0]);

        Piece p;
        Field f;
        if(eventData.pointerCurrentRaycast.gameObject != null)
        {
            if(eventData.pointerCurrentRaycast.gameObject.TryGetComponent(out p))
                Drop(p.position, p.gameObject);
            else if(!eventData.pointerCurrentRaycast.gameObject.TryGetComponent(out f))
                rectTransform.anchoredPosition = startPos;
        }
        else
            rectTransform.anchoredPosition = startPos;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(CurrentlyDragging() || GameManager.INSTANCE.chess.game.currentMove == GameManager.INSTANCE.chess.game.Color(GetComponent<Piece>().position))
            Cursor.SetCursor(UIManager.INSTANCE.hoverCursorTexture, Vector2.zero, CursorMode.Auto);
        if(CurrentlyDragging())
            return;
        if(!draggable)
            return;
        UIManager.INSTANCE.DrawMoves(GameManager.INSTANCE.chess.GetAllMovesOfPiece(GetComponent<Piece>().position));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(isDragging || CurrentlyDragging())
            return;
        if(!draggable)
        {
            Cursor.SetCursor(UIManager.INSTANCE.defaultCursorTexture, Vector2.zero, CursorMode.Auto);
            return;
        }
        if(!clicked)
            UIManager.INSTANCE.DrawMoves(new Move[0]);
        if(!isDragging)
            Cursor.SetCursor(UIManager.INSTANCE.defaultCursorTexture, Vector2.zero, CursorMode.Auto);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        startPos = rectTransform.anchoredPosition;
        if(!CurrentlyDragging() || clicked)
        {
            if(!draggable)
                return;
            clicked = !clicked;

            if(clicked)
                UIManager.INSTANCE.DrawMoves(GameManager.INSTANCE.chess.GetAllMovesOfPiece(GetComponent<Piece>().position));
            else
                UIManager.INSTANCE.DrawMoves(new Move[0]);
        }
        else
        {
            foreach(Piece piece in UIManager.INSTANCE.pieces)
            {
                if(piece == null)
                    continue;
                if(piece.GetComponent<DragDropManager>().clicked)
                {
                    piece.GetComponent<DragDropManager>().clicked = false;
                    piece.GetComponent<DragDropManager>().Drop(GetComponent<Piece>().position, gameObject);
                    return;
                }
            }
        }
    }

    public bool CurrentlyDragging() 
    {
        foreach(Piece piece in UIManager.INSTANCE.pieces)
            if(piece != null && (piece.GetComponent<DragDropManager>().isDragging || piece.GetComponent<DragDropManager>().clicked))
                return true;
        return false;
    }
}
