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
        char endSquarePiece = endSquare.pieceName;
        int currTurn = chessManager.isWhitesTurn ? 0 : 1;
        if (currTurn != startSquare.piece.GetComponent<Piece>().color)
        {
            Debug.Log("Not your turn!");
            return;
        }
        if (highlightedTile != clickedTile && chessManager.MoveIsLegal(clickedTile, highlightedTile))
        {

            endSquare.SetPiece(startSquare.pieceName);
            startSquare.SetPiece('-');
            chessManager.SetAllLegalMoves();
            chessManager.isWhitesTurn = !chessManager.isWhitesTurn;
            //Debug.Log("White in Check: " + chessManager.IsInCheck(true).ToString());
            //Debug.Log("Black in Check: " + chessManager.IsInCheck(false).ToString());
            if(endSquare.pieceName == 'K')
            {
                chessManager.whiteKingLoc = endSquare.squareName;
            }
            else if(endSquare.pieceName == 'k')
            {
                chessManager.blackKingLoc = endSquare.squareName;
            }
        }
        else { return; }
        if (chessManager.IsInCheck(!chessManager.isWhitesTurn)) 
        {
            UndoMove(endSquarePiece);
            Debug.Log("You are in check after that move");
        }
        else
        {
            bool pieceTaken = endSquarePiece == '-' ? false : true;
            chessManager.AddMoveToNotation(startSquare.squareName, endSquare.squareName, pieceTaken);
        }
    }
    private void UndoMove(char _endSquarePiece)
    {
        Tile startSquare = chessManager.tiles[clickedTile];
        Tile endSquare = chessManager.tiles[highlightedTile];

        if (endSquare.pieceName == 'K')
        {
            chessManager.whiteKingLoc = startSquare.squareName;
        }
        else if (endSquare.pieceName == 'k')
        {
            chessManager.blackKingLoc = startSquare.squareName;
        }
        startSquare.SetPiece(endSquare.pieceName);
        endSquare.SetPiece(_endSquarePiece);
        chessManager.SetAllLegalMoves();
        chessManager.isWhitesTurn = !chessManager.isWhitesTurn;
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
