using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FEN
{
    public string position = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR";
    public string currTurn = "w";
    public string castlingRights = "KQkq";
    public bool whiteKingsideCastling = true;
    public bool whiteQueensideCastling = true;
    public bool blackKingsideCastling = true;
    public bool blackQueensideCastling = true;
    public string enPassant = "-";
    public int fiftyMoveRule = 0;
    public int moves = 0;

    public FEN() { }
    public FEN(string _fen) 
    {
        //position
        int startIndex = 0;
        int count = _fen.IndexOf(' ');
        position = _fen.Substring(startIndex, count);
        UnityEngine.Debug.Log(position);

        //currturn
        startIndex = _fen.IndexOf(' ', startIndex) + 1;
        count = _fen.IndexOf(' ', startIndex) - startIndex;
        currTurn = _fen.Substring(startIndex, count);

        //castling rights
        startIndex = _fen.IndexOf(' ', startIndex) + 1;
        count = _fen.IndexOf(' ', startIndex) - startIndex;
        castlingRights = _fen.Substring(startIndex, count);

        whiteKingsideCastling = false;
        whiteQueensideCastling = false;
        blackKingsideCastling = false;
        blackQueensideCastling = false;
        if (castlingRights.Contains('K')){
            whiteKingsideCastling = true;
        }
        if (castlingRights.Contains('Q')){
            whiteQueensideCastling = true;
        }
        if (castlingRights.Contains('k')){
            blackKingsideCastling = true;
        }
        if (castlingRights.Contains('q')){
            blackQueensideCastling = true;
        }
        //enPassant
        startIndex = _fen.IndexOf(' ', startIndex) + 1;
        count = _fen.IndexOf(' ', startIndex) - startIndex;
        enPassant = _fen.Substring(startIndex, count);

        //fifty move rule
        startIndex = _fen.IndexOf(' ', startIndex) + 1;
        count = _fen.IndexOf(' ', startIndex) - startIndex;
        fiftyMoveRule = int.Parse(_fen.Substring(startIndex, count));

        //moves
        startIndex = _fen.IndexOf(' ', startIndex) + 1;
        moves = int.Parse(_fen.Substring(startIndex));

        PrintFEN();
    }

    public void UpdatePosition(Dictionary<int, Tile> _tiles)
    {
        int emptySquares = 0;
        string positionFEN = "";
        //i is row, j is column
        for (int i = 8; i > 0; i--)
        {
            for (int j = 1; j < 9; j++)
            {
                int squareNum = (j * 10) + i;
                if (_tiles[squareNum].piece.name == '-')
                {
                    emptySquares++;
                    if (j == 8)
                    {
                        positionFEN += emptySquares.ToString();
                        emptySquares = 0;
                    }
                }
                else if (emptySquares > 0)
                {
                    positionFEN += emptySquares.ToString();
                    positionFEN += _tiles[squareNum].piece.name;
                    emptySquares = 0;
                }
                else
                {
                    positionFEN += _tiles[squareNum].piece.name;
                }
            }
            if (i > 1)
            {
                positionFEN += "/";
            }
            emptySquares = 0;
        }
        position = positionFEN;
    }

    public string GetFEN()
    {
        string fen = "";
        fen = position + " ";
        fen += currTurn + " ";
        fen += GetCastlingRights() + " ";
        fen += enPassant + " ";
        fen += fiftyMoveRule.ToString() + " ";
        fen += moves.ToString();
        return fen;
    }
    private string GetCastlingRights()
    {
        castlingRights = "";
        if(!whiteKingsideCastling && !whiteQueensideCastling && !blackKingsideCastling && !blackQueensideCastling)
        {
            castlingRights = "-";
        }
        if (whiteKingsideCastling)
        {
            castlingRights += "K";
        }
        if (whiteQueensideCastling)
        {
            castlingRights += "Q";
        }
        if (blackKingsideCastling)
        {
            castlingRights += "k";
        }
        if (blackQueensideCastling)
        {
            castlingRights += "q";
        }
        return castlingRights;
    }
    public void PrintFEN()
    {
        UnityEngine.Debug.Log("Current FEN: " + GetFEN());
    }
}
