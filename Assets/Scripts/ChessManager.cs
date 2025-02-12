using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using System.Diagnostics;

public class ChessManager : MonoBehaviour
{
    //private Process stockfishProcess;
    public GameObject notationGrid;

    public string position = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR";
    public string currTurn = "w";
    public string castlingRights = "KQkq";
    public string enPassant = "-";
    public int fiftyMoveRule = 0;
    public int moves = 0;
    public Dictionary<int, Tile> tiles = new Dictionary<int, Tile>();
    public string FEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 2";
    private string allPieceChars = "rnbqkpRNBQKP";

    public int whiteKingLoc;
    public int blackKingLoc;

    public bool isWhitesTurn;

    public List<string> notation = new List<string>();


    public void ResetGame()
    {
        foreach (var tile in tiles.Values)
        {
            tile.pieceName = '-';
            tile.SetPiece('-');
        }
        position = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR";
        SetStartPosition();
    }
    public void SetStartPosition()
    {
        //FEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 2";
        //Debug.Log(position);
        SetPosition("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR");
        SetAllLegalMoves();
        //Debug.Log("White in Check: " + IsInCheck(true).ToString());
        //Debug.Log("Black in Check: " + IsInCheck(false).ToString());
        isWhitesTurn = true;
    }
    public void SetPosition(string _position)
    {
        int tileInt = 0;
        foreach (char c in _position)
        {
            if (allPieceChars.Contains(c))
            {
                tiles[IntToTileNum(tileInt)].GetComponent<Tile>().SetPiece(c);
                if (c == 'K'){whiteKingLoc = IntToTileNum(tileInt); }
                else if (c == 'k') {blackKingLoc = IntToTileNum(tileInt); }
                tileInt++;
            }
            else if(c == '/'){}
            else
            {
                tileInt += int.Parse(c.ToString());
            }
        }
    }
    private int IntToTileNum(int _tileNum)
    {
        //0
        int column = (_tileNum + 1) % 8;
        if (column == 0) { column = 8; }

        int row = (int)Mathf.Floor((63- _tileNum) / 8) + 1;

        return column*10 + row;
    }
    public bool MoveIsLegal(int startSquare, int endSquare)
    {
        //Debug.Log("Checking Legal Move: " + startSquare.ToString() + ", " + endSquare.ToString());

        //check if own piece is on square
        //Debug.Log(tiles[startSquare].GetComponent<Tile>().piece.GetComponent<Piece>().color);
        //Debug.Log(tiles[endSquare].GetComponent<Tile>().piece.GetComponent<Piece>().color);
        if (tiles[startSquare].GetComponent<Tile>().piece.GetComponent<Piece>().color == tiles[endSquare].GetComponent<Tile>().piece.GetComponent<Piece>().color) { return false; }

        //check piece type legality
        char pieceType = char.ToLower(tiles[startSquare].pieceName);
        switch (pieceType)
        {
            case 'p':
                if (PawnMovement(startSquare, endSquare)) { return true; }
                break;
            case 'b':
                if (!SquareBlocked(startSquare, endSquare) && IsDiagonal(startSquare, endSquare)) { return true; }
                break;
            case 'n':
                if (KnightMovement(startSquare, endSquare)) { return true; }
                break;
            case 'r':
                if (!SquareBlocked(startSquare, endSquare) && IsLateral(startSquare, endSquare)) { return true; }
                break;
            case 'k':
                if (KingMovement(startSquare, endSquare)) { return true; }
                break;
            case 'q':
                if (!SquareBlocked(startSquare, endSquare) && (IsLateral(startSquare, endSquare) || IsDiagonal(startSquare, endSquare))) { return true; }
                break;
        }
        return false;
    }
    private bool PawnMovement(int startSquare, int endSquare)
    {
        if (tiles[startSquare].GetComponent<Tile>().piece.GetComponent<Piece>().color == 0)
        {
            //normal movement
            if (endSquare - startSquare == 1 && tiles[endSquare].GetComponent<Tile>().pieceName == '-') { return true; }
            else if (startSquare % 10 == 2 && endSquare - startSquare == 2){ return true; }
            //take piece
            else if ((Mathf.Abs(endSquare - startSquare) == 11 | Mathf.Abs(startSquare - endSquare) == 9) && tiles[endSquare].pieceName != '-' && tiles[endSquare].piece.GetComponent<Piece>().color == 1) { return true; }
        }
        else //isBlack
        {
            //normal movement
            if (endSquare - startSquare == -1 && tiles[endSquare].GetComponent<Tile>().pieceName == '-') { return true; }
            else if (startSquare % 10 == 7 && endSquare - startSquare == -2) { return true; }
            //take piece
            else if ((Mathf.Abs(startSquare - endSquare) == 11 | Mathf.Abs(endSquare - startSquare) == 9) && tiles[endSquare].pieceName != '-' && tiles[endSquare].piece.GetComponent<Piece>().color == 0) { return true; }
        }

        return false;
    }
    private bool IsDiagonal(int startSquare, int endSquare)
    {
        //check if diagonal
        bool legal = false;
        int squareDiff = Mathf.Abs(startSquare - endSquare);
        int positiveNegative = startSquare - endSquare < 0 ? 1 : -1;
        int squareBlockDiff;
        if (tiles[startSquare].GetComponent<Tile>().isOffset != tiles[endSquare].GetComponent<Tile>().isOffset)
        {
            return false;
        }
        else if (squareDiff % 9 == 0) 
        {
            squareBlockDiff = 9;
            legal = true; 
        }
        else if(squareDiff % 11 == 0)
        {
            squareBlockDiff = 11;
            legal = true;
        }
        else { return false; }
        //check if blocked
        int squareToCheck = startSquare + (squareBlockDiff * positiveNegative);
        while (squareToCheck != endSquare)
        {
            //Debug.Log("Checking if " + squareToCheck.ToString() + " is blocked");
            if (tiles[squareToCheck].GetComponent<Tile>().pieceName != '-') { return false; }
            squareToCheck += squareBlockDiff * positiveNegative;
        }
        return legal;
    }
    private bool IsLateral(int startSquare, int endSquare)
    {
        bool legal = false;
        //check if lateral
        int squareDiff = Mathf.Abs(startSquare - endSquare);
        int positiveNegative = startSquare - endSquare < 0 ? 1 : -1;
        int squareBlockDiff;
        //up down
        if (Mathf.Floor(startSquare / 10) == Mathf.Floor(endSquare / 10)) 
        { 
            legal = true;
            squareBlockDiff = 1;
        }
        //left right
        else if (startSquare % 10 == endSquare % 10)
        {
            legal = true;
            squareBlockDiff = 10;
        }
        else { return false; }
        //check if blocked
        int squareToCheck = startSquare + (squareBlockDiff * positiveNegative);
        while (squareToCheck != endSquare)
        {
            //Debug.Log("Checking if " + squareToCheck.ToString() + " is blocked (11)");
            if (tiles[squareToCheck].GetComponent<Tile>().pieceName != '-') { return false; }
            squareToCheck += squareBlockDiff * positiveNegative;
        }
        return legal;
    }
    private bool KnightMovement(int startSquare, int endSquare)
    {
        if(Mathf.Abs(startSquare - endSquare) == 8 | Mathf.Abs(startSquare - endSquare) == 12 | Mathf.Abs(startSquare - endSquare) == 19 | Mathf.Abs(startSquare - endSquare) == 21) { return true; }
        return false;
    }
    private bool KingMovement(int startSquare, int endSquare)
    {
        if ((9 <= Mathf.Abs(startSquare - endSquare) && Mathf.Abs(startSquare - endSquare) <= 11) | Mathf.Abs(startSquare - endSquare) == 1) { return true; }
        return false;
    }
    private bool SquareBlocked(int startSquare, int endSquare)
    {
        return false;
    }

    public bool IsInCheck(bool isWhite)
    {
        int color = isWhite ? 0 : 1;
        //Debug.Log("whiteKingLoc:" + whiteKingLoc.ToString());
        //Debug.Log("blackKingLoc:" + blackKingLoc.ToString());
        foreach (var tile in tiles)
        {
            Piece piece = tile.Value.piece.GetComponent<Piece>();
            if (tile.Value.pieceName == '-') { continue; }
            else if (isWhite && piece.color != color && piece.legalMoves.Contains(whiteKingLoc))
            {
                //Debug.Log("White in check from " + tile.Value.squareName.ToString());
                return true;
            }
            else if (!isWhite && piece.color != color && piece.legalMoves.Contains(blackKingLoc))
            {
                //Debug.Log("Black in check from " + tile.Value.squareName.ToString());
                return true;
            }
        }
        return false;
    }
    public void SetAllLegalMoves()
    {
        foreach (var startSquare in tiles)
        {
            startSquare.Value.piece.GetComponent<Piece>().legalMoves = new List<int>();
            if(startSquare.Value.pieceName != '-')
            {
                foreach(KeyValuePair<int, Tile> endSquare in tiles)
                {
                    //Debug.Log("Checking Legal Move: " + startSquare.Value.squareName.ToString() + " " + endSquare.Value.squareName.ToString());
                    if (MoveIsLegal(startSquare.Value.squareName, endSquare.Value.squareName))
                    {
                        //Debug.Log("Legal move");
                        startSquare.Value.piece.GetComponent<Piece>().legalMoves.Add(endSquare.Value.squareName);
                    }
                    //else { Debug.Log("Illegal Move: " + startSquare.Value.squareName.ToString() + " " + endSquare.Value.squareName.ToString()); }
                }
            }
        }
    }
    public void AddMoveToNotation(int _startSquare, int _endSquare, bool _pieceTaken)
    {
        string algebraicStartSquare = tiles[_startSquare].algebraicSquareName;
        string algebraicEndSquare = tiles[_endSquare].algebraicSquareName;
        string notationToAdd = "";

        //check if ambiguous move
        bool ambiguousMove = false;
        foreach (var tile in tiles)
        {
            if (tile.Value.squareName != _startSquare && tile.Value.pieceName == tiles[_startSquare].pieceName && tile.Value.piece.GetComponent<Piece>().legalMoves.Contains(_endSquare))
            {
                ambiguousMove = true;
            }
        }

        //piece name / pawn file
        if (char.ToUpper(tiles[_startSquare].pieceName) != 'P'){notationToAdd += char.ToUpper(tiles[_startSquare].pieceName).ToString(); }
        else if (_pieceTaken) { notationToAdd += tiles[_startSquare].algebraicSquareName[0].ToString(); }
        //ambiguous move
        if (ambiguousMove && char.ToUpper(tiles[_startSquare].pieceName) != 'P') { notationToAdd += tiles[_startSquare].algebraicSquareName[0].ToString(); }
        //piece taken
        if (_pieceTaken){notationToAdd += "x";}
        //end square
        notationToAdd += algebraicEndSquare;
        //check, FIX THIS, this is processed before the move, so can't see if the opponent is in check

        //add to list
        notation.Add(notationToAdd);


        foreach (string move in notation) { Debug.Log(move); }  
    }

    public void AddCheckToNotation()
    {
        notation[notation.Count - 1] = notation[notation.Count - 1] + "+";
    }
    public void UpdateNotationGrid()
    {
        Debug.Log(notation.Count);
        Debug.Log(notation.Count % 2);
        //odd count notation, newest move is white
        if (notation.Count % 2 == 1)
        {
            notationGrid.GetComponent<NotationGrid>().AddWhiteMove(notation[notation.Count - 1]);
        }
        else
        {
            notationGrid.GetComponent<NotationGrid>().AddBlackMove(notation[notation.Count - 1]);
        }
    }
    //set FEN
    //integrate stockfish

}
