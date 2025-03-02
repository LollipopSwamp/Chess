using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece
{
    public char name;
    public bool isWhite;
    public int square; //numeric notation
    public List<int> legalMoves = new List<int>();

    public Piece(char _name, int _square)
    {
        name = _name;
        if (char.IsUpper(name)) { isWhite = true; }
        else { isWhite = false; }
        square = _square;
        legalMoves.Clear();
    }
    public void PrintPiece()
    {
        UnityEngine.Debug.Log("Piece Name: " + name.ToString());
        UnityEngine.Debug.Log("Is White: " + isWhite.ToString());
        UnityEngine.Debug.Log("Square: " + square.ToString());
        UnityEngine.Debug.Log("Legal Moves Count: " + legalMoves.Count.ToString());
        UnityEngine.Debug.Log("Legal Moves: " + string.Join(", ", legalMoves));
    }
}
