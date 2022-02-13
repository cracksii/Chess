using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager INSTANCE;
    public ChessManager chess;

    void Awake()
    {
        if(INSTANCE == null)
            INSTANCE = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        SoundManager.INSTANCE.Init();
        chess = new ChessManager(_debug: true);
        chess = new ChessManager("r3k2r/8/8/3Q4/2B5/5N2/8/R3K2R w KQkq - 0 1", true);
        UIManager.INSTANCE.GenerateFields();
        UIManager.INSTANCE.GeneratePieces(chess.board);
    }

    public void Reset()
    {
        Transform parent = GameObject.Find("Field").transform;
        for(int i = 0; i < parent.childCount; i++)
            Destroy(parent.GetChild(i).gameObject);

        parent = GameObject.Find("Pieces").transform;
        for(int i = 0; i < parent.childCount; i++)
            Destroy(parent.GetChild(i).gameObject);

        UIManager.INSTANCE.fields = new Field[64];
        UIManager.INSTANCE.pieces = new Piece[64];

        chess = new ChessManager(_debug: true);
        UIManager.INSTANCE.GenerateFields();
        UIManager.INSTANCE.GeneratePieces(chess.board);
    }

    void Update()
    {
        #if UNITY_EDITOR
            KeyCode c = KeyCode.LeftShift;
        #else
            KeyCode c = KeyCode.LeftControl;
        #endif

        if(Input.GetKey(c) && Input.GetKeyDown(KeyCode.Z))
        {
            if(chess.moves.Count == 0)
                return;
            Piece piece = UIManager.INSTANCE.pieces[chess.lastMove.targetPos];
            piece.position = chess.lastMove.startPos;
            piece.GetComponent<RectTransform>().anchoredPosition = UIManager.INSTANCE.fields[chess.lastMove.startPos].rectTransform.anchoredPosition;
            UIManager.INSTANCE.pieces[chess.lastMove.startPos] = piece;
            UIManager.INSTANCE.pieces[chess.lastMove.targetPos] = null;

            if(chess.lastMove.targetValue != Piece.Type.NONE)
                UIManager.INSTANCE.GeneratePiece(chess.lastMove.targetPos, chess.lastMove.targetValue);

            chess.CancelMove(chess.lastMove, chess.lastMove.targetValue);
            chess.game.currentMove = (chess.game.currentMove == Piece.Type.WHITE) ? Piece.Type.BLACK: Piece.Type.WHITE;
            chess.moves.Remove(chess.lastMove);
            UIManager.INSTANCE.UpdateDragState();
            UIManager.INSTANCE.DrawMoves(new Move[0]);
        }
        else if(Input.GetKey(c) && Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log(Parser.BoardToFen(chess.game));
        }
    }
}
