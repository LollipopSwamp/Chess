using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using System.Diagnostics;

public class ChessManager : MonoBehaviour
{
    //gameobjects
    //public GameObject gridManagerObj;
    //public GridManager gridManager;

    //FEN variables
    public string position = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR";
    public string currTurn = "w";
    public string castlingRights = "KQkq";
    public string enPassant = "-";
    public int fiftyMoveRule = 0;
    public int moves = 0;
    public FEN currFEN = new FEN();
    public FEN tempFEN = new FEN();
    public FEN customFEN = new FEN();
    public Dictionary<int, FEN> boardStates = new Dictionary<int, FEN>();

    //tiles variables
    public Dictionary<int, Tile> tiles = new Dictionary<int, Tile>();
    public Dictionary<int, Tile> tilesAfterMove = new Dictionary<int, Tile>();
    public string FEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 2";
    private string allPieceChars = "rnbqkpRNBQKP";

    //game variables
    public int whiteKingLoc;
    public int blackKingLoc;
    public bool isWhitesTurn;

    //notation
    public GameObject notationGrid;
    public List<string> notation = new List<string>();

    void Start()
    {
        //gridManager = gridManagerObj.GetComponent<GridManager>();
    }

    public void ResetGame()
    {
        foreach (var tile in tiles.Values)
        {
            tile.piece.name = '-';
            tile.SetPiece('-');
        }
        position = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR";
        SetStartPosition();
    }
    public void SetStartPosition()
    {
        //FEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 2";
        //Debug.Log(position);
        StartGame(new FEN());
        SetAllLegalMoves(tiles);
        //Debug.Log("White in Check: " + IsInCheck(true).ToString());
        //Debug.Log("Black in Check: " + IsInCheck(false).ToString());
        isWhitesTurn = true;
        FEN startFEN = new FEN();
    }
    public void SetCustomFEN(string _currTurn)
    {
        customFEN = new FEN();
        customFEN.position = PositionToFENPostion(tiles);
        customFEN.currTurn = _currTurn;
    }
    public void StartGame(FEN _fen)
{
        if(_fen.currTurn == "w") { isWhitesTurn=true;}
        else { isWhitesTurn=false;}
        int tileInt = 0;
        foreach (char c in _fen.position)
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
        SetAllLegalMoves(tiles);
    }
    private int IntToTileNum(int _tileNum)
    {
        //0
        int column = (_tileNum + 1) % 8;
        if (column == 0) { column = 8; }

        int row = (int)Mathf.Floor((63- _tileNum) / 8) + 1;

        return column*10 + row;
    }
    public void SetTilesAfterMove(int startSquare, int endSquare)
    {
        Debug.Log("SetTilesAfterMove");
        //create board after move
        //tilesAfterMove = new Dictionary<int, Tile>();
        tilesAfterMove = new Dictionary<int, Tile>();
        foreach(var tile in tiles)
        {
            Tile newTile = new Tile(tile.Value.squareName);
            newTile.piece = new Piece(tile.Value.piece.name, tile.Value.squareName);
            tilesAfterMove.Add(tile.Key,  newTile);
        }
        //Debug.Log(tiles[startSquare].piece.name);
        tilesAfterMove[endSquare].piece.name = tilesAfterMove[startSquare].piece.name;
        tilesAfterMove[startSquare].piece.name = '-';
        //Debug.Log(tiles[startSquare].piece.name);
        tilesAfterMove = SetAllLegalMoves(tilesAfterMove);
        tilesAfterMove[61].piece.PrintPiece();
        Debug.Log(PositionToFENPostion(tilesAfterMove));
    }
    public bool MoveIsLegal(int _startSquareName, int _endSquareName)
    {
        Tile startSquare = tiles[_startSquareName].GetComponent<Tile>();
        Tile endSquare = tiles[_endSquareName].GetComponent<Tile>();
        //check if own piece is on square
        //Debug.Log("startSquare");
        //startSquare.piece.PrintPiece();
        //Debug.Log("endSquare: " + _endSquareName.ToString());
        //endSquare.piece.PrintPiece();
        if (startSquare.piece.isWhite == endSquare.piece.isWhite && endSquare.piece.name != '-') { return false; }

        //check piece type legality
        char pieceType = char.ToLower(startSquare.piece.name);
        bool legal = false;
        switch (pieceType)
        {
            case 'p':
                if (PawnMovement(_startSquareName, _endSquareName)) { legal = true; }
                break;
            case 'b':
                if (!SquareBlocked(_startSquareName, _endSquareName) && IsDiagonal(_startSquareName, _endSquareName)) { legal = true; }
                break;
            case 'n':
                if (KnightMovement(_startSquareName, _endSquareName)) { legal = true; }
                break;
            case 'r':
                if (!SquareBlocked(_startSquareName, _endSquareName) && IsLateral(_startSquareName, _endSquareName)) { legal = true; }
                break;
            case 'k':
                if (KingMovement(_startSquareName, _endSquareName)) { legal = true; }
                break;
            case 'q':
                if (!SquareBlocked(_startSquareName, _endSquareName) && (IsLateral(_startSquareName, _endSquareName) || IsDiagonal(_startSquareName, _endSquareName))) { legal = true; }
                break;
        }

        return legal;
    }
    public bool IsInCheckAfterMove(bool isWhite)
    {
        Debug.Log("IsInCheckAfterMove");
        //check all tiles
        foreach (var tile in tilesAfterMove)
        {
            Piece piece = tile.Value.piece;
            //if empty square
            if (tile.Value.piece.name == '-') { continue; }
            //if checking white, piece is opposite color, and piece contains kingLoc in legal moves
            else if (isWhite && piece.isWhite != isWhite && piece.legalMoves.Contains(whiteKingLoc))
            {
                Debug.Log(tile.Value.squareName);
                Debug.Log(true);
                return true;
            }
            //if checking black, piece is opposite color, and piece contains kingLoc in legal moves
            else if (!isWhite && piece.isWhite != isWhite && piece.legalMoves.Contains(blackKingLoc))
            {
                Debug.Log(tile.Value.squareName);
                Debug.Log(true);
                return true;
            }
        }
        return false;
    }
    private bool PawnMovement(int startSquare, int endSquare)
    {
        if (tiles[startSquare].GetComponent<Tile>().piece.isWhite)
        {
            //normal movement
            if (endSquare - startSquare == 1 && tiles[endSquare].GetComponent<Tile>().piece.name == '-') { return true; }
            //2 moves forward
            else if (startSquare % 10 == 2 && endSquare - startSquare == 2 && tiles[endSquare].GetComponent<Tile>().piece.name == '-') {
                //tempFEN.enPassant = tiles[endSquare].algebraicSquareName - 1;
                return true; 
            }
            //take piece
            else if ((endSquare - startSquare == 11 | startSquare - endSquare == 9) 
                && ((tiles[endSquare].piece.name != '-' && !tiles[endSquare].piece.isWhite) | (tiles[endSquare].algebraicSquareName == currFEN.enPassant && endSquare % 10 == 6)))
            {
                return true; 
            }
        }
        else //isBlack
        {
            //normal movement
            if (endSquare - startSquare == -1 && tiles[endSquare].GetComponent<Tile>().piece.name == '-') { return true; }
            //2 moves forward
            else if (startSquare % 10 == 7 && endSquare - startSquare == -2 && tiles[endSquare].GetComponent<Tile>().piece.name == '-') 
            { 
                //tempFEN.enPassant = tiles[endSquare].algebraicSquareName + 1;
                return true;
            }
            //take piece
            else if ((startSquare - endSquare == 11 | endSquare - startSquare == 9) 
                && ((tiles[endSquare].piece.name != '-' && tiles[endSquare].piece.isWhite) | (tiles[endSquare].algebraicSquareName == currFEN.enPassant && endSquare % 10 == 3 )))
            {
                return true;
            }
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
            if (tiles[squareToCheck].GetComponent<Tile>().piece.name != '-') { return false; }
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
            if (tiles[squareToCheck].GetComponent<Tile>().piece.name != '-') { return false; }
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
        //normal movement
        if ((9 <= Mathf.Abs(startSquare - endSquare) && Mathf.Abs(startSquare - endSquare) <= 11) | Mathf.Abs(startSquare - endSquare) == 1) { return true; }
        //castling white kingside
        if (startSquare == 51 && endSquare == 71 && currFEN.whiteKingsideCastling && tiles[61].piece.name == '-' && tiles[71].piece.name == '-' && !CastleThroughCheck(61,true))
        {
            return true;
        }
        //castling white queenside
        if (startSquare == 51 && endSquare == 31 && currFEN.whiteQueensideCastling && tiles[41].piece.name == '-' && tiles[31].piece.name == '-' && tiles[21].piece.name == '-' && !CastleThroughCheck(41, true))
        {
            return true;
        }
        //castling white kingside
        if (startSquare == 58 && endSquare == 78 && currFEN.blackKingsideCastling && tiles[68].piece.name == '-' && tiles[78].piece.name == '-' && !CastleThroughCheck(68, false))
        {
            return true;
        }
        //castling white kingside
        if (startSquare == 58 && endSquare == 38 && currFEN.blackQueensideCastling && tiles[48].piece.name == '-' && tiles[38].piece.name == '-' && tiles[28].piece.name == '-' && !CastleThroughCheck(48, false))
        {
            return true;
        }
        return false;
    }
    private bool CastleThroughCheck(int _squareName, bool _isWhite)
    {
        Debug.Log("CastleThroughCheck");
        //check all tiles
        foreach (var tile in tiles)
        {
            Piece piece = tile.Value.piece;
            if (piece.name != '-' && piece.isWhite != _isWhite && piece.legalMoves.Contains(_squareName))
            {
                Debug.Log("Cannot castle through check bc of " + piece.name + " on " + tile.Value.algebraicSquareName);
                return true;
            }
        }
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
            Piece piece = tile.Value.piece;
            if (tile.Value.piece.name == '-') { continue; }
            else if (isWhite && piece.isWhite != isWhite && piece.legalMoves.Contains(whiteKingLoc))
            {
                //Debug.Log("White in check from " + tile.Value.squareName.ToString());
                return true;
            }
            else if (!isWhite && piece.isWhite != isWhite && piece.legalMoves.Contains(blackKingLoc))
            {
                //Debug.Log("Black in check from " + tile.Value.squareName.ToString());
                return true;
            }
        }
        return false;
    }
    public Dictionary<int, Tile> SetAllLegalMoves(Dictionary<int, Tile> _tiles)
    {
        foreach (var startSquare in _tiles)
        {
            //startSquare.Value.PrintTile();
            startSquare.Value.piece.legalMoves = new List<int>();
            if(startSquare.Value.piece.name != '-')
            {
                foreach(KeyValuePair<int, Tile> endSquare in _tiles)
                {
                    //Debug.Log("Checking Legal Move: " + startSquare.Value.squareName.ToString() + " " + endSquare.Value.squareName.ToString());
                    if (MoveIsLegal(startSquare.Value.squareName, endSquare.Value.squareName))
                    {
                        //Debug.Log("Legal move");
                        startSquare.Value.piece.legalMoves.Add(endSquare.Value.squareName);
                    }
                    //else { Debug.Log("Illegal Move: " + startSquare.Value.squareName.ToString() + " " + endSquare.Value.squareName.ToString()); }
                }
            }
        }
        return _tiles;
    }

    public bool IsCheckmate() //checked before currTurn is updated, but after piece is moved
    {
        bool checkmate = true;
        //move each pieces legal move, then check if in check
        int currKingLoc = !isWhitesTurn ? whiteKingLoc : blackKingLoc;
        foreach(var tile in tiles)
        {
            //if tile is not empty and piece color matches, check moves
            Piece piece = tile.Value.piece;
            if(piece.name != '-' && piece.isWhite != isWhitesTurn)
            {
                foreach(int legalMove in piece.legalMoves)
                {
                    //prep undo move
                    char endSquarePiece = tiles[legalMove].piece.name;

                    //make move
                    tiles[legalMove].SetPiece(piece.name);
                    tile.Value.SetPiece('-');
                    tiles = SetAllLegalMoves(tiles);
                    //update kingLoc's
                    if (piece.name == 'K')
                    {
                        whiteKingLoc = legalMove;
                    }
                    else if (piece.name == 'k')
                    {
                        blackKingLoc = legalMove;
                    }

                    //if move removes check
                    if (!IsInCheck(!isWhitesTurn))
                    {
                        Debug.Log(tile.Value.squareName.ToString() + " to " + legalMove.ToString() + " removes check. Not checkmate.");
                        checkmate = false;
                    }
                    //undo move
                    tile.Value.SetPiece(tiles[legalMove].piece.name);
                    tiles[legalMove].SetPiece(endSquarePiece);
                    tiles = SetAllLegalMoves(tiles);

                    //break if checkmate
                    if (!checkmate) { return checkmate; }
                }
            }
        }
        Debug.Log("Checkmate!!");
        return checkmate;
    }
    public void GameEnded(bool _whiteIsWinner)
    {

    }

    public void AddMoveToNotation(int _startSquare, int _endSquare, bool _pieceTaken, bool _ambiguousMove, int _castleType) //processed after move
    {
        string algebraicStartSquare = tiles[_startSquare].algebraicSquareName;
        string algebraicEndSquare = tiles[_endSquare].algebraicSquareName;
        string notationToAdd = "";
        //castling
        if(_castleType == 1) { 
            notationToAdd = "O-O";
            notation.Add(notationToAdd);
            return;
        }
        else if(_castleType == 2) { 
            notationToAdd = "O-O-O";
            notation.Add(notationToAdd);
            return;
        }
        //piece name / pawn file
        if (char.ToUpper(tiles[_endSquare].piece.name) != 'P'){notationToAdd += char.ToUpper(tiles[_endSquare].piece.name).ToString(); }
        else if (_pieceTaken) { notationToAdd += tiles[_startSquare].algebraicSquareName[0].ToString(); }
        //ambiguous move
        if (_ambiguousMove && char.ToUpper(tiles[_endSquare].piece.name) != 'P') { notationToAdd += tiles[_startSquare].algebraicSquareName[0].ToString(); }
        //piece taken
        if (_pieceTaken){notationToAdd += "x";}
        //end square
        notationToAdd += algebraicEndSquare;

        //add to list
        notation.Add(notationToAdd);

    }

    public void AddToNotation(string _checkChar)
    {
        notation[notation.Count - 1] = notation[notation.Count - 1] + _checkChar;
    }
    public void UpdateNotationGrid()
    {
        //update tempFEN and currFEN
        if(isWhitesTurn)
        {
            tempFEN.currTurn = "w";
        }
        else
        {
            tempFEN.currTurn = "b";
            tempFEN.moves += 1;
        }
        tempFEN.UpdatePosition(tiles);
        currFEN = tempFEN;

        //odd count notation, newest move is white
        if (notation.Count % 2 == 1)
        {
            notationGrid.GetComponent<NotationGrid>().AddWhiteMove(notation[notation.Count - 1], currFEN);
        }
        else
        {
            notationGrid.GetComponent<NotationGrid>().AddBlackMove(notation[notation.Count - 1], currFEN);
        }
    }
    //set FEN
    //integrate stockfish
    public void PrintCurrentFENPosition()
    {
        currFEN.PrintFEN();
    }
    public string PositionToFENPostion(Dictionary <int, Tile> _tiles)
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
                    if(j == 8)
                    {
                        positionFEN += emptySquares.ToString();
                        emptySquares = 0;
                    }
                }
                else if(emptySquares > 0)
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
        return positionFEN;
    }
}
