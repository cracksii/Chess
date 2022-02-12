using System;

[System.Serializable]
public struct Move 
{
    public int startPos;
    public int targetPos;
    public int difference;
    public int absDifference;
    public int targetValue;

    public Move(int _startPos, int _targetPos)
    {
        startPos = _startPos;
        targetPos = _targetPos;
        difference = targetPos - startPos;
        absDifference = Math.Abs(difference);
        targetValue = -1;
    }

    public new string ToString()
    {
        return $"<{ChessGame.PosToCoordinate(startPos)} {ChessGame.PosToCoordinate(targetPos)} | {difference}>";
    }
}