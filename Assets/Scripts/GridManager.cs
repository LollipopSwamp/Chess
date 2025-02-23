using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int width, height;
    [SerializeField] private Tile tilePrefab;
    [SerializeField] private Transform camera;
    public GameObject chessManagerObj;
    private ChessManager chessManager;
    public GameObject notationGrid;
    public int highlightedTile;
    public int clickedTile;

    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        chessManager  = chessManagerObj.GetComponent<ChessManager>();
        Vector3 cameraPosition = new Vector3((float)width / 2 - 0.5f, (float)height / 2 - 0.5f, -7);
        for (int x = 0; x< width; x++) {
            for (int y = 0; y < height; y++)
            {
                var spawnedTile = Instantiate(tilePrefab, new Vector3(x, y), Quaternion.identity, this.transform);
                string tileName  = (x+1).ToString() + (y+1).ToString();
                bool useAlgebraicTileNames = true;
                spawnedTile.name = tileName;
                var isOffset = (x % 2 == 0) && (y % 2 != 0) || (x % 2 != 0) && (y % 2 == 0);
                Vector3 tileNamePos = camera.transform.InverseTransformDirection(spawnedTile.transform.position - cameraPosition);
                tileNamePos.z = 2;
                spawnedTile.Init(isOffset, tileName, tileNamePos);
                chessManager.tiles.Add(int.Parse(tileName), spawnedTile);
            }
        }
        camera.transform.position = cameraPosition;
        chessManager.SetStartPosition();
    }
    public void MovePiece()
    {
        Tile startSquare = chessManager.tiles[clickedTile];
        Tile endSquare = chessManager.tiles[highlightedTile];
        char endSquarePiece = endSquare.piece.name;

        Debug.Log("tiles[endSquare].algebraicSquareName " + endSquare.algebraicSquareName.ToString());
        Debug.Log("currFEN.enPassant " + chessManager.currFEN.enPassant.ToString());
        //update tempFEN to previous move FEN
        chessManager.tempFEN = chessManager.currFEN;

        //check if its your turn
        if (chessManager.isWhitesTurn != startSquare.piece.isWhite)
        {
            Debug.Log("Not your turn!");
            return;
        }

        //released on different square than clicked, move is legal
        if (highlightedTile != clickedTile && chessManager.MoveIsLegal(clickedTile, highlightedTile))
        {
            //check if move is ambiguous (before moving piece)
            bool ambiguousMove = false;
            foreach (var tile in chessManager.tiles)
            {
                //skip startSquare, same piece type, and has same legal move
                if (tile.Value.squareName != startSquare.squareName && tile.Value.piece.name == startSquare.piece.name && tile.Value.piece.legalMoves.Contains(endSquare.squareName))
                {
                    ambiguousMove = true;
                    break;
                }
            }

            //mark piece taken, update tempFEN fiftyMoveRule
            bool pieceTaken = endSquarePiece == '-' ? false : true;
            if (pieceTaken | startSquare.piece.name == 'P' | startSquare.piece.name == 'p')
            {
                chessManager.tempFEN.fiftyMoveRule = 0;
            }
            else
            {
                Debug.Log("chessManager.tempFEN.fiftyMoveRule += 1");
                chessManager.tempFEN.fiftyMoveRule += 1;
            }
            //move piece
            endSquare.SetPiece(startSquare.piece.name);
            startSquare.SetPiece('-');

            //update kingLoc's
            if (endSquare.piece.name == 'K')
            {
                chessManager.whiteKingLoc = endSquare.squareName;
                chessManager.tempFEN.whiteKingsideCastling = false;
                chessManager.tempFEN.whiteQueensideCastling = false;
            }
            else if (endSquare.piece.name == 'k')
            {
                chessManager.blackKingLoc = endSquare.squareName;
                chessManager.tempFEN.blackKingsideCastling = false;
                chessManager.tempFEN.blackQueensideCastling = false;
            }

            //if in check after, undo, else continue
            chessManager.tiles = chessManager.SetAllLegalMoves(chessManager.tiles);
            if (chessManager.IsInCheck(chessManager.isWhitesTurn))
            {
                Debug.Log("Move illegal, puts you in check!");
                startSquare.SetPiece(endSquare.piece.name);
                endSquare.SetPiece(endSquarePiece);
                chessManager.tempFEN = chessManager.currFEN;
                chessManager.tiles = chessManager.SetAllLegalMoves(chessManager.tiles);
                return;
            }

            //castling
            int castleType = 0; // 1 is kingside, 2 is queenside
            if (endSquare.piece.name == 'K')
            {
                //castle kingside
                if (endSquare.squareName == 71)
                {
                    chessManager.tiles[81].SetPiece('-');
                    chessManager.tiles[61].SetPiece('R');
                    castleType = 1;
                }
                //castle queenside
                if (endSquare.squareName == 31)
                {
                    chessManager.tiles[11].SetPiece('-');
                    chessManager.tiles[41].SetPiece('R');
                    castleType = 2;
                }
            }
            else if (endSquare.piece.name == 'k')
            {
                chessManager.blackKingLoc = endSquare.squareName;
                chessManager.tempFEN.blackKingsideCastling = false;
                chessManager.tempFEN.blackQueensideCastling = false;
                //castle kingside
                if (endSquare.squareName == 78)
                {
                    chessManager.tiles[88].SetPiece('-');
                    chessManager.tiles[68].SetPiece('r');
                    castleType = 1;
                }
                //castle queenside
                if (endSquare.squareName == 38)
                {
                    chessManager.tiles[18].SetPiece('-');
                    chessManager.tiles[48].SetPiece('r');
                    castleType = 2;
                }
            }

            //check enPassant
            chessManager.tempFEN.enPassant = GetEnPassant(startSquare, endSquare);


            //add notation
            chessManager.AddMoveToNotation(startSquare.squareName, endSquare.squareName, pieceTaken, ambiguousMove, castleType);

            if (chessManager.IsInCheck(!chessManager.isWhitesTurn))
            {
                //check for mate
                if (chessManager.IsCheckmate())
                {
                    chessManager.AddToNotation("#");
                }
                //else add '+' to notation
                else
                {
                    chessManager.AddToNotation("+");
                }
            }

            //update notation, legal moves and currTurn
            //chessManager.tiles = chessManager.SetAllLegalMoves(chessManager.tiles);

            chessManager.isWhitesTurn = !chessManager.isWhitesTurn;
            chessManager.UpdateNotationGrid();
        }
        else 
        {
            Debug.Log("Move illegal");
            return;
        }
        chessManager.tiles = chessManager.SetAllLegalMoves(chessManager.tiles);
        chessManager.currFEN = chessManager.tempFEN;
        chessManager.PrintCurrentFENPosition();
    }
    private string GetEnPassant(Tile startSquare, Tile endSquare) //processed after move
    {
        //Debug.Log(startSquare.squareName);
        //Debug.Log(endSquare.squareName);
        string enPassant = "-";
        //not pawn movement
        if (endSquare.piece.name != 'P' &&  endSquare.piece.name != 'p') { return "-"; }

        //white
        if (endSquare.piece.isWhite)
        {
            //2 moves forward
            if (startSquare.squareName % 10 == 2 && endSquare.squareName - startSquare.squareName == 2)
            {
                int enPassantSquare = endSquare.squareName - 1;
                return chessManager.tiles[enPassantSquare].algebraicSquareName;
            }
        }
        else //isBlack
        {
            //2 moves forward
            if (startSquare.squareName % 10 == 7 && endSquare.squareName - startSquare.squareName == -2)
            {
                int enPassantSquare = endSquare.squareName + 1;
                return chessManager.tiles[enPassantSquare].algebraicSquareName;
            }
        }
        return enPassant;
    }
    private void UndoMove(char _endSquarePiece)
    {
        Tile startSquare = chessManager.tiles[clickedTile];
        Tile endSquare = chessManager.tiles[highlightedTile];

        if (endSquare.piece.name == 'K')
        {
            chessManager.whiteKingLoc = startSquare.squareName;
        }
        else if (endSquare.piece.name == 'k')
        {
            chessManager.blackKingLoc = startSquare.squareName;
        }
        startSquare.SetPiece(endSquare.piece.name);
        endSquare.SetPiece(_endSquarePiece);
        //chessManager.SetAllLegalMoves();
        chessManager.isWhitesTurn = !chessManager.isWhitesTurn;
        chessManager.notation.RemoveAt(chessManager.notation.Count - 1);
    }
    public void HighlightLegalMoveTiles(List<int> legalMoves)
    {
        foreach (int legalMove in legalMoves)
        {
            chessManager.tiles[legalMove].legalMoveHighlight.SetActive(true);
        }
    }
    public void UnhighlightLegalMoveTiles()
    {
        foreach (var tile in chessManager.tiles)
        {
            tile.Value.legalMoveHighlight.SetActive(false);
        }
    }
    public int AlgebraicToNumericNotation(string _algebraicNotation)
    {
        string column;
        int numericNotation = 0;
        switch (_algebraicNotation[0])
        {
            case 'a':
                numericNotation = 10;
                break;
            case 'b':
                numericNotation = 20;
                break;
            case 'c':
                numericNotation = 30;
                break;
            case 'd':
                numericNotation = 40;
                break;
            case 'e':
                numericNotation = 50;
                break;
            case 'f':
                numericNotation = 60;
                break;
            case 'g':
                numericNotation = 70;
                break;
            case 'h':
                numericNotation = 80;
                break;
        }
        numericNotation += int.Parse(_algebraicNotation[1].ToString());
        return numericNotation;
    }
}
