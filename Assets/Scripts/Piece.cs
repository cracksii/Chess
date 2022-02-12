using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image), typeof(DragDropManager))]
[System.Serializable]
public class Piece : MonoBehaviour
{
    public int position;

    public static class Type
    {
        public const int NONE = 0b000;
        public const int PAWN = 0b001;
        public const int ROOK = 0b010;
        public const int KNIGHT = 0b011;
        public const int BISHOP = 0b100;
        public const int QUEEN = 0b101;
        public const int KING = 0b110;

        public const int BLACK = 0b01000;
        public const int WHITE = 0b10000;
    }
}
