using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager INSTANCE;
    public bool showCoordinates = false;
    [HideInInspector]
    public Field[] fields = new Field[64];
    public Piece[] pieces = new Piece[64];
    public Color32 lightColor = new Color32(255, 255, 255, 255);
    public Color32 darkColor = new Color32(0, 0, 0, 255);
    public Color32 moveColor = new Color32(130, 30, 60, 255);
    public Color32 takeColor = new Color32(170, 30, 60, 255);
    public Color32 lastMoveColor = new Color32(40, 120, 10, 255);
    public Texture2D defaultCursorTexture;
    public Texture2D hoverCursorTexture;
    public int factor = 10;
    public int xOffset, yOffset;
    public Vector3 scale = Vector3.one;

    #region Sprites
    [Header("Sprites")]
    public Sprite fieldSprite;
    public Sprite blackPawnSprite;
    public Sprite blackRookSprite;
    public Sprite blackKnightSprite;
    public Sprite blackBishopSprite;
    public Sprite blackQueenSprite;
    public Sprite blackKingSprite;

    [Space]
    public Sprite whitePawnSprite;
    public Sprite whiteRookSprite;
    public Sprite whiteKnightSprite;
    public Sprite whiteBishopSprite;
    public Sprite whiteQueenSprite;
    public Sprite whiteKingSprite;
    #endregion
    
    private Dictionary<int, Sprite> map;

    void Awake()
    {
        if(INSTANCE == null)
            INSTANCE = this;
        else
            Destroy(gameObject);
        
        map = new Dictionary<int, Sprite>()
        {
            {Piece.Type.BLACK | Piece.Type.PAWN, blackPawnSprite},
            {Piece.Type.BLACK | Piece.Type.ROOK, blackRookSprite},
            {Piece.Type.BLACK | Piece.Type.KNIGHT, blackKnightSprite},
            {Piece.Type.BLACK | Piece.Type.BISHOP, blackBishopSprite},
            {Piece.Type.BLACK | Piece.Type.QUEEN, blackQueenSprite},
            {Piece.Type.BLACK | Piece.Type.KING, blackKingSprite},
            {Piece.Type.WHITE | Piece.Type.PAWN, whitePawnSprite},
            {Piece.Type.WHITE | Piece.Type.ROOK, whiteRookSprite},
            {Piece.Type.WHITE | Piece.Type.KNIGHT, whiteKnightSprite},
            {Piece.Type.WHITE | Piece.Type.BISHOP, whiteBishopSprite},
            {Piece.Type.WHITE | Piece.Type.QUEEN, whiteQueenSprite},
            {Piece.Type.WHITE | Piece.Type.KING, whiteKingSprite},
        };
        Cursor.SetCursor(defaultCursorTexture, Vector2.zero, CursorMode.Auto);
    }

    public void GenerateFields() 
    {
        string m = "abcdefgh";
        Transform parent = GameObject.Find("Field").transform;
        for(int i = 0; i < 8; i++)
        {
            if(showCoordinates)
            {
                Text row = new GameObject(i.ToString(), typeof(Text)).GetComponent<Text>();
                row.transform.SetParent(parent);
                row.transform.localScale = Vector3.one;
                row.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
                row.text = (Mathf.Abs(8 - i)).ToString();
                row.fontSize = 50;
                row.GetComponent<RectTransform>().anchoredPosition = new Vector2(-0.25f * factor + xOffset, -i * factor + yOffset + (row.fontSize / 2));
            }

            for(int j = 0; j < 8; j++)
            {
                if(i == 7 && showCoordinates)
                {
                    Text column = new GameObject(i.ToString(), typeof(Text)).GetComponent<Text>();
                    column.transform.SetParent(parent);
                    column.transform.localScale = Vector3.one;
                    column.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
                    column.text = m[j].ToString();
                    column.fontSize = 50;
                    column.GetComponent<RectTransform>().anchoredPosition = new Vector2(j * factor + xOffset + factor - 15, -(i + 0.75f) * factor + yOffset);
                }


                int idx = i * 8 + j;

                fields[idx] = new GameObject($"field {i} {j}", typeof(Field)).GetComponent<Field>();
                fields[idx].position = idx;
                fields[idx].transform.SetParent(parent);
                fields[idx].GetComponent<RectTransform>().anchoredPosition = new Vector2(j * factor + xOffset, -i * factor + yOffset);
                fields[idx].GetComponent<RectTransform>().pivot = Vector2.zero;
                fields[idx].transform.localScale = scale;
                fields[idx].GetComponent<Image>().sprite = fieldSprite;
                fields[idx].GetComponent<Image>().color = (i + j) % 2 == 0 ? lightColor: darkColor;
            } 
        }
    }

    public void GeneratePieces(int[] _board) 
    {
        for(int i = 0; i < 64; i++) 
        {
            if(_board[i] == Piece.Type.NONE)
                continue;
            GeneratePiece(i, _board[i]);
        }
        UpdateDragState();
    }

    public void GeneratePiece(int _position, int _value)
    {
        Piece p =  new GameObject($"piece {_position}", typeof(Piece)).GetComponent<Piece>();
        p.transform.SetParent(GameObject.Find("Pieces").transform);
        p.GetComponent<Image>().sprite = map[_value];
        p.transform.localScale = scale;
        p.GetComponent<RectTransform>().anchoredPosition = fields[_position].rectTransform.anchoredPosition;
        p.GetComponent<RectTransform>().pivot = Vector2.zero;
        p.GetComponent<DragDropManager>().startPos = p.GetComponent<RectTransform>().anchoredPosition;
        p.position = _position;
        pieces[_position] = p;
    }

    void DisplayChanges() 
    {
        int i = 0;
        int j = 0;
        foreach(var field in fields) {
            field.transform.localScale = scale;
            field.GetComponent<RectTransform>().anchoredPosition = new Vector2(j * factor + xOffset, i * factor + yOffset);
            field.GetComponent<Image>().color = (i + j) % 2 == 0 ? lightColor: darkColor;

            if(j == 7)
            {
                j = 0;
                i++;
            }
            else
                j++;
        }
    }

    public void UpdateDragState()
    {
        for(int i = 0; i < pieces.Length; i++)
        {
            if(GameManager.INSTANCE.chess.debug) 
            {
                if(pieces[i] != null)
                    pieces[i].GetComponent<DragDropManager>().draggable = true;
                continue;
            }

            if(pieces[i] == null)
                continue;
            
            if(GameManager.INSTANCE.chess.game.Color(i) == GameManager.INSTANCE.chess.game.currentMove)
                pieces[i].GetComponent<DragDropManager>().draggable = true;
            else if(GameManager.INSTANCE.chess.game.Color(i) != Piece.Type.NONE)
                pieces[i].GetComponent<DragDropManager>().draggable = false;
        }
    }

    public void DrawMoves(Move[] _moves)
    {
        int i = 0;
        int j = 0;
        
        foreach(Field field in fields) {
            if(GameManager.INSTANCE.chess.moves.Count == 0 || (field.position != GameManager.INSTANCE.chess.lastMove.startPos && field.position != GameManager.INSTANCE.chess.lastMove.targetPos) || GameManager.INSTANCE.chess.lastMove.startPos == GameManager.INSTANCE.chess.lastMove.targetPos)
                field.GetComponent<Image>().color = (i + j) % 2 == 0 ? lightColor: darkColor;
            else
            {
                if(field.GetComponent<Image>().color == lightColor || field.GetComponent<Image>().color == darkColor || field.GetComponent<Image>().color == takeColor)
                    field.GetComponent<Image>().color = Color32.Lerp((i + j) % 2 == 0 ? lightColor: darkColor, lastMoveColor, 0.5f);
                else if(field.GetComponent<Image>().color != Color32.Lerp((i + j) % 2 == 0 ? lightColor: darkColor, lastMoveColor, 0.5f))
                    field.GetComponent<Image>().color = Color32.Lerp((i + j) % 2 == 0 ? lightColor: darkColor, lastMoveColor, 0.5f);
            }

            i = (j == 7) ? i + 1: i;
            j = (j == 7) ? 0: j + 1;
        }

        foreach(Move move in _moves)
        {
            Color32 c = GameManager.INSTANCE.chess.game.Color(move.targetPos) != Piece.Type.NONE ? takeColor: moveColor; 
            fields[move.targetPos].GetComponent<Image>().color = Color32.Lerp(fields[move.targetPos].GetComponent<Image>().color, c, 0.8f);
        }
    }

    public void Checkmate()
    {
        Debug.Log($"{(GameManager.INSTANCE.chess.game.currentMove == Piece.Type.WHITE ? "Black": "White")} won");
        Cursor.SetCursor(defaultCursorTexture, Vector2.zero, CursorMode.Auto);
        SoundManager.INSTANCE.Play("end");
        foreach(Piece piece in UIManager.INSTANCE.pieces)
        {
            if(piece != null)
            {
                GameObject obj = piece.gameObject;
                Destroy(piece);
                Destroy(obj.GetComponent<DragDropManager>());
            }
        }

        StartCoroutine(Reset());
    }

    private IEnumerator Reset()
    {
        yield return new WaitForSeconds(3);
        GameManager.INSTANCE.Reset();
    }
}
