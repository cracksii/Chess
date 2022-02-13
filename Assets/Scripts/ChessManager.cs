using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class ChessManager
{
    public ChessGame game;
    public bool debug;
    public int[] board { get => game.board; }
    public List<Move> moves;
    public Move lastMove { get => moves.Last(); }

    public ChessManager(string _fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", bool _debug=false) 
    {
        game = ChessGame.Load(_fen);
        
        #if UNITY_EDITOR
            debug = _debug;
        #else
            debug = false;
        #endif

        moves = new List<Move>();
    }

    public Move[] GetAllMovesOfColor(int _color, bool _checkCheckMate=true)
    {
        List<Move> possibleMoves = new List<Move>();
        int[] fields = board
        .Select((f, i) => i)
        .Where(f => game.Color(f) == _color)
        .ToArray();

        foreach(int field in fields) 
        {
            for(int i = 0; i < 64; i++) 
            {
                if(CheckMove(new Move(field, i), _checkCheckMate, !_checkCheckMate))
                    possibleMoves.Add(new Move(field, i));
            }
        }
        
        return possibleMoves.ToArray();
    }

    public Move[] GetAllMovesOfPiece(int _position, bool _checkCheckMate=true)
    {
        List<Move> possibleMoves = new List<Move>();
        for(int i = 0; i < 64; i++) 
        {
            if(CheckMove(new Move(_position, i), _checkCheckMate, !_checkCheckMate))
                possibleMoves.Add(new Move(_position, i));
        }
        return possibleMoves.ToArray();
    }

    public bool CheckMove(Move _move, bool _checkCheckMate=true, bool _checkCall=false, bool _finalCall=false)
    {
        int tempEnPassant = game.enPassant;
        if(debug && Input.GetKey(KeyCode.LeftShift))
            return true;

        if(_checkCall)
            game.currentMove = (game.currentMove == Piece.Type.WHITE) ? Piece.Type.BLACK: Piece.Type.WHITE;

        if(game.Color(_move.startPos) != game.currentMove || game.GetPiece(_move.startPos) == Piece.Type.NONE || game.Color(_move.targetPos) == game.currentMove)
        {
            if(_checkCall)
                game.currentMove = (game.currentMove == Piece.Type.WHITE) ? Piece.Type.BLACK: Piece.Type.WHITE;
            return false;
        }

        bool canMove = false;

        switch(game.GetPiece(_move.startPos))
        {
            case Piece.Type.PAWN:
                canMove = CheckPawn(_move, _finalCall);
                break;

            case Piece.Type.ROOK:
                canMove = CheckRook(_move);
                break;
            case Piece.Type.KNIGHT:
                canMove = CheckKnight(_move);
                break;

            case Piece.Type.BISHOP:
                canMove = CheckBishop(_move);
                break;

            case Piece.Type.QUEEN:
                canMove = CheckQueen(_move);
                break;

            case Piece.Type.KING:
                canMove = CheckKing(_move, _finalCall);
                break;
            default:
                Debug.LogError("ERROR");
                return false;
        }

        if(_checkCall)
            game.currentMove = (game.currentMove == Piece.Type.WHITE) ? Piece.Type.BLACK: Piece.Type.WHITE;

        if(!_checkCheckMate)
            return canMove;

        if(!canMove)
            return false;
        
        int value = DoMove(_move);
        bool valid = CheckForCheck(_move);
        CancelMove(_move, value);
        if(tempEnPassant == game.enPassant && _finalCall && valid)
            game.enPassant = -1;
        return valid;
    }

    private bool CheckPawn(Move _move, bool _isRealCall)
    {
        bool move = ((_move.difference == 8 && game.currentMove == Piece.Type.BLACK) || (_move.difference == -8 && game.currentMove == Piece.Type.WHITE)) && board[_move.targetPos] == Piece.Type.NONE;
    	bool jump = _move.absDifference == 16 && board[_move.startPos + (_move.difference / 2)] == Piece.Type.NONE && board[_move.targetPos] == Piece.Type.NONE && game.CanDoPawnJump(_move.startPos, game.currentMove);
        bool attack = (((_move.difference == 7 || _move.difference == 9) && game.currentMove == Piece.Type.BLACK) || ((_move.difference == -7 || _move.difference == -9) && game.currentMove == Piece.Type.WHITE)) && game.Color(_move.targetPos) != Piece.Type.NONE && Math.Abs(_move.startPos % 8 - _move.targetPos % 8) == 1;

        if(jump && _isRealCall)
            game.enPassant = _move.startPos + (_move.difference / 2);

        if(move || jump || attack)
            return true;
        return false;
    }

    private bool CheckRook(Move _move)
    {
        int step = (_move.absDifference % 8 == 0) ? (_move.difference > 0) ? 8: -8: (_move.difference > 0) ? 1: -1;
        if(_move.absDifference % 8 == 0 ||  _move.startPos / 8 == _move.targetPos / 8)
        {
            int pos = _move.startPos;
            while(pos != _move.targetPos)
            {
                if(game.GetPiece(pos += step) != Piece.Type.NONE && pos != _move.targetPos)
                    return false;
            }
            return true;
        }
        return false;
    }

    private bool CheckKnight(Move _move)
    {
        // Debug.Log($"{_move.absDifference} {_move.startPos % 8} {_move.targetPos % 8}");
        if((_move.absDifference == 15 || _move.absDifference == 6 || _move.absDifference == 10 || _move.absDifference == 17) && 
        (_move.startPos % 8 == _move.targetPos % 8 + 2 || _move.startPos % 8 == _move.targetPos % 8 - 2 || 
        _move.startPos % 8 == _move.targetPos % 8 + 1 || _move.startPos % 8 == _move.targetPos % 8 - 1))
            return true;
        return false;
    }

    private bool CheckBishop(Move _move)
    {
        if((_move.absDifference % 7 == 0 || _move.absDifference % 9 == 0) && Math.Abs(_move.startPos % 8 - _move.targetPos % 8) == Math.Abs(_move.startPos / 8 - _move.targetPos / 8))
        {
            int step = (_move.absDifference % 7 == 0 && _move.startPos != 0 && _move.startPos != 63) ? (_move.difference > 0) ? 7: -7: (_move.difference > 0) ? 9: -9;
            int pos = _move.startPos;
            while(pos != _move.targetPos)
            {
                if(game.GetPiece(pos += step) != Piece.Type.NONE && pos != _move.targetPos)
                    return false;
            }
            return true;
        }
        return false;
    }

    private bool CheckQueen(Move _move)
    {
        if(CheckBishop(_move) || CheckRook(_move))
            return true;
        return false;
    }

    private bool CheckKing(Move _move, bool _isRealCall)
    {
        int xDistance = Math.Abs(_move.startPos % 8 - _move.targetPos % 8);
        int yDistance = Math.Abs(_move.startPos / 8 - _move.targetPos / 8);
        if(xDistance <= 1 && yDistance <= 1)
        {
            if(_isRealCall) 
                game.allowedCastlings = string.Join("", game.allowedCastlings).Replace(game.currentMove == Piece.Type.WHITE ? "K": "k", "").Replace(game.currentMove == Piece.Type.WHITE ? "Q": "q", "").ToCharArray();
            return true;
        }

        return false;
    }

    private bool CheckForCheck(Move _move)
    {
        int prevColor = game.currentMove;
        int checkColor = (game.currentMove == Piece.Type.WHITE) ? Piece.Type.BLACK: Piece.Type.WHITE;

        //Debug.Log($"{prevColor} {checkColor}");
        var moves = GetAllMovesOfColor(checkColor, false)
        .Where((i) => (board[i.targetPos] == (prevColor | Piece.Type.KING)))
        .ToArray();

        List<Move> possibleMoves = new List<Move>();

        foreach(Move m in GetAllMovesOfColor(checkColor, false))
        {
            if(board[m.targetPos] != (prevColor | Piece.Type.KING))
                possibleMoves.Add(m);
        }

        UIManager.INSTANCE.DrawMoves(new Move[0]);

        if(possibleMoves.Count == 0)
            Debug.Log("Game is over");
        return moves.Length == 0;
    }

    private int DoMove(Move _move)
    {
        int targetValue = game.board[_move.targetPos];
        game.board[_move.targetPos] = game.board[_move.startPos];
        game.board[_move.startPos] = Piece.Type.NONE;
        game.halfTurns++;
        return targetValue;
    }

    public void CancelMove(Move _move, int _targetValue)
    {
        game.board[_move.startPos] = game.board[_move.targetPos];
        game.board[_move.targetPos] = _targetValue;
        if(_targetValue == Piece.Type.NONE)
            game.halfTurns--;
    }

    private void CheckForPawnPromotion()
    {
        for(int i = 0; i < 64; i++)
        {
            if(board[i] == Piece.Type.NONE)
                continue;
            if(board[i] == (game.currentMove | Piece.Type.PAWN) && ((game.currentMove == Piece.Type.WHITE && board[i] / 8 == 0) || (game.currentMove == Piece.Type.BLACK && board[i] / 8 == 7)))
            {
                Debug.Log("PROMOTION");
                board[i] = game.currentMove | Piece.Type.QUEEN;
            }
        }
    }

    public string TryMove(Move _move)
    {
        Cursor.SetCursor(UIManager.INSTANCE.defaultCursorTexture, Vector2.zero, CursorMode.Auto);
        if(!CheckMove(_move, _finalCall: true))
            return "error";

        _move.targetValue = DoMove(_move);
        CheckForPawnPromotion();

        game.currentMove = (game.currentMove == Piece.Type.WHITE) ? Piece.Type.BLACK: Piece.Type.WHITE;
        moves.Add(_move);
        if(lastMove.targetValue == Piece.Type.NONE)
            SoundManager.INSTANCE.Play("move");
        else
            SoundManager.INSTANCE.Play("capture");

        if(GetAllMovesOfColor(game.currentMove, true).Length == 0) 
            return "checkmate";
        return "success";
    }
}
