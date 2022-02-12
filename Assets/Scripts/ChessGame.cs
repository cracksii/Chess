using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct ChessGame
{
    public int[] board;
    private List<int>[] allowedPawnJumps;
    public int currentMove;
    public char[] allowedCastlings;
    public int enPassant;
    public int halfTurns;
    public int nextTurnNumber;

    public ChessGame(int[] _board, int _currentMove, char[] _allowedCastlings, int _enPassant, int _halfTurns, int _nextTurnNumber) 
    {
        board = _board;
        currentMove = _currentMove;
        allowedCastlings = _allowedCastlings;
        enPassant = _enPassant;
        halfTurns = _halfTurns;
        nextTurnNumber = _nextTurnNumber;
        allowedPawnJumps = new List<int>[2];

        for(int i = 0; i < 2; i++)
        {
            allowedPawnJumps[i] = new List<int>();
            for(int j = 0; j < 8; j++)
            {
                int idx = (i == 1 ? 8: 48) + j;
                //Debug.Log($"{idx} {board[idx]} {this.GetPiece(idx)}");
                if(this.GetPiece(idx) == Piece.Type.PAWN)
                    allowedPawnJumps[i].Add(idx);
            }
        }

        if(_currentMove == -1 || _halfTurns == -1 || _nextTurnNumber == -1)
            Debug.LogError("Received incorrect arguments during initialization. Most likely due to incorrect FEN string.");
        //this.Print();
    }

    public static ChessGame Load(string _fen)
    {
        return Parser.LoadFenString(_fen);
    }

    public void Print()
    {
        string b = "";
        for(int a = 0; a < 64; a++) 
        {
            if(a % 8 == 0 && a != 0)
                b += "\n";
            b += board[a] + " ";
        }
        Debug.Log(b + $"\nc: {currentMove} enPassant: {enPassant}\nht: {halfTurns} nt: {nextTurnNumber}");
    }

    public int Color(int _position)
    {
        if(board[_position] == Piece.Type.NONE)
            return Piece.Type.NONE;
        if(board[_position] > (Piece.Type.BLACK | Piece.Type.KING))
            return Piece.Type.WHITE;
        return Piece.Type.BLACK;
    }

    public int GetPiece(int _position)
    {
        return board[_position] - Color(_position);
    }

    public bool CanDoPawnJump(int _position, int _side)
    {
        if(allowedPawnJumps[(_side == Piece.Type.WHITE) ? 0: 1].Contains(_position)) 
            return true;
        return false;
    }

    public static string PosToCoordinate(int _position) 
    {
        char[] chars = new char[]{'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h'};
        return chars[_position % 8] + (8 - (_position / 8)).ToString();
    }
}
