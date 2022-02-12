using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager INSTANCE;
    public ChessManager chess;

    void Awake()
    {
        if(INSTANCE != null)
            Destroy(gameObject);
        else
            INSTANCE = this;
    }

    void Start()
    {
        // chess = new ChessManager("k1r5/pppr4/3Q4/7R/8/8/8/7K w KQkq - 0 1", false);
        chess = new ChessManager(_debug: false);
        UIManager.INSTANCE.GenerateFields();
        UIManager.INSTANCE.GeneratePieces(chess.board);
    }

    void Update()
    {
        if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Z))
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
    }
}
