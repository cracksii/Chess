using System;
using System.Collections.Generic;
using UnityEngine;
public static class Parser
{
    
    static Dictionary<char, int> mapping = new Dictionary<char, int>()
    {
        {'p', Piece.Type.PAWN},
        {'r', Piece.Type.ROOK},
        {'n', Piece.Type.KNIGHT},
        {'b', Piece.Type.BISHOP},
        {'q', Piece.Type.QUEEN},
        {'k', Piece.Type.KING},
    };


    public static int[] LoadPosition(string _fen)
    {
        return LoadFenString(_fen).board;
    }

    public static string BoardToFen(ChessGame _game) 
    {
        return "";
    }

    public static ChessGame LoadFenString(string _fen)
    {
        int[] board = new int[64];
        int currentMove = -1;
        int enPassant = -1;
        int halfTurns = -1;
        int nextTurnNumber = -1;
        char[] castlings = new char[4];

        int i, s = i = 0;


        for(int c = 0; c < _fen.Length; c++) 
        {
            if(_fen[c] == '/' || _fen[c] == '-')
                continue;

            if(_fen[c] == ' ') 
            {
                s++;
                continue;
            }

            switch(s)
            {
                case 0:     // board
                    char mapChar = Char.ToLower(_fen[c]);
                    if(mapping.ContainsKey(mapChar)) 
                    {
                        board[i] = Char.IsUpper(_fen[c]) ? Piece.Type.WHITE | mapping[mapChar]: Piece.Type.BLACK | mapping[mapChar];
                        i++;
                    }
                    else
                        i += (int)Char.GetNumericValue(_fen[c]);
                    break;
                case 1:     // current move
                    currentMove = _fen[c] == 'w' ? Piece.Type.WHITE: Piece.Type.BLACK;
                    i = 0;
                    break;
                case 2:     // allowed castlings
                    castlings[i] = _fen[c];
                    i++;
                    break;
                case 3:     // en passant
                    enPassant = (int)Char.GetNumericValue(_fen[c]);
                    break;
                case 4:     // half turns
                    halfTurns = (int)Char.GetNumericValue(_fen[c]);
                    break;
                case 5:     // next turn
                    nextTurnNumber = (int)Char.GetNumericValue(_fen[c]);
                    break;
                default:
                    Debug.LogError($"FEN string '{_fen}' contains {s} (more than 5) spaces");
                    break;
            }
        }
        return new ChessGame(board, currentMove, castlings, enPassant, halfTurns, nextTurnNumber);
    }

    public static void PrintBoard(int[] _board) 
    {
        string b = "";
        for(int a = 0; a < 64; a++) 
        {
            if(a % 8 == 0 && a != 0)
                b += "\n";
            b += _board[a] + " ";
        }
        Debug.Log(b);
    }
}
