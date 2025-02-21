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

            // move piece, update tiles dict, mark piece taken
            bool pieceTaken = endSquarePiece == '-' ? false : true;
            endSquare.SetPiece(startSquare.piece.name);
            startSquare.SetPiece('-');
            chessManager.tiles = chessManager.SetAllLegalMoves(chessManager.tiles);

            //update kingLoc's
            if (endSquare.piece.name == 'K')
            {
                chessManager.whiteKingLoc = endSquare.squareName;
            }
            else if (endSquare.piece.name == 'k')
            {
                chessManager.blackKingLoc = endSquare.squareName;
            }

            //if in check after, undo, else continue
            if (chessManager.IsInCheck(chessManager.isWhitesTurn))
            {
                Debug.Log("Move illegal, puts you in check!");
                startSquare.SetPiece(endSquare.piece.name);
                endSquare.SetPiece(endSquarePiece);
                chessManager.tiles = chessManager.SetAllLegalMoves(chessManager.tiles);
                return;
            }



            //add notation
            chessManager.AddMoveToNotation(startSquare.squareName, endSquare.squareName, pieceTaken, ambiguousMove);

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

            //update notation and currTurn
            chessManager.isWhitesTurn = !chessManager.isWhitesTurn;
            chessManager.UpdateNotationGrid();
        }
        else 
        {
            Debug.Log("Move illegal");
            return; 
        }
        chessManager.PrintCurrentFENPosition();
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
